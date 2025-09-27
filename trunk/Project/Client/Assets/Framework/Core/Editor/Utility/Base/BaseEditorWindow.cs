namespace Framework.Core
{
    using UnityEditor;

    public class BaseEditorWindow : EditorWindow, IBaseEditor
    {
        protected bool mIsInited = false;

        #region virtual function

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void OnEnable()
        {
            RegisterEvent();
        }

        protected virtual void OnDisable()
        {
            UnRegisterEvent();
        }

        protected virtual void OnFocus()
        {

        }

        protected virtual void OnLostFocus()
        {

        }

        protected virtual void OnGUI()
        {
            if (!mIsInited) return;
        }

        protected virtual void OnDestroy()
        {
            Clear();
        }

        #endregion

        #region implement function

        public virtual void Init()
        {

        }

        public virtual void RegisterEvent()
        {

        }

        public virtual void UnRegisterEvent()
        {

        }

        public virtual void Clear()
        {

        }

        #endregion
    }
}