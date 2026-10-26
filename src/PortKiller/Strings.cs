using System.Globalization;

namespace PortKiller;

/// <summary>Arayüz metinleri; Windows görüntü diline göre seçilir, bulunamazsa İngilizce.</summary>
internal static class S
{
    public static string Show => T[0];
    public static string Refresh => T[1];
    public static string Exit => T[2];
    public static string Search => T[3];
    public static string Kill => T[4];
    public static string App => T[5];
    public static string Address => T[6];
    public static string Other => T[7];
    public static string NewPort => T[8];
    public static string Empty => T[9];
    public static string ConfirmKill => T[10];
    public static string KillFailed => T[11];
    public static string About => T[12];

    public static bool IsRtl => CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

    public static MessageBoxOptions MsgOptions => IsRtl ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0;

    private static string[] T => Pick(CultureInfo.CurrentUICulture);

    // ICU'da zh-CN'in üstü "zh-Hans" değil "zh" olabiliyor; Norveççe de nb/nn olarak gelir.
    static S()
    {
        Table["zh"] = Table["zh-Hans"];
        Table["nb"] = Table["nn"] = Table["no"];
    }

    // zh-TW → zh-Hant, nb-NO / nn-NO → no gibi üst kültürlere düşerek arar.
    internal static string[] Pick(CultureInfo culture)
    {
        for (var c = culture; !c.Equals(CultureInfo.InvariantCulture); c = c.Parent)
            if (Table.TryGetValue(c.Name, out var t)) return t;
        return Table["en"];
    }

    // Sıra: Show, Refresh, Exit, Search, Kill, App, Address, Other, NewPort, Empty, ConfirmKill, KillFailed, About
    internal static readonly Dictionary<string, string[]> Table = new(StringComparer.OrdinalIgnoreCase)
    {
        ["en"] = ["Show ports", "Refresh", "Exit", "Search", "Kill process", "Application", "Address", "Other applications", "New port", "No listening ports", "This is not a development tool. Kill it anyway?", "Could not kill the process. Try running as administrator.", "About"],
        ["tr"] = ["Portları göster", "Yenile", "Çıkış", "Ara", "Süreci sonlandır", "Uygulama", "Adres", "Diğer uygulamalar", "Yeni port", "Dinlenen port yok", "Bu bir geliştirme aracı değil. Yine de sonlandırılsın mı?", "Süreç sonlandırılamadı. Yönetici olarak çalıştırmayı deneyin.", "Hakkında"],
        ["de"] = ["Ports anzeigen", "Aktualisieren", "Beenden", "Suchen", "Prozess beenden", "Anwendung", "Adresse", "Andere Anwendungen", "Neuer Port", "Keine lauschenden Ports", "Dies ist kein Entwicklungswerkzeug. Trotzdem beenden?", "Der Prozess konnte nicht beendet werden. Als Administrator ausführen.", "Info"],
        ["fr"] = ["Afficher les ports", "Actualiser", "Quitter", "Rechercher", "Terminer le processus", "Application", "Adresse", "Autres applications", "Nouveau port", "Aucun port en écoute", "Ce n'est pas un outil de développement. Le terminer quand même ?", "Impossible de terminer le processus. Essayez en tant qu'administrateur.", "À propos"],
        ["es"] = ["Mostrar puertos", "Actualizar", "Salir", "Buscar", "Finalizar proceso", "Aplicación", "Dirección", "Otras aplicaciones", "Nuevo puerto", "No hay puertos en escucha", "No es una herramienta de desarrollo. ¿Finalizarlo de todos modos?", "No se pudo finalizar el proceso. Pruebe a ejecutar como administrador.", "Acerca de"],
        ["it"] = ["Mostra porte", "Aggiorna", "Esci", "Cerca", "Termina processo", "Applicazione", "Indirizzo", "Altre applicazioni", "Nuova porta", "Nessuna porta in ascolto", "Non è uno strumento di sviluppo. Terminarlo comunque?", "Impossibile terminare il processo. Prova a eseguire come amministratore.", "Informazioni"],
        ["pt"] = ["Mostrar portas", "Atualizar", "Sair", "Pesquisar", "Encerrar processo", "Aplicativo", "Endereço", "Outros aplicativos", "Nova porta", "Nenhuma porta em escuta", "Não é uma ferramenta de desenvolvimento. Encerrar mesmo assim?", "Não foi possível encerrar o processo. Tente executar como administrador.", "Sobre"],
        ["nl"] = ["Poorten tonen", "Vernieuwen", "Afsluiten", "Zoeken", "Proces beëindigen", "Toepassing", "Adres", "Andere toepassingen", "Nieuwe poort", "Geen luisterende poorten", "Dit is geen ontwikkeltool. Toch beëindigen?", "Het proces kon niet worden beëindigd. Probeer als administrator.", "Info"],
        ["pl"] = ["Pokaż porty", "Odśwież", "Zakończ", "Szukaj", "Zakończ proces", "Aplikacja", "Adres", "Inne aplikacje", "Nowy port", "Brak nasłuchujących portów", "To nie jest narzędzie programistyczne. Mimo to zakończyć?", "Nie można zakończyć procesu. Spróbuj uruchomić jako administrator.", "O programie"],
        ["cs"] = ["Zobrazit porty", "Obnovit", "Konec", "Hledat", "Ukončit proces", "Aplikace", "Adresa", "Ostatní aplikace", "Nový port", "Žádné naslouchající porty", "Toto není vývojový nástroj. Přesto ukončit?", "Proces nelze ukončit. Zkuste spustit jako správce.", "O aplikaci"],
        ["sk"] = ["Zobraziť porty", "Obnoviť", "Koniec", "Hľadať", "Ukončiť proces", "Aplikácia", "Adresa", "Ostatné aplikácie", "Nový port", "Žiadne načúvajúce porty", "Toto nie je vývojársky nástroj. Napriek tomu ukončiť?", "Proces sa nepodarilo ukončiť. Skúste spustiť ako správca.", "O aplikácii"],
        ["hu"] = ["Portok megjelenítése", "Frissítés", "Kilépés", "Keresés", "Folyamat leállítása", "Alkalmazás", "Cím", "Egyéb alkalmazások", "Új port", "Nincs figyelő port", "Ez nem fejlesztőeszköz. Mégis leállítja?", "A folyamat nem állítható le. Próbálja rendszergazdaként futtatni.", "Névjegy"],
        ["ro"] = ["Afișează porturile", "Reîmprospătează", "Ieșire", "Căutare", "Termină procesul", "Aplicație", "Adresă", "Alte aplicații", "Port nou", "Niciun port în ascultare", "Nu este un instrument de dezvoltare. Îl terminați oricum?", "Procesul nu a putut fi terminat. Încercați să rulați ca administrator.", "Despre"],
        ["bg"] = ["Покажи портовете", "Обнови", "Изход", "Търсене", "Прекрати процеса", "Приложение", "Адрес", "Други приложения", "Нов порт", "Няма слушащи портове", "Това не е инструмент за разработка. Да се прекрати ли?", "Процесът не може да бъде прекратен. Опитайте като администратор.", "Относно"],
        ["hr"] = ["Prikaži portove", "Osvježi", "Izlaz", "Traži", "Završi proces", "Aplikacija", "Adresa", "Ostale aplikacije", "Novi port", "Nema portova koji slušaju", "Ovo nije razvojni alat. Ipak završiti?", "Proces nije moguće završiti. Pokušajte pokrenuti kao administrator.", "O programu"],
        ["ru"] = ["Показать порты", "Обновить", "Выход", "Поиск", "Завершить процесс", "Приложение", "Адрес", "Другие приложения", "Новый порт", "Нет прослушиваемых портов", "Это не инструмент разработки. Всё равно завершить?", "Не удалось завершить процесс. Попробуйте запустить от имени администратора.", "О программе"],
        ["uk"] = ["Показати порти", "Оновити", "Вихід", "Пошук", "Завершити процес", "Програма", "Адреса", "Інші програми", "Новий порт", "Немає портів, що прослуховуються", "Це не інструмент розробки. Все одно завершити?", "Не вдалося завершити процес. Спробуйте запустити від імені адміністратора.", "Про програму"],
        ["el"] = ["Εμφάνιση θυρών", "Ανανέωση", "Έξοδος", "Αναζήτηση", "Τερματισμός διεργασίας", "Εφαρμογή", "Διεύθυνση", "Άλλες εφαρμογές", "Νέα θύρα", "Δεν υπάρχουν θύρες σε ακρόαση", "Δεν είναι εργαλείο ανάπτυξης. Τερματισμός παρ' όλα αυτά;", "Η διεργασία δεν τερματίστηκε. Δοκιμάστε εκτέλεση ως διαχειριστής.", "Σχετικά"],
        ["sv"] = ["Visa portar", "Uppdatera", "Avsluta", "Sök", "Avsluta process", "Program", "Adress", "Andra program", "Ny port", "Inga lyssnande portar", "Detta är inget utvecklingsverktyg. Avsluta ändå?", "Processen kunde inte avslutas. Prova att köra som administratör.", "Om"],
        ["da"] = ["Vis porte", "Opdater", "Afslut", "Søg", "Afslut proces", "Program", "Adresse", "Andre programmer", "Ny port", "Ingen lyttende porte", "Dette er ikke et udviklingsværktøj. Afslut alligevel?", "Processen kunne ikke afsluttes. Prøv at køre som administrator.", "Om"],
        ["no"] = ["Vis porter", "Oppdater", "Avslutt", "Søk", "Avslutt prosess", "Program", "Adresse", "Andre programmer", "Ny port", "Ingen lyttende porter", "Dette er ikke et utviklingsverktøy. Avslutte likevel?", "Prosessen kunne ikke avsluttes. Prøv å kjøre som administrator.", "Om"],
        ["fi"] = ["Näytä portit", "Päivitä", "Lopeta", "Hae", "Lopeta prosessi", "Sovellus", "Osoite", "Muut sovellukset", "Uusi portti", "Ei kuuntelevia portteja", "Tämä ei ole kehitystyökalu. Lopetetaanko silti?", "Prosessia ei voitu lopettaa. Kokeile suorittaa järjestelmänvalvojana.", "Tietoja"],
        ["ar"] = ["عرض المنافذ", "تحديث", "خروج", "بحث", "إنهاء العملية", "التطبيق", "العنوان", "تطبيقات أخرى", "منفذ جديد", "لا توجد منافذ مستمعة", "هذه ليست أداة تطوير. هل تريد إنهاءها على أي حال؟", "تعذر إنهاء العملية. جرّب التشغيل كمسؤول.", "حول"],
        ["he"] = ["הצג פורטים", "רענן", "יציאה", "חיפוש", "סיים תהליך", "יישום", "כתובת", "יישומים אחרים", "פורט חדש", "אין פורטים מאזינים", "זה אינו כלי פיתוח. לסיים בכל זאת?", "לא ניתן לסיים את התהליך. נסה להפעיל כמנהל.", "אודות"],
        ["fa"] = ["نمایش پورت‌ها", "بازخوانی", "خروج", "جستجو", "پایان فرایند", "برنامه", "نشانی", "برنامه‌های دیگر", "پورت جدید", "هیچ پورت شنونده‌ای نیست", "این ابزار توسعه نیست. با این حال پایان داده شود؟", "پایان فرایند ممکن نشد. آن را به‌عنوان مدیر اجرا کنید.", "درباره"],
        ["hi"] = ["पोर्ट दिखाएँ", "रीफ़्रेश", "बाहर निकलें", "खोजें", "प्रोसेस बंद करें", "ऐप्लिकेशन", "पता", "अन्य ऐप्लिकेशन", "नया पोर्ट", "कोई सुनने वाला पोर्ट नहीं", "यह डेवलपमेंट टूल नहीं है। फिर भी बंद करें?", "प्रोसेस बंद नहीं हो सका। व्यवस्थापक के रूप में चलाकर देखें।", "परिचय"],
        ["ja"] = ["ポートを表示", "更新", "終了", "検索", "プロセスを終了", "アプリケーション", "アドレス", "その他のアプリ", "新しいポート", "待ち受け中のポートはありません", "開発ツールではありません。終了しますか?", "プロセスを終了できませんでした。管理者として実行してください。", "バージョン情報"],
        ["ko"] = ["포트 보기", "새로 고침", "종료", "검색", "프로세스 종료", "애플리케이션", "주소", "기타 앱", "새 포트", "수신 대기 중인 포트 없음", "개발 도구가 아닙니다. 그래도 종료할까요?", "프로세스를 종료할 수 없습니다. 관리자 권한으로 실행해 보세요.", "정보"],
        ["zh-Hans"] = ["显示端口", "刷新", "退出", "搜索", "结束进程", "应用程序", "地址", "其他应用", "新端口", "没有监听的端口", "这不是开发工具。仍要结束吗?", "无法结束进程。请尝试以管理员身份运行。", "关于"],
        ["zh-Hant"] = ["顯示連接埠", "重新整理", "結束", "搜尋", "結束處理程序", "應用程式", "位址", "其他應用程式", "新連接埠", "沒有正在接聽的連接埠", "這不是開發工具。仍要結束嗎?", "無法結束處理程序。請嘗試以系統管理員身分執行。", "關於"],
        ["id"] = ["Tampilkan port", "Segarkan", "Keluar", "Cari", "Akhiri proses", "Aplikasi", "Alamat", "Aplikasi lain", "Port baru", "Tidak ada port yang mendengarkan", "Ini bukan alat pengembangan. Tetap akhiri?", "Proses tidak dapat diakhiri. Coba jalankan sebagai administrator.", "Tentang"],
        ["ms"] = ["Tunjukkan port", "Segar semula", "Keluar", "Cari", "Tamatkan proses", "Aplikasi", "Alamat", "Aplikasi lain", "Port baharu", "Tiada port yang mendengar", "Ini bukan alat pembangunan. Tamatkan juga?", "Proses tidak dapat ditamatkan. Cuba jalankan sebagai pentadbir.", "Perihal"],
        ["vi"] = ["Hiện cổng", "Làm mới", "Thoát", "Tìm kiếm", "Kết thúc tiến trình", "Ứng dụng", "Địa chỉ", "Ứng dụng khác", "Cổng mới", "Không có cổng đang lắng nghe", "Đây không phải công cụ phát triển. Vẫn kết thúc?", "Không thể kết thúc tiến trình. Hãy thử chạy với quyền quản trị.", "Giới thiệu"],
        ["th"] = ["แสดงพอร์ต", "รีเฟรช", "ออก", "ค้นหา", "สิ้นสุดโปรเซส", "แอปพลิเคชัน", "ที่อยู่", "แอปอื่น ๆ", "พอร์ตใหม่", "ไม่มีพอร์ตที่รอรับการเชื่อมต่อ", "นี่ไม่ใช่เครื่องมือพัฒนา ต้องการสิ้นสุดหรือไม่?", "ไม่สามารถสิ้นสุดโปรเซสได้ ลองเรียกใช้ในฐานะผู้ดูแลระบบ", "เกี่ยวกับ"],
    };
}
