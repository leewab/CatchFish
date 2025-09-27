namespace Game.UI
{
    using UnityEngine;
    using System;

    public abstract class BaseEditorView : IEditorView
    {
        protected GUIStyle pBoxGUIStyle;
        protected GUIStyle pLableGUIStyle;
        protected GUIStyle pButtonGUIStyle;
        protected GUIStyle pBtnToggleGUIStyle;
        protected GUIStyle pGridGUIStyle;
        protected GUIStyle pTitleGUIStyle;
        protected GUIStyle pBlueLableStyle;

        public virtual void OnGUI()
        {
            InitStyle();
        }

        public virtual void OnGUI(BaseEditorModel model)
        {
            InitStyle();
        }

        public virtual void OnGUI(BaseEditorModel model, params Action[] actions)
        {
            InitStyle();
        }

        public virtual void Update()
        {
            
        }

        public virtual void Clear()
        {
            
        }

        private void InitStyle()
        {
            if (pBoxGUIStyle == null)
            {
                pBoxGUIStyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.MiddleCenter,
                    // margin = new RectOffset(0, 0, 0, 0),
                };
            }

            if (pLableGUIStyle == null)
            {
                pLableGUIStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }

            if (pButtonGUIStyle == null)
            {
                pButtonGUIStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }

            if (pBtnToggleGUIStyle == null)
            {
                pBtnToggleGUIStyle = new GUIStyle(GUI.skin.button);
            }

            if (pGridGUIStyle == null)
            {
                pGridGUIStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }

            if (pTitleGUIStyle == null)
            {
                pTitleGUIStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 0, 0),
                    border = pButtonGUIStyle.border,
                    imagePosition = pButtonGUIStyle.imagePosition,
                };
            }

            if (pBlueLableStyle == null)
            {
                pBlueLableStyle = new GUIStyle
                {
                    normal = {textColor = Color.blue},
                    alignment = TextAnchor.MiddleCenter
                };
            }
        }
    }
}