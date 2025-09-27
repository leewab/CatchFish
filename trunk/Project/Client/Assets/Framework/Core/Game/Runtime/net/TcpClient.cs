using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Game.UI;
using MUGame;
using UnityEngine;

namespace Utils
{
    public abstract class msg_base : global::ProtoBuf.IExtensible
    {
        public virtual int _msgid() { return 0; }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }

    public struct SMsgData
    {
        public uint msgID;
        public byte[] msgData;
    }

    public sealed class TcpClient
    {
        private Socket _socket = null;
        private bool _ready = false;
        bool connectFailed = false;
        /// <summary>
        /// 调试模式，会在控制台打印发送和接收到的消息
        /// </summary>
        public static bool debugMode = false;

        private const int MAX_BUFF_SIZE = 256 * 1024;
        private const int HEAD_SIZE = 8;
        private byte[] _headBuffer = new byte[HEAD_SIZE];
        private byte[] _sendBuffer = new byte[64 * 1024];
        //private MemoryPoolSafe<Package> _packagePool = new MemoryPoolSafe<Package>();
        //private Package _currPackage = null;
        private System.IO.MemoryStream _sendStream = null;

        const int RECV_BUFFER_SIZE = 256 * 1024;
        private MemoryStream recvBuffer = new MemoryStream();
        private int lastMsgLength = -1;

        private byte[] socketAsyncBuffer = new byte[RECV_BUFFER_SIZE];
        private SocketAsyncEventArgs saeArgs;
        private object socketLockObj = new object();

        bool canPing;
        public bool CanPing
        {
            get { return canPing; }
            set
            {
                if (value)
                {
                    timeStamp = -1;
                    timeStampAck = -1;
                }
                canPing = value;
            }
        }

        public bool BeReady
        {
            get { return _ready; }
        }
        public TcpClient()
        {
            _sendStream = new System.IO.MemoryStream(_sendBuffer);
            //_recvStream = new System.IO.MemoryStream();
        }
        public string cIp { get; set; }
        public int nPort { get; set; }
        Socket socket;
        public void Connect(string ip, int port)
        {
            Close();
            cIp = ip;
            nPort = port;

            Socket socket;
            string newServerIp;
            AddressFamily type;
            Ipv6.getIPType(ip, port.ToString(), out newServerIp, out type);
            socket = new Socket(type, SocketType.Stream, ProtocolType.Tcp);
            Debug.Log("Socket Connect IP: "+ ip+ "  newServerIp  " + newServerIp);
//             Close();
//             socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//             socket.BeginConnect(ip, port, new AsyncCallback(onConnected), this);
            if(MsgHandler.ConnetSocketFirst)
            {
                socket.BeginConnect(newServerIp, port, new AsyncCallback(onConnected), this);
                _socket = socket;
            }
            _ready = false;
            _readKey = false;
            _popOnConnected = false;
            connectFailed = false;
            recvBuffer.SetLength(0);
            lastMsgLength = -1;
            if(!MsgHandler.ConnetSocketFirst)
            {
                _socket = socket;
                socket.BeginConnect(newServerIp, port, new AsyncCallback(onConnected), this);
            }
        }

        //在主线程调用此函数，解决OnConnect只能在主线程调用场景相关函数的问题
        private bool _popOnConnected = false; //是否已经派发OnConnect事件
        private void onConnectProc()
        {
            if (_popOnConnected)
                return;
            if ( (_ready && _socket.Connected)  && (!ENCRYPT || (ENCRYPT && _readKey )) )
            {
                _popOnConnected = true;
                if (OnConnect != null)
                    OnConnect();
            }
            else if (connectFailed)
            {
                connectFailed = false; 
                OnConnectFailed.SafeInvoke();
            }
        }

        private void onCloseProc()
        {
            if ((_ready || bNeedCloseInfo) && _socket == null)
            {
                _ready = false;
                _receivePackage.Clear();
                if (OnClose != null)
                    OnClose();
            }
            bNeedCloseInfo = false;
        }

        private float timeStamp = -1;
        private float timeStampAck = -1;
        bool needAck = false;
        float logStamp = -1;
        int msgCnt = 0;
        public void DealMessage()
        {
            if (_socket == null)
            {
                onCloseProc();
                return;
            }
            onConnectProc();
            while (true)
            {
                var message = popPackage();
                if (message.msgID == 0)
                    break;
                msgCnt++;

                MsgHandler.CallMsg(message.msgID, message.msgData);
            }
#if UNITY_EDITOR
            if (UnityEngine.Time.realtimeSinceStartup - logStamp > 5)
            {
                logStamp = UnityEngine.Time.realtimeSinceStartup;
                msgCnt = 0;
            }
#endif
        }
        private ThreadSafeQueue<SMsgData> _receivePackage = new ThreadSafeQueue<SMsgData>();
        private void pushPackage(SMsgData msg)
        {
            _receivePackage.Enqueue(msg);
        }

        void ReceiveLoop(object state)
        {
            while (_ready)
            {
                if (!DoSyncRecv())
                    return;
            }
        }

        private bool DoSyncRecv()
        {
            try
            {
                if (_socket == null || _socket.Connected == false)
                    return false;
                int received;
                received = _socket.Receive(socketAsyncBuffer);
                if (received > 0)
                {
                    ReceivePayload(socketAsyncBuffer, received);
                }
                else
                {
                    Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                //D.error(ex.ToString());
                Close();
                return false;
            }
            return true;
        }
        private void AsyncRecv_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                try
                {
                    ReceivePayload(e.Buffer, e.BytesTransferred);
                }
                catch (Exception ex)
                {
                    D.error(ex.ToString());
                    Close();
                    return;
                }
            }
            else
            {
                D.log("socket closed");
                Close();
                return;
            }

            try
            {
                //继续接受数据
                if (!_socket.ReceiveAsync(saeArgs))
                {
                    AsyncRecv_Completed(null, saeArgs);
                }
            }
            catch (Exception ex)
            {
                Close();
                throw ex;
            }
        }
        private int readInt32FromNetwork(BinaryReader br)
        {
            //int value = br.ReadInt32();

            byte[] buff = br.ReadBytes(4);
            return getBufferInt(buff);
            
            //return networkToHost(value);
            //return IPAddress.NetworkToHostOrder(value);
        }
        private void ReceivePayload(byte[] data, int length)
        {
            if (_socket == null)
                return;
            if (!_socket.Connected)
            {
                Close();
                return;
            }
            //接受数据并拼接成message
            byte[] msgBuff;
            //写入缓存
            recvBuffer.Position = recvBuffer.Length;
            recvBuffer.Write(data, 0, length);
            //如果长度有错，返回
            if (lastMsgLength < 0 && recvBuffer.Length < 4)
            {
                msgBuff = null;
                return;
            }

            recvBuffer.Position = 0; 
            BinaryReader br = new BinaryReader(recvBuffer);
            //读取消息长度
            if (lastMsgLength < 0)
            {
                lastMsgLength = readInt32FromNetwork(br) - 4;
                if (lastMsgLength > MAX_BUFF_SIZE)
                {
                    Close();
                    int type = readInt32FromNetwork(br);
                    //UnityEngine.Debug.LogError(type);
                    throw new Exception("Too long package length!");
                }
            }
            int remaining = (int)(recvBuffer.Length - recvBuffer.Position);
            //消息已经完整
            while (remaining >= lastMsgLength && lastMsgLength > 0)
            {
                if (ENCRYPT && !_readKey)
                {
                    byte[] key = br.ReadBytes(lastMsgLength);
                    _rc4 = new RC4(key);
                    _readKey = true;

                }
                else
                {
                    //读取一条消息
                    int type = readInt32FromNetwork(br);
                    msgBuff = br.ReadBytes(lastMsgLength - 4);
                    int dataLen = lastMsgLength - 4;

                    //异步

                    if (debugMode)
                    {
                        D.log(string.Format("<color=green> Recive --->{0}</color>", type));
                    }
                    SMsgData datap = new SMsgData();
                    datap.msgID = (uint)type;
                    datap.msgData = msgBuff;
                    pushPackage(datap);
                }
 
                lastMsgLength = -1;
                remaining = (int)(recvBuffer.Length - recvBuffer.Position);
                //保留剩余数据
                if (remaining >= 4)
                {
                    lastMsgLength = readInt32FromNetwork(br) - 4;
                    remaining -= 4;
                    if (lastMsgLength > MAX_BUFF_SIZE)
                    {
                        Close();
                        throw new Exception("Too long package length!");
                    }
                }
            }

            remaining = (int)(recvBuffer.Length - recvBuffer.Position); 
            if (remaining > 0)
            {
                byte[] buffer = recvBuffer.GetBuffer();
                Array.Copy(buffer, recvBuffer.Position, buffer, 0, remaining);
            }
            recvBuffer.Position = 0;
            recvBuffer.SetLength(remaining);
        }
        private SMsgData popPackage()
        {
            SMsgData msg;
            msg.msgID = 0;
            _receivePackage.Dequeue(out msg);
            return msg;
        }

        public Action OnConnect { get; set; }

        public Action OnConnectFailed { get; set; }
        public Action OnClose { get; set; }
        private void onConnected(IAsyncResult result)
        {
            if(GameConfig.TryEndConnect)
            {
                //UnityEngine.Debug.Log("tcpclient onConnected------1");
                try
                {
                    _socket.EndConnect(result);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
            }
           
            if (_socket.Connected)
            {
                UnityEngine.Debug.Log("tcpclient onConnected------2");
                if (GameConfig.TryEndConnect == false)
                {
                    _socket.EndConnect(result);
                }
                /*saeArgs = new SocketAsyncEventArgs();
                saeArgs.Completed += AsyncRecv_Completed;
                saeArgs.SetBuffer(socketAsyncBuffer, 0, socketAsyncBuffer.Length);
                _socket.ReceiveAsync(saeArgs);*/
                _ready = true;
                System.Threading.ThreadPool.QueueUserWorkItem(ReceiveLoop, null);
                //ReceiveOnce();
            }
            else
            {
                UnityEngine.Debug.Log("tcpclient onConnected------3");
                connectFailed = true;
            }
        }

        //len type msg
        public void Send(msg_base msg)
        {
            if (!_ready)
                return;
            if (debugMode)
            {
                if (msg._msgid() != 1116)
                D.log(string.Format("<color = red> Send <---{0}</color>", msg));
            }
                
            
            //timeStamp = UnityEngine.Time.realtimeSinceStartup;
            _sendStream.Seek(HEAD_SIZE, System.IO.SeekOrigin.Begin);
            ProtoBuf.Serializer.Serialize(_sendStream, msg);
            
            int len = (int)_sendStream.Position;

            setBufferInt(_sendHeaderBuffer, len, 0);
            setBufferInt(_sendHeaderBuffer, msg._msgid(), 4);
            _sendStream.Seek(0, System.IO.SeekOrigin.Begin);
            _sendStream.Write(_sendHeaderBuffer, 0, HEAD_SIZE);

            rawSend(_socket, _sendBuffer, 4, len);
            //_socket.Send(_sendBuffer, len, SocketFlags.None);
        }

        public void Send(int msgId, byte[] data,int dataSize)
        {
            if (!_ready)
                return;
            if (debugMode)
            {
                if (msgId != 1116)
                    D.log(string.Format("<color=red> Send <---{0}</color>", msgId));
            }


            //timeStamp = UnityEngine.Time.realtimeSinceStartup;
            _sendStream.Seek(HEAD_SIZE, System.IO.SeekOrigin.Begin);

            //_sendStream.Write(data, 0, data.Length);
            _sendStream.Write(data, 0, dataSize);

            int len = (int)_sendStream.Position;

            setBufferInt(_sendHeaderBuffer, len, 0);
            setBufferInt(_sendHeaderBuffer, msgId, 4);
            _sendStream.Seek(0, System.IO.SeekOrigin.Begin);
            _sendStream.Write(_sendHeaderBuffer, 0, HEAD_SIZE);

            rawSend(_socket, _sendBuffer, 4, len);
            //_socket.Send(_sendBuffer, len, SocketFlags.None);
        }

/*
        public void Send(int type, byte[] buf, int len = -1)
        {
            if (!_ready)
                return;
            if (len < 0)
                len = buf.Length;
            int msgLen = HEAD_SIZE + len;
            sendHeader(msgLen, type);
            rawSend(_socket, buf, 0, len);
            //_socket.Send(buf,len, SocketFlags.None);
        }*/
        private byte[] _sendHeaderBuffer = new byte[HEAD_SIZE];
        /*private void sendHeader(int len, int type)
        {
            setBufferInt(_sendHeaderBuffer, len, 0);
            setBufferInt(_sendHeaderBuffer, type, 4);
            rawSend(_socket, _sendHeaderBuffer, 4, -1);
            //_socket.Send(_sendHeaderBuffer);
        }*/
        private const bool ENCRYPT = true;
        private volatile bool _readKey = false;
        private RC4 _rc4 = null;
        private void rawSend(Socket sock, byte[] buf, int start, int end)
        {
            if(sock == null)
                return;
            if (end < 0)
                end = buf.Length;
            #region RC4
            if (ENCRYPT && _rc4 != null)
            {
                _rc4.Crypt(buf, start, end);
            }
            #endregion
            try
            {
                sock.Send(buf, end, SocketFlags.None);
            }
            catch (Exception ex)
            {
                D.error(ex.ToString());
            }
        }
        private void beginSend()
        {

        }
        private void endAndCommitSend()
        {

        }
        private void setBufferInt(byte[] buf, int value, int offset)
        {
            int pos = offset;
            for (int i = 0; i < 4; i++)
            {
                buf[pos++] = (byte)((value >> (24 - 8 * i)) & 0xff);
            }
        }
        private int getBufferInt(byte[] buf)
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                int value = buf[i];
                result += value << (24 - 8 * i);
            }
            return result;
        }

        private bool bNeedCloseInfo = false;
        public void Close()
        {
            if (_socket == null || !_ready)
                return;

            _ready = false;
            bNeedCloseInfo = true;
            //先设置false，关闭线程池
            if (saeArgs != null)
                saeArgs.Dispose();
            try
            {
                if (_socket.Connected)
                    _socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {

            }
            

            _socket.Close();
            _socket = null;

        }
    }
}
