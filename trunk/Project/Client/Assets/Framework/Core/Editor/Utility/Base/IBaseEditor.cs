namespace Framework.Core
{
    public interface IBaseEditor
    {
        void Init();

        void RegisterEvent();

        void UnRegisterEvent();

        void Clear();
    }
}