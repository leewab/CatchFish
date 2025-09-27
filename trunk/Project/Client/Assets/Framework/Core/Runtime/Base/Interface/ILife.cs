 public interface ILife
{
    bool Enable { get; }

    void Init();

    void Dispose();

    void Start();

    /// <summary>
    /// 无限制执行帧率的Update
    /// </summary>
    void Update();

    /// <summary>
    /// 限制执行帧率的逻辑Update
    /// </summary>
    void LogicUpdate();
    
    void FixedUpdate();
    
    void LateUpdate();

    void RegisterEvent();

    void UnRegisterEvent();
}