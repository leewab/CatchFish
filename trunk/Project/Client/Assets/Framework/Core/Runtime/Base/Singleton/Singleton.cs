public class Singleton<T> where T : Singleton<T>, new()
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                instance.OnSingleInit();
            }

            return instance;
        }
    }

    public virtual void OnSingleInit()
    {
    }

    public virtual void OnSingleDispose()
    {
    }
}