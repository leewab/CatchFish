using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using protocol;
using UnityEngine;

namespace Framework.Case.Net
{
    public static class ProtoHandler
    {
        
        private static Queue<NetMessage> netMsgQueue = new Queue<NetMessage>();

        public static NetMessage GetProtoMessage(ProtoDefine protoDefine, IExtensible protoData, ushort protoSize = 0)
        {
            NetMessage netMessage = null;
            if (netMsgQueue.Count > 0)
            {
                netMessage = netMsgQueue.Dequeue();
            }
            else
            {
                netMessage = new NetMessage();
            }

            netMessage.ProtoId = (ushort)protoDefine;
            netMessage.ProtoSize = protoSize;
            netMessage.ProtoData = protoData;

            return netMessage;
        }

        public static void RecycleProtoMessage(NetMessage netMessage)
        {
            netMessage.Reset();
            netMsgQueue.Enqueue(netMessage);
        }
        
        
        
        public static byte[] PackNetMsg(NetMessage data)
        {
            ushort protoId = data.ProtoId;
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                byte[] pbData = Serialize(data.ProtoData);
                int msgLen = pbData.Length;
                writer.Write(protoId);
                Debug.Log(protoId);
                writer.Write(msgLen);
                Debug.Log(msgLen);
                writer.Write(pbData);
                Debug.Log(pbData);
                writer.Flush();
                return ms.ToArray();
            }
        }

        public static NetMessage UnPackNetMsg(byte[] msgBytes)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream(msgBytes))
            {
                BinaryReader reader = new BinaryReader(ms);
                ushort protoId = reader.ReadUInt16();
                ushort msgLen = reader.ReadUInt16();
                Debug.LogError("protoId="+protoId);
                Debug.LogError("msgLen="+msgLen);
                Debug.LogError("msgBytes.Length="+msgBytes.Length);
                if (msgLen <= msgBytes.Length - 4)
                {
                    IExtensible protoData = ProtobufHandler.GetProtoData((ProtoDefine) protoId, reader.ReadBytes(msgLen));
                    return GetProtoMessage((ProtoDefine) protoId, protoData, msgLen);
                }
                else
                {
                    Debug.LogError("协议长度错误！");
                }
            }

            return null;
        }
        
        public static byte[] Serialize<T>(T msg)
        {
            byte[] result = null;
            if (msg != null)
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, msg);
                    result = stream.ToArray();
                }
            }

            return result;
        }

        public static T Deserialize<T>(byte[] message)
        {
            T result = default(T);
            if (message != null)
            {
                using(var stream = new MemoryStream(message))
                {
                    result = Serializer.Deserialize<T>(stream);
                }
            }

            return result;
        }
    }
}