using MUEngine.Implement;
#if UNITY_TOLUA
using LuaInterface;
#endif

namespace MUGame
{
    public class UpdateMgrUtil
    {

#if UNITY_TOLUA
        private static LuaTable _table;
        private static LuaFunction _OnHandlerFun;
        private static LuaFunction _OnWarningBackGroundSizeFun;

        public static void Initialize(LuaTable table)
        {
            _table = table;

            _OnHandlerFun = _table.GetLuaFunction("OnHandler");
            _OnWarningBackGroundSizeFun = _table.GetLuaFunction("OnWarningBackGroundSize");

            MUCore.UpdateMgr.OnHandler = OnHandler;
            MUEngine.UpdateDef.WarningBackGroundSize = OnWarningBackGroundSize;
        }
#endif

        #region 分包下载

        /// <summary>
        /// 选择语言名称
        /// </summary>
        public static string GetSelectLanguage()
        {
            return MUCore.UpdateMgr.LangConfig.Name;
        }

        /// <summary>
        /// 语言名称集合
        /// </summary>
        /// <returns></returns>
        public static string GetLanguageName()
        {
            if(MUCore.UpdateMgr.LangConfig.SupportList != null)
            {
                return string.Join("|", MUCore.UpdateMgr.LangConfig.SupportList);
            }
            return string.Empty;
        }
        /// <summary>
        /// 检测subpackage是否完成
        /// </summary>
        /// <returns></returns>
        public static bool IsDoneSubPackage()
        {
            return MUCore.UpdateMgr.IsDoneSubPackage();
        }
        /// <summary>
        /// 获取分包信息
        /// </summary>
        /// <returns></returns>
        public static string GetSubPackageInfo()
        {
            return MUCore.UpdateMgr.BGUpdate.GetPackageInfo();
        }
        /// <summary>
        /// 获取单个分包下载进度
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static float GetProgressBySubPackage(int index)
        {
            return MUCore.UpdateMgr.BGUpdate.GetProgress(index);
        }

        public static int GetItemStateBySubPackage(int index)
        {
            return MUCore.UpdateMgr.BGUpdate.GetItemState(index);
        }
        /// <summary>
        /// 获取分包下载速度
        /// </summary>
        /// <returns></returns>
        public static float GetSpeedBySubPackage()
        {
            return MUCore.UpdateMgr.BGUpdate.GetSpeed();
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="count"></param>
        public static void SetMaxLoadCount(int count)
        {
            MUCore.UpdateMgr.BGUpdate.SetMaxLoadCount(count);
        }

        public static void SetMaxSpeed(int speed)
        {
            MUCore.UpdateMgr.BGUpdate.SetMaxSpeed(speed);
        }

        public static bool GetEnableBySubPackage()
        {
            return MUCore.UpdateMgr.BGUpdate.GetDownloadEnable();
        }
        /// <summary>
        /// 是否激活分包下载
        /// </summary>
        /// <returns></returns>
        public static bool IsActiveBySubPackage()
        {
            return MUCore.UpdateMgr.IsActiveSubPackage;
        }

        #endregion

        public static void SetFlag(int flag)
        {
            MUCore.UpdateMgr.SetFlag(flag);
        }
        /// <summary>
        /// 引擎回调方法
        /// </summary>
        /// <param name="value"></param>
        private static void OnHandler(int value,string str)
        {
#if UNITY_TOLUA
            if (_OnHandlerFun != null)
            {
                _OnHandlerFun.BeginPCall();
                _OnHandlerFun.Push(_table);
                _OnHandlerFun.Push(value);
                _OnHandlerFun.Push(str);
                _OnHandlerFun.PCall();
                _OnHandlerFun.EndPCall();
            }
#endif
        }

        private static void OnWarningBackGroundSize(int type,long size)
        {
#if UNITY_TOLUA
            if (_OnWarningBackGroundSizeFun != null)
            {
                _OnWarningBackGroundSizeFun.BeginPCall();
                _OnWarningBackGroundSizeFun.Push(_table);
                _OnWarningBackGroundSizeFun.Push(type);
                _OnWarningBackGroundSizeFun.Push(size);
                _OnWarningBackGroundSizeFun.PCall();
                _OnWarningBackGroundSizeFun.EndPCall();
            }
#endif
        }
        
    }
}
