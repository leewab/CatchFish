public interface IMono
{
    void Awake();
    void Start();
    void Update();
    void LogicUpdate();
    void LateUpdate();
    void FixedUpdate();
    void OnEnable();
    void OnDisable();
    void OnDestroy();
}