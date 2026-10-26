using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace PortKiller;

internal sealed record Listener(int Port, int Pid, string Name, string Address)
{
    public bool IsDev => Ports.DevTools.ContainsKey(Name);
    public string Group => Ports.DevTools.TryGetValue(Name, out var label) ? label : S.Other;
}

/// <summary>Bu makinede LISTEN durumundaki TCP portları (netstat -ano'nun dil bağımsız karşılığı).</summary>
internal static class Ports
{
    /// <summary>Süreç adı (uzantısız) → grup başlığı. Burada olanlar geliştirme aracı sayılır.</summary>
    public static readonly Dictionary<string, string> DevTools = new(StringComparer.OrdinalIgnoreCase)
    {
        ["node"] = "Node.js", ["bun"] = "Bun", ["deno"] = "Deno", ["esbuild"] = "esbuild",
        ["python"] = "Python", ["pythonw"] = "Python", ["java"] = "Java", ["javaw"] = "Java",
        ["dotnet"] = ".NET", ["iisexpress"] = "IIS Express", ["php"] = "PHP", ["php-cgi"] = "PHP",
        ["ruby"] = "Ruby", ["go"] = "Go", ["electron"] = "Electron",
        ["com.docker.backend"] = "Docker", ["docker-proxy"] = "Docker", ["wslrelay"] = "WSL",
        ["postgres"] = "PostgreSQL", ["mysqld"] = "MySQL", ["mariadbd"] = "MariaDB",
        ["redis-server"] = "Redis", ["mongod"] = "MongoDB", ["nginx"] = "nginx", ["httpd"] = "Apache",
        ["caddy"] = "Caddy", ["ngrok"] = "ngrok",
        ["Code"] = "VS Code", ["devenv"] = "Visual Studio", ["idea64"] = "IntelliJ IDEA",
        ["rider64"] = "Rider", ["webstorm64"] = "WebStorm", ["pycharm64"] = "PyCharm",
    };

    // Kapatılırsa Windows'u bozabilecek süreçler; listede hiç görünmez.
    private static readonly HashSet<string> SystemProcesses = new(StringComparer.OrdinalIgnoreCase)
    {
        "System", "Idle", "svchost", "lsass", "services", "wininit", "winlogon", "csrss", "smss",
        "spoolsv", "MsMpEng", "NisSrv", "SearchIndexer", "dasHost", "Registry",
    };

    // 0-1023 iyi bilinen sistem portları (SMB, RPC, HTTP.sys...) gösterilmez.
    private const int FirstUserPort = 1024;

    public static List<Listener> Snapshot()
    {
        var names = new Dictionary<int, string?>();
        return Read(AF_INET).Concat(Read(AF_INET6))
            .Where(r => r.Port >= FirstUserPort)
            .GroupBy(r => (r.Port, r.Pid))
            .Select(g => (g.Key, Name: NameOf(g.Key.Pid, names), Address: string.Join(", ", g.Select(r => r.Address).Distinct())))
            .Where(x => x.Name != null && !SystemProcesses.Contains(x.Name))
            .Select(x => new Listener(x.Key.Port, x.Key.Pid, x.Name!, x.Address))
            .OrderBy(l => l.Port)
            .ToList();
    }

    /// <summary>Stop-Process -Force ile aynı: süreci doğrudan sonlandırır.</summary>
    public static void Kill(int pid)
    {
        using var p = Process.GetProcessById(pid);
        p.Kill();
        p.WaitForExit(2000);
    }

    private static string? NameOf(int pid, Dictionary<int, string?> cache)
    {
        if (cache.TryGetValue(pid, out var name)) return name;
        try
        {
            using var p = Process.GetProcessById(pid);
            name = p.ProcessName;
        }
        catch { name = null; } // tarama sırasında kapanmış
        return cache[pid] = name;
    }

    private const int AF_INET = 2, AF_INET6 = 23, TCP_TABLE_OWNER_PID_LISTENER = 3, ERROR_INSUFFICIENT_BUFFER = 122;

    [DllImport("iphlpapi.dll")]
    private static extern uint GetExtendedTcpTable(IntPtr table, ref int size, bool sort, int af, int tableClass, int reserved);

    private static List<(int Port, int Pid, string Address)> Read(int af)
    {
        int size = 0;
        GetExtendedTcpTable(IntPtr.Zero, ref size, false, af, TCP_TABLE_OWNER_PID_LISTENER, 0);
        while (true)
        {
            var buf = Marshal.AllocHGlobal(size);
            try
            {
                uint rc = GetExtendedTcpTable(buf, ref size, false, af, TCP_TABLE_OWNER_PID_LISTENER, 0);
                if (rc == ERROR_INSUFFICIENT_BUFFER) continue; // iki çağrı arasında tablo büyüdü
                if (rc != 0) throw new Win32Exception((int)rc);

                int count = Marshal.ReadInt32(buf);
                var rows = new List<(int, int, string)>(count);
                for (int i = 0; i < count; i++)
                {
                    if (af == AF_INET)
                    {
                        // MIB_TCPROW_OWNER_PID: state, localAddr, localPort, remoteAddr, remotePort, pid (24 bayt)
                        var row = buf + 4 + i * 24;
                        var ip = new IPAddress((uint)Marshal.ReadInt32(row, 4));
                        rows.Add((PortOf(Marshal.ReadInt32(row, 8)), Marshal.ReadInt32(row, 20), ip.ToString()));
                    }
                    else
                    {
                        // MIB_TCP6ROW_OWNER_PID: localAddr[16], scope, localPort, remoteAddr[16], scope, remotePort, state, pid (56 bayt)
                        var row = buf + 4 + i * 56;
                        var bytes = new byte[16];
                        Marshal.Copy(row, bytes, 0, 16);
                        rows.Add((PortOf(Marshal.ReadInt32(row, 20)), Marshal.ReadInt32(row, 52), $"[{new IPAddress(bytes)}]"));
                    }
                }
                return rows;
            }
            finally { Marshal.FreeHGlobal(buf); }
        }
    }

    // Port, alt 16 bitte ağ bayt sırasıyla (big-endian) durur.
    private static int PortOf(int raw) => ((raw & 0xFF) << 8) | ((raw >> 8) & 0xFF);
}
