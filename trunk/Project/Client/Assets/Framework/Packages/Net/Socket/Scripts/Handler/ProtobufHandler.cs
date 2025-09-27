using protocol;

namespace Framework.Case.Net
{
    /// <summary>
    /// 代码动态添加 根据proto
    /// </summary>
    public class ProtobufHandler
    {
        public static ProtoBuf.IExtensible GetProtoData(ProtoDefine protoId, byte[] msgData)
        {
            switch (protoId)
            {
                //dynamic
                case ProtoDefine.IND_Broadcast:
                    return ProtoHandler.Deserialize<IND_Broadcast>(msgData);
                case ProtoDefine.COM_Build:
                    return ProtoHandler.Deserialize<COM_Build>(msgData);
                case ProtoDefine.RES_Connect:
                    return ProtoHandler.Deserialize<RES_Connect>(msgData);
                case ProtoDefine.RES_Login:
                    return ProtoHandler.Deserialize<RES_Login>(msgData);
                case ProtoDefine.REQ_Register:
                    return ProtoHandler.Deserialize<REQ_Register>(msgData);
                default:
                    return null;
            }
        }
    }
}
