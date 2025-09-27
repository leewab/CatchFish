namespace Framework.PM
{
    /// <summary>
    /// 协议结构
    /// </summary>
    public class ProtocolInfo
    {
        //Id 不可重复，避免重复消息
        public string Id;
        //事件 对应客户端和服务器的协议号
        public string ProtocolAction;
        //结果
        public string ProtocolResult;
        //数据
        public string ProtocolData;
    }
}