using protocol;
using protocol.ssb;

namespace Framework.Case.Net
{
    public class NetEventHandler
    {
        public static void RegisterNetEvent()
        {
            NetMsgHandler.AddListener(ProtoDefine.RES_Connect, TokenHandler.Instance.ResVerifyToken);
        }

        public static void UnRegisterNetEvent()
        {
            NetMsgHandler.RemoveListener(ProtoDefine.RES_Connect, TokenHandler.Instance.ResVerifyToken);
        }
    }
}