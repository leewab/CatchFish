using System;
using System.Collections.Generic;
using Game;
using Game.Core;
using Utils;

namespace MUGame
{
    public class NetHandler : BaseHandler
    {
        /// <summary>
        /// 单例注册
        /// </summary>
        public static NetHandler Instance => HandlerModule.NetHandler;

        // Use this for initialization
        private TcpClient _client = new TcpClient();
        private TcpClient _crossClient = new TcpClient();
        Dictionary<int, DateTime> sendTimeMapping = new Dictionary<int, DateTime>();
        HashSet<int> sendLimitExclude = new HashSet<int>();
        string kickReason = null;

        //socke连接成功标志
        private bool connected = false;
        //是否客户端主动断线标志
        private bool disconnected = true;
        //socket连接中标志
        private bool connecting = false;
        //每次socket断线连接的时间（断线重连每5秒重置）
        private DateTime reconnectTime;
        //socket连接初始时间
        private DateTime connectInitTime;
        //socket断开连接时间（客户端主动断开不记录）
        private DateTime disconnectTime;

        private bool _bNeedCross = false;
        //每次socket断线连接的时间（断线重连每5秒重置）
        private DateTime crossReconnectTime;
        //是否客户端主动断线标志
        private bool crossDisconnected = true;

        public TcpClient TcpClient { get { return _client; } }

        public override void Init()
        {
            base.Init();
            //sendLimitExclude.Add(MsgDef.CSFightAtk);
            //sendLimitExclude.Add(MsgDef.CSItemEquipPutOn);
            //sendLimitExclude.Add(MsgDef.CSInstanceAll);//no CSInstanceAll funben
            //sendLimitExclude.Add(MsgDef.CSPickUpDropObject);
            registerEvents(true);
        }
        private void registerEvents(bool enable)
        {
            if (enable)
            {
                //_client.DealMessageFunc += msg.MsgHandler.Process;
                _client.OnConnect = OnConnect;
                _client.OnClose = OnClose;
                _client.OnConnectFailed = OnConnectFailed;

                _crossClient.OnConnect = OnCrossConnect;
                _crossClient.OnClose = OnCrossClose;
                _crossClient.OnConnectFailed = OnCrossConnectFailed;

                Functions.Net.Connect += Connect;
                Functions.Net.Reconnect += AutoReconnect;
                Functions.Net.Disconnect += Disconnect;      
                Functions.Net.CrossConnect += CrossConnect;
                Functions.Net.CrossReconnect += AutoCrossReconnect;
                Functions.Net.CrossDisconnect += CrossDisconnect;

                Functions.Net.Send += send;
                Functions.Net.SendById += sendByID;

                //MsgHandler.Reg(MsgDef.SCInitData, OnSCInitData);
                //MsgHandler.Reg(MsgDef.SCHumanKick, OnSCHumanKick);
            }
            else
            {
                _client.OnConnect = null;
                _client.OnClose = null;
                _client.OnConnectFailed = null;
                _crossClient.OnConnect = null;
                _crossClient.OnClose = null;
                _crossClient.OnConnectFailed = null;
                Functions.Net.Connect -= Connect;
                Functions.Net.Reconnect -= AutoReconnect;
                Functions.Net.Disconnect -= Disconnect;
                Functions.Net.CrossConnect -= CrossConnect;
                Functions.Net.CrossReconnect -= AutoCrossReconnect;
                Functions.Net.CrossDisconnect -= CrossDisconnect;
                Functions.Net.Send -= send;
                Functions.Net.SendById -= sendByID;

                //MsgHandler.UnReg(MsgDef.SCInitData, OnSCInitData);
                //MsgHandler.UnReg(MsgDef.SCHumanKick, OnSCHumanKick);
            }
        }

        /**
         * 开始连接socket
         */
        private void Connect(string ip, int port)
        {
            //D.log("Connect");
            //中断之前重连
            //TimerHandler.RemoveTimeaction(_reconnect);

            UnityEngine.Debug.Log("NetMod-->Connect " + ip + ":"+ port);
            //D.log(string.Format("connect ip={0},port={1}", ip, port));
            kickReason = null;
            GameConfig.serverIP = ip;
            GameConfig.serverPort = port;
            _client.Connect(ip, port);
            if (!connecting)
            {
                connecting = true;
                connectInitTime = DateTime.Now;
            }

        }
        //外部调用，主动掉线
        private void Disconnect()
        {
            UnityEngine.Debug.Log("NetMod-->Disconnect ");
            //主动掉线不进入重连流程
            disconnected = true;
            _disconnect();
        }
        //外部调用，自动重连
        private void AutoReconnect()
        {
            UnityEngine.Debug.Log("NetMod-->AutoReconnect");
            if (connected)
            {
                //连接中，直接断线，模拟服务器掉线，进入重连流程，
                _disconnect();
            }
            else
            {
                disconnected = false;
                _disconnectToReconnect();
            }
        }
        private void _reconnect()
        {
            reconnectTime = DateTime.Now;
            UnityEngine.Debug.Log("NetMod-->_reconnect:"+reconnectTime);
            Connect(GameConfig.serverIP, GameConfig.serverPort);
        }

        private void _disconnect()
        {
            UnityEngine.Debug.Log("NetMod-->_disconnect");
            if (connected)
            {
                UnityEngine.Debug.Log("NetMod-->_disconnect 1");
                _client.Close();
                connected = false;
            }
        }


        private void _disconnectToReconnect()
        {
            UnityEngine.Debug.Log("NetMod-->_disconnectToReconnect");
            disconnectTime = DateTime.Now;
            MsgHandler.OnConnectResult(2);//断线        
            _reconnect();
        }


        //TcpClient连接成功回调
        private void OnConnect()
        {
            UnityEngine.Debug.Log("NetMod-->OnConnect");
            connecting = false;
            connected = true;
            //通知lua层socket连接成功
            MsgHandler.OnConnect();

            if (disconnected == false)
            {
                //重连成功
                MsgHandler.OnConnectResult(3);
            }
            else
            {
                disconnected = false;
            }
        }

        //TcpClient连接断开回调
        private void OnClose()
        {
            UnityEngine.Debug.Log("NetMod-->OnClose");
            connected = false;
            //通知lua层socket连接断开
            MsgHandler.OnDisConnect();

            if (_client != null)
                _client.CanPing = false;

            if (disconnected == false)
            {
                //非主动断线，自动重连
                _disconnectToReconnect();
            }
        }

        //TcpClient连接失败回调
        void OnConnectFailed()
        {
            UnityEngine.Debug.Log("NetMod-->OnConnectFailed");
            if (disconnected == false)
            {
                if ((DateTime.Now - disconnectTime).TotalSeconds >= MsgHandler.ReconnectMaxTime)
                {
                    UnityEngine.Debug.Log("NetMod-->OnConnectFailed---断线重连超时");
                    MsgHandler.OnConnectResult(0);//断线重连900秒失败
                    disconnected = true;
                    return;
                }
                MsgHandler.OnFailedConnect();
                //TimerHandler.SetTimeout(_reconnect, 5, false, false);
            }
            else if (connecting)
            {
                //if ((DateTime.Now - connectInitTime).TotalSeconds >= 30)
                //{
                UnityEngine.Debug.Log("NetMod-->OnConnectFailed--连接服务器失败");
                MsgHandler.OnConnectResult(1);//连接服务器30秒失败
                    connecting = false;
                 //   return;
                //}
            }
        }



        public void CrossConnect(string ip, int port)
        {
            UnityEngine.Debug.Log("NetMod-->CrossConnect " + ip + ":" + port);
            _crossClient.Connect(ip, port);
            _bNeedCross = true;
        }

        public void CrossDisconnect()
        {
            UnityEngine.Debug.Log("NetMod-->CrossDisconnect");
            _bNeedCross = false;
            crossDisconnected = true;
            _crossClient.Close();
        }

        //外部调用，自动重连
        private void AutoCrossReconnect()
        {
            UnityEngine.Debug.Log("NetMod-->AutoCrossReconnect");
            if (_crossClient.BeReady)
            {
                //连接中，直接断线，模拟服务器掉线，进入重连流程，
                _crossClient.Close();
            }
            else
            {
                crossDisconnected = false;
                _crossReconnect();
            }
        }

        private void OnCrossConnect()
        {
            UnityEngine.Debug.Log("NetMod-->OnCrossConnect");
            MsgHandler.OnCrossConnect();
            if (crossDisconnected == false)
            {
                //跨服重连成功
                MsgHandler.OnCrossConnectResult(3);
            }
            else
            {
                crossDisconnected = false;
            }
        }
        private void OnCrossClose()
        {
            UnityEngine.Debug.Log("NetMod-->OnCrossClose");
            MsgHandler.OnCrossDisConnect();
            if (crossDisconnected == false)
            {
                MsgHandler.OnCrossConnectResult(2);//跨服断线
                _crossReconnect();
            }
        }

        private void OnCrossConnectFailed()
        {
            UnityEngine.Debug.Log("NetMod-->OnCrossConnectFailed");
            MsgHandler.OnCrossFailedConnect();

            //连接跨服失败
            if(crossDisconnected == false)
            {
                //断线重连失败
            }
            else if(_bNeedCross)
            {
                UnityEngine.Debug.Log("NetMod-->OnCrossConnectFailed--连接服务器失败");
                MsgHandler.OnCrossConnectResult(1);//连接服务器失败，自动重连
            }
        }
        private void _crossReconnect()
        {
            crossReconnectTime = DateTime.Now;
            UnityEngine.Debug.Log("NetMod-->_crossReconnect:" + crossReconnectTime);
            _crossClient.Connect(_crossClient.cIp, _crossClient.nPort);
        }

        public override void Update()
        {
            //加载地图时暂停消息处理
            //if (SceneMod.IsLoading)
            //    return;
            _client.DealMessage();
            _crossClient.DealMessage();

            if( false == _bNeedCross && connecting && disconnected == false)
            {
                //D.log("当前时间："+DateTime.Now);
                //D.log("重连时间：" + reconnectTime);
                if ((DateTime.Now - reconnectTime).TotalSeconds >= 3)
                {
                    UnityEngine.Debug.Log("Connection failed.....reconnectTime > 3s");
                    _reconnect();
                }
            }
            if(_bNeedCross && !_crossClient.BeReady && crossDisconnected == false)
            {
                if ((DateTime.Now - crossReconnectTime).TotalSeconds >= 3)
                {
                    UnityEngine.Debug.Log("Connection failed.....crossReconnectTime > 3s");
                    _crossReconnect();
                }
            }

        }
        public override void Dispose()
        {
            registerEvents(false);
            _bNeedCross = false;
            _client.Close();
            _crossClient.Close();
        }

        void OnSCInitData(object obj)
        {
            if (_client != null)
                _client.CanPing = true;
            if (_crossClient != null)
                _crossClient.CanPing = true;
        }

        //void OnSCHumanKick(object obj)
        //{
        //    SCHumanKick msg = obj as SCHumanKick;
        //    kickReason = msg.reason;
        //}
        //ConfCrossProtocol _CrossProtocol = null;
        private void send(Utils.msg_base msg)
        {
            
            //int msgID = msg._msgid();
            //if ((SceneMod.CurStageType == StageType.MainCity && !sendLimitExclude.Contains(msgID)))
            //{
            //    DateTime now = DateTime.Now;
            //    DateTime lastSent = now;
            //    if (!sendTimeMapping.TryGetValue(msgID, out lastSent))
            //    {
            //        lastSent = DateTime.MinValue;
            //    }
            //    sendTimeMapping[msgID] = now;

            //}
            if (_crossClient.BeReady)
            {
                _crossClient.Send(msg);
                //if (ConfCrossProtocol.GetConfig(msg._msgid(), out _CrossProtocol))
                    //_client.Send(msg);
            }
            else
            {
                _client.Send(msg);
            }
        }

        private void sendByID(int msgId, byte[] data,int len)
        {
            //int msgID = msgId;
            //if ((SceneMod.CurStageType == StageType.MainCity && !sendLimitExclude.Contains(msgID)))
            //{
            //    DateTime now = DateTime.Now;
            //    DateTime lastSent = now;
            //    if (!sendTimeMapping.TryGetValue(msgID, out lastSent))
            //    {
            //        lastSent = DateTime.MinValue;
            //    }
            //    //同样的消息避免重复发送，需要测试会不会有问题
            //    //if ((now - lastSent).TotalMilliseconds < 100)
            //    //    return;
            //    sendTimeMapping[msgID] = now;

            //}
            if (_crossClient.BeReady)
            {
                _crossClient.Send(msgId, data,len);
                //if (ConfCrossProtocol.GetConfig(msgID, out _CrossProtocol))
                    //_client.Send(msgId, data);
            }
            else
            {
                _client.Send(msgId, data,len);
            }
        }
        
        bool alertShown = false;
        private void returnToLogin(string kickReason = null)
        {
            //alertShown = true;
            //Alert.Hide(UIDef.UI_ALERT_RECONNECT);
            //if (string.IsNullOrEmpty(kickReason))
            //{
            //    TimerHandler.SetTimeout(() =>
            //    {
            //        Alert.Show("无法连接到服务器, 是否重新登录？", onReturnConfirm, false, UIDef.UI_ALERT_RECONNECT);
            //    }, 0.6f, false, false);
            //}
            //else
            //{
            //    TimerHandler.SetTimeout(() =>
            //    {
            //        Alert.Show(kickReason + "\n是否重新登录？", onReturnConfirm, false, UIDef.UI_ALERT_RECONNECT);
            //    }, 0.6f, false, false);
            //}
        }

        private void onReturnConfirm()
        {
            _client.Close();
            _crossClient.Close();
            _bNeedCross = false;
            alertShown = false;
            Functions.Net.ReturnToLogin.SafeInvoke();
        }
    }
}
