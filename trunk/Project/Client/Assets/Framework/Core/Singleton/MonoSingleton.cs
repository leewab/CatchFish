using UnityEngine;

namespace Framework.Core
{
    public class MonoSingleton<T> : MonoBehaviour
        where T : Component
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        var obj = new GameObject($"MonoSingleton_{typeof(T).Name}");
                        instance = obj.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        private void Awake()
        {
            OnSingletonInit(false);
        }

        private void OnDestroy()
        {
            OnSingletonDispose();
        }

        protected virtual void OnSingletonInit(bool isDontDestory = false)
        {
            if (isDontDestory)
            {
                DontDestroyOnLoad(gameObject);
                if (instance == null)
                {
                    instance = this as T;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        protected virtual void OnSingletonDispose()
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (instance != null)
            {
                instance = null;
            }
        }
    }
}
