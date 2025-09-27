using ProtoBuf;

namespace Framework.Case.Net
{
    public class NetMessage
    {
        public ushort ProtoId;
        public int ProtoSize;
        public IExtensible ProtoData;

        public NetMessage()
        {
            
        }

        public NetMessage(ushort protoId, int protoSize, IExtensible protoData)
        {
            ProtoId = protoId;
            ProtoSize = protoSize;
            ProtoData = protoData;
        }

        public void Reset()
        {
            ProtoId = 0;
            ProtoSize = 0;
            ProtoData = null;
        }
    }
}