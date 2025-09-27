namespace Framework.Case.Net.Socket
{
    public interface IMessage
    {
        string Name { get; set; }
        string Type { get; set; }
        object Body { get; set; }
    }
}