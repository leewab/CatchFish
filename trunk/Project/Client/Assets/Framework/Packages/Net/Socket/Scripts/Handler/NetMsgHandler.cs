using System;
using System.Collections.Generic;
using Framework.Core;
using ProtoBuf;
using protocol;
using protocol.ssb;

namespace Framework.Case.Net
{
    public delegate void NetMsgEvent(IExtensible msgData);
    
    public class NetMsgHandler
    {
        private static Dictionary<ProtoDefine, Delegate> netEventTable = new Dictionary<ProtoDefine, Delegate>();
        
        public static void AddListener(ProtoDefine protoType, NetMsgEvent msgEvent)
        {
            if (!netEventTable.ContainsKey(protoType))
            {
                netEventTable.Add(protoType, null);
            }

            netEventTable[protoType] = (NetMsgEvent) netEventTable[protoType] + msgEvent;
        }

        public static void RemoveListener(ProtoDefine protoType, NetMsgEvent msgEvent)
        {
            if (netEventTable.ContainsKey(protoType))
            {
                netEventTable[protoType] = (NetMsgEvent) netEventTable[protoType] - msgEvent;
                
                if (netEventTable[protoType] == null)
                {
                    netEventTable.Remove(protoType);
                }
            }
        }

        public static void DispatchMsg(NetMessage netMessage)
        {
            ProtoDefine protoType = (ProtoDefine) netMessage.ProtoId;
            Delegate eventD;
            if (netEventTable.TryGetValue(protoType, out eventD))
            {
                NetMsgEvent netMsgEvent = eventD as NetMsgEvent;
                netMsgEvent?.Invoke(netMessage.ProtoData);
            }
        }

        public static void SendMsg(ProtoDefine protoType, IExtensible protoData)
        {
            // GameController.Instance.SocketCtrl.SendMsg(protoType, protoData);
        }
    }
}