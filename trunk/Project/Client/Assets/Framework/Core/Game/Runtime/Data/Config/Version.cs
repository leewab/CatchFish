namespace Game
{
    /// <summary>
    /// version版本信息
    ///     用于热更比对版本信息
    /// </summary>
    public class Version
    {
        public string VersionID;
        public string VersionMD5;
        public string BuildConfigID;
        public string BuildConfigMD5;
        public string LuaID;
        public string LuaMD5;
        public string ResID;
        public string ResMD5;
    }

    public enum VersionState
    {
        Normal, //正常版本
        Different, //不同版本
        LowVersion, //低版本
        HighVersion //高版本
    }
}