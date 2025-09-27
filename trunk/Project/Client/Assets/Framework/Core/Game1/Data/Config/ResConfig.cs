using System.Collections.Generic;

namespace Framework.Core
{
    public class ResConfig
    {
        public int ResVersion;
        public List<ResGroup> ResGroups;
    }
    
    /// <summary>
    /// Res分组信息
    /// </summary>
    public class ResGroup
    {
        public string GroupName;
        public string GroupHash;
        public float GroupUpdateSize;            //该分组下更新改变的大小
        public List<ResInfo> ResAssets;
    }
    
    /// <summary>
    /// Res资源信息 包括AB资源和独立资源
    /// </summary>
    public class ResInfo
    {
        public string Name;
        public string Path;
        public string Hash;
        public float Size;
    }

    public enum ABGroupType
    {
        Prefabs,
        Lua,
        Media,
        Custom
    }
}