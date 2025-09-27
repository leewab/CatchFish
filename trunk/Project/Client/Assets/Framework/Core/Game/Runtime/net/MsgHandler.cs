using System;
using System.Collections.Generic;

namespace Utils
{
	public static class MsgHandler
	{
        //断线重连持续的最大时间
        public static int ReconnectMaxTime = 30;
        
        private static bool _socketFirst = false;
        public static bool ConnetSocketFirst
        {
            set { _socketFirst = value; }
            get { return _socketFirst; }
        }
        
        public static Dictionary<int, Action<object>> _dict = new Dictionary<int, Action<object>>();
        public static void Reg(int msgid, Action<object> func){
            Action<object> action = null;
            if (!_dict.TryGetValue(msgid, out action))
                _dict[msgid] = func;
            else
                _dict[msgid] += func;
        }
        public static void UnReg(int msgid, Action<object> func)
        {
            Action<object> action = null;
            if (_dict.TryGetValue(msgid, out action))
                _dict[msgid] -= func;
        }
        public static void Call(int msgid, object msg)
        {
            //LuaClient.Instance.FetchMessage
            //Action<object> action = null;
            //if (!_dict.TryGetValue(msgid, out action))
            //    return;
            //if (action == null)
            //    return;
            //action(msg);
        }
        
        public static void OnConnectResult(int result)
        {
#if UNITY_TOLUA
            LuaMsgHandler.GetInstance().OnConnectResult(result);
#endif
        }
        
        public static void OnConnect()
        {
#if UNITY_TOLUA
            LuaMsgHandler.GetInstance().OnConnect();
#endif
        }
        
        /**
         * 连接断开
         */
        public static void OnDisConnect()
        {
#if UNITY_TOLUA
            LuaMsgHandler.GetInstance().OnDisConnect();
#endif
        }
        
        /**
         * 连接失败
         */
        public static void OnFailedConnect()
        {
#if UNITY_TOLUA
            LuaMsgHandler.GetInstance().OnFailedConnect();
#endif
        }
        
        public static void OnCrossConnect()
        {
#if UNITY_TOLUA
            LuaMsgHandler.GetInstance().OnCrossConnect();
#endif
        }
        
        public static void OnCrossConnectResult(int result)
        {
#if UNITY_TOLUA
            LuaMsgHandler.GetInstance().OnCrossConnectResult(result);
#endif
        }
        
        public static void OnCrossDisConnect()
        {
#if UNITY_TOLUA
            LuaMsgHandler.GetInstance().OnCrossDisConnect();
#endif
        }
        
        /**
         * 跨服连接失败
         */
        public static void OnCrossFailedConnect()
        {
#if UNITY_TOLUA
            LuaMsgHandler.GetInstance().OnCrossFailedConnect();
#endif
        }
        
        public static void CallMsg(uint msgid,  byte[] data)
        {
            // if (GameConfig.UseNewProtoBuf)
            {
#if DEVELOPMENT_BUILD
                if (msgid == 1804)
                {
                    Profiler.BeginSample("SCBuffDispel");
                }
                else
                if (msgid == 1240)
                {
                    Profiler.BeginSample("SCStageObjectInfoChange");
                }
                else
                if (msgid == 1216)
                {
                    Profiler.BeginSample("SCStageObjectAppear");
                }
                else
                if (msgid == 1403)
                {
                    Profiler.BeginSample("SCUpdateHp");
                }
                else
                if (msgid == 1303)
                {
                    Profiler.BeginSample("SCFightSkill");
                }
                else
                if (msgid == 1217)
                {
                    Profiler.BeginSample("SCStageObjectDisappear");
                }
                else
                if (msgid == 1802)
                {
                    Profiler.BeginSample("SCBuffAdd");
                }
                else
                if (msgid == 1304)
                {
                    Profiler.BeginSample("SCFightHpChange");
                }
                else
                if (msgid == 1227)
                {
                    Profiler.BeginSample("SCUnitobjStatusChange");
                }
                else
                {
                    Profiler.BeginSample(msgid.ToString());
                }

                LuaClient.Instance.FetchMessageForNewProtoBuf(data, msgid, OnGetSocketBytes);
                Profiler.EndSample();
#else
#if UNITY_TOLUA
                LuaMsgHandler.GetInstance().CallMsg(msgid, data);
#else
                // TODO: 这里修改了 发送协议需要C#接口 看看Lua怎么发送的
                
#endif
#endif
            }
            // else
            // {
            //     LuaClient.Instance.FetchMessage(data, msgid, OnGetSocketBytes);
            // }
        }
        
	}
}
