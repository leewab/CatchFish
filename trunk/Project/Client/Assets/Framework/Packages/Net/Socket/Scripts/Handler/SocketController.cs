using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Framework.Core;
using ProtoBuf;
using protocol;
using UnityEngine;

namespace Framework.Case.Net
{
    public class SocketController : MonoSingleton<SocketController>
    {
        private volatile bool mIsRunning = false;
        private System.Net.Sockets.Socket mSocket;

        //Send
        private Thread mSendThread;
        private object mSendLock = null;
        private Queue<NetMessage> mSendingNetMsgQueue = null;
        private Queue<NetMessage> mSendWaitingNetMsgQueue = null;
        
        //Receive
        private Thread mRecvThread;
        private object mRecvLock = null;
        private Queue<NetMessage> mRecvingNetMsgQueue = null;
        private Queue<NetMessage> mRecvWaitingNetMsgQueue = null;
        
        private void OnEnable()
        {
            NetEventHandler.RegisterNetEvent();
        }
        
        private void OnDisable()
        {
            DisConnect();
            mIsRunning = false;
            NetEventHandler.UnRegisterNetEvent();
        }
        
        private void Start()
        {
            Connect(NetDefine.IPAddress, NetDefine.Port);
        }

        private void Update()
        {
            Dispatcher();
        }

        private void Connect(string host, int port)
        {
            if (mIsRunning) return;
            
            if (string.IsNullOrEmpty(host))
            {
                Debug.LogError("Connect Func 'host' is null");
                return;
            }

            IPEndPoint ipEndPoint = null;
            Regex regex = new Regex("((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|\\d)\\.){3}(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|[1-9])");
            Match match = regex.Match(host);
            if (match.Success)
            {
                //IP
                ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            }
            else
            {
                //域名
                IPAddress[] addresses = Dns.GetHostAddresses(host);
                ipEndPoint = new IPEndPoint(addresses[0], port);
            }
            
            //建立Socket TCP 连接
            mSocket = new System.Net.Sockets.Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                mSocket.Connect(ipEndPoint);
                mIsRunning = true;
                InitNetMessageQueue();
                InitNetMessageThread();
                //请求握手
                TokenHandler.Instance.ReqVerifyToken();
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                DisConnect();
            }
        }

        /// <summary>
        /// 初始化消息队列
        /// </summary>
        private void InitNetMessageQueue()
        {
            mSendLock = new object();
            mSendingNetMsgQueue = new Queue<NetMessage>();
            mSendWaitingNetMsgQueue = new Queue<NetMessage>();
            
            mRecvLock = new object();
            mRecvingNetMsgQueue = new Queue<NetMessage>();
            mRecvWaitingNetMsgQueue = new Queue<NetMessage>();
        }

        /// <summary>
        /// 初始化消息线程（收、发）
        /// </summary>
        private void InitNetMessageThread()
        {
            mSendThread = new Thread(Send);
            mRecvThread = new Thread(Receive);
            mSendThread.Start();
            mRecvThread.Start();
        }
        
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="protoType"></param>
        /// <param name="protoData"></param>
        public void SendMsg(ProtoDefine protoType, IExtensible protoData)
        {
            if (!this.mIsRunning) return;
            lock (this.mSendLock)
            {
                mSendWaitingNetMsgQueue.Enqueue(ProtoHandler.GetProtoMessage(protoType, protoData));
                Debug.LogError(mSendWaitingNetMsgQueue.Count);
                Monitor.Pulse(this.mSendLock);
            }
        }

        private void Send()
        {
            while (mIsRunning)
            {
                if (mSendingNetMsgQueue.Count == 0)
                {
                    lock (mSendLock)
                    {
                        while (mSendWaitingNetMsgQueue.Count == 0)
                        {
                            Monitor.Wait(mSendLock);
                        }

                        Queue<NetMessage> temp = mSendingNetMsgQueue;
                        mSendingNetMsgQueue = mSendWaitingNetMsgQueue;
                        mSendWaitingNetMsgQueue = temp;
                    }
                }
                else
                {
                    try
                    {
                        NetMessage netMessage = mSendingNetMsgQueue.Dequeue();
                        byte[] netMsg = ProtoHandler.PackNetMsg(netMessage);
                        mSocket.Send(netMsg, netMsg.Length, SocketFlags.None);
                        Debug.Log("Client send : " + (ProtoDefine) netMessage.ProtoId);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        DisConnect();
                    }
                }
            }
            
            mSendingNetMsgQueue.Clear();
            mSendWaitingNetMsgQueue.Clear();
        }

        private void Receive()
        {
            byte[] data = new byte[1024];
            while (mIsRunning)
            {
                try
                {
                    int len = mSocket.Receive(data);
                    Debug.LogError(len);
                    NetMessage netMessage = ProtoHandler.UnPackNetMsg(data);
                    Debug.Log("Client receive : " + (ProtoDefine) netMessage.ProtoId);
                    lock (mRecvLock)
                    {
                        mRecvWaitingNetMsgQueue.Enqueue(netMessage);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    DisConnect();
                }
            }
        }

        private void Dispatcher()
        {
            if (!mIsRunning) return;
            if (mRecvingNetMsgQueue.Count == 0)
            {
                lock (mRecvLock)
                {
                    if (mRecvWaitingNetMsgQueue.Count > 0)
                    {
                        Queue<NetMessage> temp = mRecvingNetMsgQueue;
                        mRecvingNetMsgQueue = mRecvWaitingNetMsgQueue;
                        mRecvWaitingNetMsgQueue = temp;
                    }
                }
            }
            else
            {
                while (mRecvingNetMsgQueue.Count > 0)
                {
                    NetMessage netMessage = mRecvingNetMsgQueue.Dequeue();
                    NetMsgHandler.DispatchMsg(netMessage);
                }
            }
        }

        private void DisConnect()
        {
            if (!mIsRunning) return;
            try
            {
                if (mSocket.Available != 0)
                {
                    mSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                mIsRunning = false;
                mSocket.Close();
            }
        }
    }
}