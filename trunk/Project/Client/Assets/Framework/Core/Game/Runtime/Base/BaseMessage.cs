namespace Game
{
    public class BaseMessage : IMessageData
    {
        //消息类型
        public int Type { get; set; }
        //消息名称 消息的时间类型可以在MsgEvent中注册
        public string Name { get; set; }
        //消息来源
        public string From { get; set; }
        //消息数据
        public object Data { get; set; }
    }
}