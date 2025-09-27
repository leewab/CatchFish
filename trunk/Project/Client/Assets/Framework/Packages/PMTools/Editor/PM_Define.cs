namespace Framework.PM
{
    public class PM_Define
    {
        private static string remoteUrl = "http://ibinggame.ticp.io";
        private static string localUrl = "http://127.0.0.1:8081";
                 
        public static string ServerURL => PM_Main.IsLocalServer ? localUrl : remoteUrl;
        public static string AdminURL = ServerURL + "/pm/admin?operation={0}";
        public static string PmURL = ServerURL + "/pm";
        public static string UploadURL = ServerURL + "/pm/upload";
        public static string DownloadURL = ServerURL + "/pm/download?filePath={0}";
    }
}