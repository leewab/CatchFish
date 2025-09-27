namespace Framework.Case.Net.Socket
{
    public class Message : IMessage
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Body { get; set; }
    }
}