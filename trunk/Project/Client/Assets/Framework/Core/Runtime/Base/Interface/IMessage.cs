public interface IMessage
{
    bool HasCommand<T>(T t) where T : class;
    void RegisterCommand<T>(T t) where T : class;
    void UnRegisterCommand<T>(T t) where T : class;
    void ExecuteCommand(IMessageData message);
    void SendCommand<T>(IMessageData message) where T : class;
}

public interface IMessageData
{
    int Type { get; set; }
    string Name { get; set; }
    string From { get; set; }
    object Data { get; set; }
    string ToString();
}