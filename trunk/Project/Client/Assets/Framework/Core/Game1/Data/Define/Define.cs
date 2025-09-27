namespace Framework.Core
{
    public partial class Define
    {
        public const string MediaResPath = "/Media/";                       //媒体文件（包括音效、视频）
        public const string IconResPath = "/Icon/";                         //图标（动态加载的图片）
        public const string PrefabsResPath = "/Prefabs/";                   //包括预制体、材质球、图集、动画、特效
        public const string LuaResPath = "/Lua/";                           //Lua文件
        public const string ConfigResPath = "/Config/";                     //配表文件
        public const string BaseConfigPath = "/Scripts/BaseConfig/";        //配表基类
        public const string Prefabs_AssetBundleManifest = "Prefabs";        //Prefabs预制体的manifest文件
#if UNITY_TOLUA
        public const string Lua_AssetBundleManifest = "Lua";                //Lua AB包的manifest文件
#endif
    }
}