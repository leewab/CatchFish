#if UNITY_TOLUA
using System.Collections.Generic;
using LuaInterface;

namespace MUGame{

    public class LuaMsgHandler
    {
		
        //public List<FetchTask> taskList = new List<FetchTask>();

        //public MemoryPool<FetchTask> FetchPool = new MemoryPool<FetchTask>(50);
        
        
        private Dictionary<uint, List<LuaFunction>> eventDic = new Dictionary<uint, List<LuaFunction>>();

        public bool isActive = true;

        public LuaFunction OnGetSocketBytes = null;

        static LuaMsgHandler _instance;

        public static LuaMsgHandler GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LuaMsgHandler();
                //Events.Common.SendEventToLua += _instance.SendEvent;
            }
            return _instance;
        }

		public class EventTask
		{
			public string method = null;
			public EventTask()
			{

			}

			public EventTask(string method)
			{
				this.method = method;
			}
		}

		public class FetchTask
		{
			public uint eventId = 0;
            public byte[] netData = null;
            public bool isNetString = false;
            public string Parameter = null;
		}

        LuaFunction _OnConnectFun = null;
        LuaFunction _OnDisConnectFun = null;
        LuaFunction _OnFailedConnectFun = null;
        LuaFunction _OnConnectResultFun = null;
        private LuaTable _table = null;
        public void Init(LuaTable table)
        {
            _table = table;
            _OnConnectFun = _table.GetLuaFunction("OnNormalConnect");
            _OnDisConnectFun = _table.GetLuaFunction("OnNormalDisConnect");
            _OnFailedConnectFun = _table.GetLuaFunction("OnNormalFailedConnect");
            _OnConnectResultFun = _table.GetLuaFunction("OnNormalConnectResult");

            _OnCrossConnectFun = _table.GetLuaFunction("OnCrossConnect");
            _OnCrossDisConnectFun = _table.GetLuaFunction("OnCrossDisConnect");
            _OnCrossFailedConnectFun = _table.GetLuaFunction("OnCrossFailedConnect");
            _OnCrossConnectResultFun = _table.GetLuaFunction("OnCrossConnectResult");
        }

        public void Connect(string ip, int port)
        {
            Functions.Net.Connect.SafeInvoke(ip, port);
        }

        public void Reconnect()
        {
            Functions.Net.Reconnect.SafeInvoke();
        }
        /**
         * 主动断开连接
         */
        public void DisConnect()
        {
            Functions.Net.Disconnect.SafeInvoke();
        }

        /**
         * 连接成功
         */
        public void OnConnect()
        {
            if (_OnConnectFun != null)
            {
                _OnConnectFun.BeginPCall();
                _OnConnectFun.Push(_table);
                _OnConnectFun.PCall();
                _OnConnectFun.EndPCall();
            }
        }
        /**
         * 连接断开
         */
        public void OnDisConnect()
        {
            if (_OnDisConnectFun != null)
            {
                _OnDisConnectFun.BeginPCall();
                _OnDisConnectFun.Push(_table);
                _OnDisConnectFun.PCall();
                _OnDisConnectFun.EndPCall();
            }
        }
        /**
         * 连接失败
         */
        public void OnFailedConnect()
        {
            if (_OnFailedConnectFun != null)
            {
                _OnFailedConnectFun.BeginPCall();
                _OnFailedConnectFun.Push(_table);
                _OnFailedConnectFun.PCall();
                _OnFailedConnectFun.EndPCall();
            }
        }

        public void OnConnectResult(int result)
        {
            if(_OnConnectResultFun != null)
            {
                _OnConnectResultFun.BeginPCall();
                _OnConnectResultFun.Push(_table);
                _OnConnectResultFun.Push(result);
                _OnConnectResultFun.PCall();
                _OnConnectResultFun.EndPCall();
            }
        }

        /// 
        /// ----------------------------
        /// 
        LuaFunction _OnCrossConnectFun = null;
        LuaFunction _OnCrossDisConnectFun = null;
        LuaFunction _OnCrossFailedConnectFun = null;
        LuaFunction _OnCrossConnectResultFun = null;
        public void CrossConnect(string ip, int port)
        {
            Functions.Net.CrossConnect.SafeInvoke(ip, port);
        }
        public void CrossReconnect()
        {
            Functions.Net.CrossReconnect.SafeInvoke();
        }
        /**
         * 主动断开连接
         */
        public void CrossDisConnect()
        {
            Functions.Net.CrossDisconnect.SafeInvoke();
        }

        public void OnCrossConnect()
        {
            if (_OnCrossConnectFun != null)
            {
                _OnCrossConnectFun.BeginPCall();
                _OnCrossConnectFun.Push(_table);
                _OnCrossConnectFun.PCall();
                _OnCrossConnectFun.EndPCall();
            }
        }

        public void OnCrossDisConnect()
        {
            if (_OnCrossDisConnectFun != null)
            {
                _OnCrossDisConnectFun.BeginPCall();
                _OnCrossDisConnectFun.Push(_table);
                _OnCrossDisConnectFun.PCall();
                _OnCrossDisConnectFun.EndPCall();
            }
        }

        /**
         * 跨服连接失败
         */
        public void OnCrossFailedConnect()
        {
            if (_OnCrossFailedConnectFun != null)
            {
                _OnCrossFailedConnectFun.BeginPCall();
                _OnCrossFailedConnectFun.Push(_table);
                _OnCrossFailedConnectFun.PCall();
                _OnCrossFailedConnectFun.EndPCall();
            }
        }

        public void OnCrossConnectResult(int result)
        {
            if (_OnCrossConnectResultFun != null)
            {
                _OnCrossConnectResultFun.BeginPCall();
                _OnCrossConnectResultFun.Push(_table);
                _OnCrossConnectResultFun.Push(result);
                _OnCrossConnectResultFun.PCall();
                _OnCrossConnectResultFun.EndPCall();
            }
        }
        
        public void CallMsg(uint msgid,  byte[] data )
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
                LuaClient.Instance.FetchMessageForNewProtoBuf(data, msgid, OnGetSocketBytes);
#endif
            }
            // else
            // {
            //     LuaClient.Instance.FetchMessage(data, msgid, OnGetSocketBytes);
            // }
        }

        public void SendMessage(uint eventId, byte[] netData,int len)
        {
            Functions.Net.SendById.SafeInvoke((int)eventId, netData,len);
        }

        public string GetMsgPbFilePath()
        {
            return "";
        }
       
    }
}
#endif