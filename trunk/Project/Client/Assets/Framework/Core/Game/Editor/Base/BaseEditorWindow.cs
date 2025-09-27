using System;

namespace Game.UI
{
    using UnityEditor;

    public class BaseEditorWindow : EditorWindow
    {
        protected bool mIsInited = false;
        protected BaseEditorView mEditorView;
        protected BaseEditorModel mEditorModule;

        #region virtual function

        protected virtual void Awake()
        {
            this.Init();
            mEditorModule?.Awake();
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
            mEditorView?.OnGUI();
            
        }

        protected virtual void Update()
        {
            mEditorView?.Update();
            mEditorModule?.Update();
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
            mEditorView?.Clear();
            mEditorModule?.Clear();
        }

        #endregion
    }
}