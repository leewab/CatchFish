using System.Collections.Generic;
using Game;

/// <summary>
/// 在C#端用于显示的文本集合
/// 方便在制作多语言版本时替换文字
/// </summary>
public static class CSDisplayText
{
    private static Dictionary<string, Dictionary<string, string>> _messages;

    static CSDisplayText()
    {
        _messages = new Dictionary<string, Dictionary<string, string>>();

        var simplifiedChineseMessages = new Dictionary<string, string>
        {
            { "CheckForUpdates", "正在检查当前游戏版本" },
            { "Loading", "正在加载" },
            { "LoadingX", "正在加载{0}" },
            { "UpdatingAssets", "正在更新资源" },
            { "UpdatingLanguage", "正在更新语言包" },
            { "DeCompressAssets", "正在解压资源（不消耗流量）" },
            { "BundleMapAssets", "获取版本信息中" },
            { "Initializing", "正在初始化游戏（不消耗流量）" },
            { "ArrangingAssets", "正在整理游戏资源（不消耗流量）" },
            { "EnteringGame", "正在连入神魔大陆" },
            { "NetworkSpeed", "下载速度：{0} {1}" },
            { "UpdateResDesc1", "还需下载{0}资源\n<size=18>下载完成后即可获得最佳游戏体验</size>" },
            { "UpdateResDesc2", "重试" },
            { "UpdateResDesc3", "直接下载" },
            { "UpdateResDesc4", "边玩边下" },
            { "UpdateResDesc5", "您当前处于非WiFi下，是否确定更新{0}大小的资源吗？" },
            { "UpdateResDesc6", "当前无WIFI连接，是否边玩边下载完整游戏资源？\n<size=18>下载完整游戏资源后可领取丰厚奖励！</size>" },
            { "UpdateResDesc7", "暂不下载" },
            { "UpdateResDesc8", "确定下载" },
            { "UpdateResDesc9", "当前网络环境异常，建议检查网络设置后重试。" },
            { "LoadingDuringShader", "首次进入游戏，初始化中，请稍等" },
            { "SelectLanguage", "切换语言" },
            { "Confirm", "确定"},
            { "ForceUpdate", "当前版本过低，无法进入游戏，请前往官网或应用商店下载最新包"},
            { "ForceUpdate_GO", "前往官网"},
        };
        _messages.Add("SC", simplifiedChineseMessages);

        var traditionalChineseMessages = new Dictionary<string, string>
        {
            { "CheckForUpdates", "正在檢查當前遊戲版本" },
            { "Loading", "正在載入" },
            { "LoadingX", "正在載入{0}" },
            { "UpdatingAssets", "正在更新資源" },
            { "UpdatingLanguage", "正在更新語言包" },
            { "DeCompressAssets", "正在解壓資源（不消耗流量）" },
            { "BundleMapAssets", "正在更新資源列表" },
            { "Initializing", "正在初始化遊戲（不消耗流量）" },
            { "ArrangingAssets", "正在整理遊戲資源（不消耗流量）" },
            { "EnteringGame", "正在連入神魔大陸" },
            { "NetworkSpeed", "下載速度：{0} {1}" },
            { "UpdateResDesc1", "還需下載{0}資源\n<size=18>下載完成後即可獲得最佳遊戲體驗</size>" },
            { "UpdateResDesc2", "重試" },
            { "UpdateResDesc3", "直接下載" },
            { "UpdateResDesc4", "邊玩邊下" },
            { "UpdateResDesc5", "您當前處於非WiFi下，是否確定更新{0}大小的資源嗎？" },
            { "UpdateResDesc6", "當前無WIFI連接，是否邊玩邊下載完整遊戲資源？\n<size=18>下載完整遊戲資源後可領取豐厚獎勵！</size>" },
            { "UpdateResDesc7", "暫不下載" },
            { "UpdateResDesc8", "確定下載" },
            { "UpdateResDesc9", "當前網路環境異常，建議檢查網路設置後重試。" },
            { "LoadingDuringShader", "首次進入遊戲，初始化中，請稍等" },
            { "SelectLanguage", "切換語言" },
            { "Confirm", "確定"},
            { "ForceUpdate", "當前版本過低，無法進入遊戲，請前往官網或應用商店下載最新包。"},
            { "ForceUpdate_GO", "前往官網"},
        };
        _messages.Add("tc", traditionalChineseMessages);

        var russianMessages = new Dictionary<string, string>
        {
            { "CheckForUpdates", "Идет проверка текущей версии игры" },
            { "Loading", "Идет загрузка" },
            { "LoadingX", "Идет загрузка {0}" },
            { "UpdatingAssets", "Идет обновление ресурсов" },
            { "UpdatingLanguage", "Идет обновление языкового пакета" },
            { "DeCompressAssets", "Идет распаковка ресурсов (трафик не расходуется)" },
            { "BundleMapAssets", "Получение информации о версии" },
            { "Initializing", "Идет инициализация игры" },
            { "ArrangingAssets", "Разбирается в игровых ресурсах" },
            { "EnteringGame", "войти в игру" },
            { "NetworkSpeed", "Скорость загрузки: {0} {1}" },
            { "UpdateResDesc1", "Необходимо загрузить {0} ресурсов\n<size=18>После загрузки вы получите наилучший игровой опыт</size>" },
            { "UpdateResDesc2", "Попробовать снова" },
            { "UpdateResDesc3", "Прямая загрузка" },
            { "UpdateResDesc4", "Загружать во время игры" },
            { "UpdateResDesc5", "Вы не подключены к WiFi, все равно обновить ресурсы размером {0}?" },
            { "UpdateResDesc6", "Не подключены к WIFI. Продолжить загрузку во время игры, чтобы закончить обновление ресурсов?\n<size=18>После завершения загрузки вы получите щедрую награду!</size>" },
            { "UpdateResDesc7", "Пока не загружать" },
            { "UpdateResDesc8", "Подтвердить загрузку" },
            { "UpdateResDesc9", "Нестабильное интернет соединение, проверьте настройки интернета и попробуйте снова." },
            { "LoadingDuringShader", "Первый запуск игры, идет инициализация, пожалуйста, подождите" },
            { "SelectLanguage", "переключить язык" },
            { "Confirm", "ОК"},
            { "ForceUpdate", "игры устарела и не может быть запущена. Пожалуйста, посетите официальный сайт или магазин приложений для загрузки последнего обновления."},
            { "ForceUpdate_GO", "Перейти на официальный сайт"},
        };
        _messages.Add("ru", russianMessages);

        var vietnameseMessages = new Dictionary<string, string>
        {
            { "CheckForUpdates", "Kiểm tra phiên bản trò chơi hiện tại" },
            { "Loading", "Đang tải" },
            { "LoadingX", "Đang tải {0} " },
            { "UpdatingAssets", "Cập nhật tài nguyên" },
            { "UpdatingLanguage", "Đang cập nhật gói ngôn ngữ" },
            { "DeCompressAssets", "Giải nén tài nguyên (không tiêu tốn lưu lượng)" },
            { "BundleMapAssets", "Cập nhật danh sách tài nguyên" },
            { "Initializing", "Đang khởi tạo trò chơi (không tiêu tốn dữ liệu)" },
            { "ArrangingAssets", "Tổ chức tài nguyên trò chơi (không tiêu tốn dữ liệu)" },
            { "EnteringGame", "Kết nối với lục địa của các vị thần và ác quỷ" },
            { "NetworkSpeed", "Tốc độ tải xuống: {0}{1}" },
            { "UpdateResDesc1", "Bạn cũng cần tải xuống {0} tài nguyên\n<size=18>Sau khi quá trình tải xuống hoàn tất, bạn có thể có được trải nghiệm chơi trò chơi tốt nhất</size>" },
            { "UpdateResDesc2", "Thử lại" },
            { "UpdateResDesc3", "Tải xuống" },
            { "UpdateResDesc4", "Chơi trong khi chơi" },
            { "UpdateResDesc5", "Bạn hiện không có WiFi. Bạn có chắc chắn muốn cập nhật tài nguyên có kích thước {0} không?" },
            { "UpdateResDesc6", "Hiện tại không có kết nối WIFI. Bạn có muốn tải xuống tài nguyên trò chơi hoàn chỉnh khi chơi không? \n<size=18>Bạn có thể nhận được phần thưởng hậu hĩnh sau khi tải xuống toàn bộ tài nguyên trò chơi! </kích thước>" },
            { "UpdateResDesc7", "Chưa tải xuống" },
            { "UpdateResDesc8", "Xác nhận tải xuống" },
            { "UpdateResDesc9", "Môi trường mạng hiện tại không bình thường. Bạn nên kiểm tra cài đặt mạng và thử lại." },
            { "LoadingDuringShader", "Vào game lần đầu, đang khởi tạo, vui lòng chờ." },
            { "SelectLanguage", "chuyển đổi ngôn ngữ" },
            { "Confirm", "Đồng ý"},
            { "ForceUpdate", "Phiên bản hiện tại quá thấp để vào trò chơi. Vui lòng truy cập trang web chính thức hoặc cửa hàng ứng dụng để tải gói mới nhất."},
            { "ForceUpdate_GO", "Truy cập trang web chính thức"},
        };
        _messages.Add("vn", vietnameseMessages);

        var indonesianChineseMessages = new Dictionary<string, string>
        {
            { "CheckForUpdates", "Sedang memeriksa Versi saat ini" },
            { "Loading", "Sedang memuat" },
            { "LoadingX", "Sedang memuat {0}" },
            { "UpdatingAssets", "Sedang memperbarui Resource" },
            { "UpdatingLanguage", "Sedang memperbarui Language Pack" },
            { "DeCompressAssets", "Dekompresi sumber daya (Tidak memakai kuota)" },
            { "BundleMapAssets", "Sedang memperbarui Resource List" },
            { "Initializing", "Sedang menginisialisasi Game" },
            { "ArrangingAssets", "Sedang mengumpulkan Resource (Tidak memakai data)" },
            { "EnteringGame", "Sedang memasuki Forsaken World" },
            { "NetworkSpeed", "Kecepatan: {0} {1}" },
            { "UpdateResDesc1", "Perlu mengunduh {0} Resource\n<size=18>Rasakan pengalaman bermain yang terbaik setelah selesai mengunduh</size>" },
            { "UpdateResDesc2", "Coba Lagi" },
            { "UpdateResDesc3", "Download" },
            { "UpdateResDesc4", "Bermain sembari Download" },
            { "UpdateResDesc5", "Anda terhubung dengan WIFI, apakah setuju memperbarui Resource dengan ukura {0}?" },
            { "UpdateResDesc6", "Tidak terhubung dengan WIFI, apakah Bermain sembari Download, mengunduh Resource utuh?\n<size=18>dapatkan Reward berlimpah setelah selesai mengunduh!</size>" },
            { "UpdateResDesc7", "Nanti Saja" },
            { "UpdateResDesc8", "Setuju" },
            { "UpdateResDesc9", "Jaringan abnormal, periksa pengaturan jaringan lalu coba lagi." },
            { "LoadingDuringShader", "Pertama kali masuk game, sedang inisialisasi, harap tunggu." },
            { "SelectLanguage", "beralih bahasa" },
            { "Confirm", "Setuju"},
            { "ForceUpdate", "Versi saat ini terlalu rendah untuk masuk ke dalam permainan. Silakan kunjungi situs resmi atau toko aplikasi untuk mengunduh paket terbaru."},
            { "ForceUpdate_GO", "Kunjungi situs resmi"},
        };
        _messages.Add("id", indonesianChineseMessages);

        var englishMessages = new Dictionary<string, string>
        {
            { "CheckForUpdates", "Checking current game version" },
            { "Loading", "Loading" },
            { "LoadingX", "Loading {0}" },
            { "UpdatingAssets", "Updating resource files" },
            { "UpdatingLanguage", "Updating language pack" },
            { "DeCompressAssets", "Unpacking resource files... (doesn't use data)" },
            { "BundleMapAssets", "Updating resource file list" },
            { "Initializing", "Initializing game (does not use data)" },
            { "ArrangingAssets", "Organizing game resources... (doesn't use data)" },
            { "EnteringGame", "Connecting to Forsaken World" },
            { "NetworkSpeed", "Download Speed: {0} {1}" },
            { "UpdateResDesc1", "You still need to download {0} resource files.\n<size=18>Finish the download for the best gaming experience.</size>" },
            { "UpdateResDesc2", "Retry" },
            { "UpdateResDesc3", "Download now" },
            { "UpdateResDesc4", "Download while playing" },
            { "UpdateResDesc5", "You are not on a Wi-Fi connection. Are you sure you want to update? Size: {0}" },
            { "UpdateResDesc6", "Not connected to Wi-Fi. Are you sure you want to download all the game resources while you play?\n<size=18>Download the game resources and you can claim a great reward!</size>" },
            { "UpdateResDesc7", "Not Now" },
            { "UpdateResDesc8", "Confirm" },
            { "UpdateResDesc9", "Current network error. It is recommended to try again after checking the network settings." },
            { "LoadingDuringShader", "First time entering the game. Initializing it, please wait" },
            { "SelectLanguage", "switch language" },
            { "Confirm", "OK"},
            { "ForceUpdate", "The current version is too low to enter the game. Please visit the official website or app store to download the latest package."},
            { "ForceUpdate_GO", "Visit the official website"},
        };
        _messages.Add("en", englishMessages);

        var thaiMessages = new Dictionary<string, string>
        {
            { "CheckForUpdates", "กำลังตรวจสอบเวอร์ชันเกมปัจจุบัน" },
            { "Loading", "กำลังโหลด" },
            { "LoadingX", "กำลังโหลด{0}" },
            { "UpdatingAssets", "กำลังอัปเดตทรัพยากร" },
            { "UpdatingLanguage", "กำลังอัปเดตแพ็กภาษา" },
            { "DeCompressAssets", "กำลังคลายการบีบอัดทรัพยากร (ไม่ใช้ปริมาณอินเทอร์เน็ต)" },
            { "BundleMapAssets", "กำลังอัปเดตรายการทรัพยากร" },
            { "Initializing", "กำลังเริ่มต้นเกม (ไม่ใช้ปริมาณอินเทอร์เน็ต)" },
            { "ArrangingAssets", "กำลังจัดระเบียบทรัพยากรเกม (ไม่ใช้ปริมาณอินเทอร์เน็ต)" },
            { "EnteringGame", "กำลังเชื่อมต่อ Forsaken World" },
            { "NetworkSpeed", "ความเร็วดาวน์โหลด : {0} {1}" },
            { "UpdateResDesc1", "ยังจำเป็นต้องดาวน์โหลดทรัพยากร {0} \n<size=18>หลังดาวน์โหลดเสร็จจะสามารถรับประสบการณ์เกมที่ดีที่สุดได้</size>" },
            { "UpdateResDesc2", "ลองอีกครั้ง" },
            { "UpdateResDesc3", "ดาวน์โหลดทันที" },
            { "UpdateResDesc4", "ดาวน์โหลดขณะเล่นเกม" },
            { "UpdateResDesc5", "ปัจจุบันท่านอยู่ในสถานะไม่มี Wi-Fi ยืนยันอัปเดตทรัพยากรขนาด {0} หรือไม่?" },
            { "UpdateResDesc6", "ปัจจุบันไม่มีการเชื่อมต่อ Wi-Fi จะดาวน์โหลดทรัพยากรเกมสมบูรณ์ขณะเล่นเกมหรือไม่? \n<size=18>ดาวน์โหลดทรัพยากรสมบูรณ์แล้วจะได้รับรางวัลมากมาย!</size>" },
            { "UpdateResDesc7", "ยังไม่ดาวน์โหลด" },
            { "UpdateResDesc8", "ยืนยันดาวน์โหลด" },
            { "UpdateResDesc9", "สภาพแวดล้อมเครือข่ายปัจจุบันผิดปกติ แนะนำให้ตรวจสอบการตั้งค่าเครือข่ายแล้วลองอีกครั้ง" },
            { "LoadingDuringShader", "เข้าเกมครั้งแรก กำลังปรับแต่งเริ่มต้น โปรดรอสักครู่" },
            { "SelectLanguage", "เปลี่ยนภาษา" },
            { "Confirm", "ยืนยัน"},
            { "ForceUpdate", "เวอร์ชันปัจจุบันต่ำเกินไป ไม่สามารถเข้าเกมได้ กรุณาไปที่เว็บไซต์ทางการหรือร้านค้าแอปเพื่อดาวน์โหลดแพ็กเกจล่าสุด"},
            { "ForceUpdate_GO", "ไปที่เว็บไซต์ทางการ"},
        };
        _messages.Add("th", thaiMessages);
    }

    public static string GetMessage(string code)
    {
        string language = LanguageHelper.Instance.SelectLanguage;
        if (_messages.ContainsKey(language) && _messages[language].ContainsKey(code))
        {
            return _messages[language][code];
        }
        return "";
    }

}