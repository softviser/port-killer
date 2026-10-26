namespace PortKiller;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var mutex = new Mutex(true, "Softviser.PortKiller.SingleInstance", out bool createdNew);
        if (!createdNew) return; // zaten tepside çalışıyor

        ApplicationConfiguration.Initialize();
        Application.Run(new TrayApp());
    }
}
