using System;
using UnityEngine;

namespace Game.UI
{
    
    public abstract class BaseDrawGUI : IDrawButton, IDrawGrid, IDrawTable, IDrawLabel, IDrawField, IProgressBar, IToggle, IToolBar
    {
        #region GUIStyle

        protected GUIStyle boxGUIStyle;
        public abstract GUIStyle BoxGUIStyle { get; }
        
        protected GUIStyle lableGUIStyle;
        public abstract GUIStyle LableGUIStyle { get; }
        
        protected GUIStyle buttonGUIStyle;
        public abstract GUIStyle ButtonGUIStyle { get; }
        
        protected GUIStyle btnToggleGUIStyle;
        public abstract GUIStyle BtnToggleGUIStyle { get; }
        
        protected GUIStyle gridGUIStyle;
        public abstract GUIStyle GridGUIStyle { get; }
        
        protected GUIStyle titleGUIStyle = null;
        public abstract GUIStyle TitleGUIStyle { get; }

        public abstract GUIStyle GetGUIStyle_WH(int _width, int _height);

        #endregion

        #region DrawButton

        public abstract void DrawButton(GUIContent _btnGUIContent, Action _onClickEvent, GUIStyle _guiStyle = null, params GUILayoutOption[] _options);
        public abstract void DrawButton(GUIContent _btnGUIContent, Vector2 _btnSize, Action onClickEvent);
        public abstract void DrawButton(GUIContent _btnGUIContent, Vector2 _btnSize, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null);
        public abstract void DrawButton(GUIContent _btnGUIContent, RectOffset _rectOffset, Vector2 _size, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null);
        public abstract void DrawButton(string _btnName, Action _onClickEvent, GUIStyle _guiStyle = null, params GUILayoutOption[] _options);
        public abstract void DrawButton(string _btnName, Vector2 _btnSize, Action onClickEvent);
        public abstract void DrawButton(string _btnName, Vector2 _btnSize, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null);
        public abstract void DrawButton(string _btnName, RectOffset _rectOffset, Vector2 _size, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null);

        #endregion
        
        #region DrawLabel

        public abstract void DrawTitle(string _title, GUIStyle _lableStyle = null);
        public abstract void DrawLabels(string[] _names, Vector2 _size);
        public abstract void DrawLabels(string[] _names, Vector2[] _sizes);
        public abstract void DrawLabels(string[] _names, Vector2[] _sizes, GUIStyle[] _styles, GUIStyle _gridStyle);
        public abstract void DrawLabel(string _name, GUIStyle _style = null, GUIStyle _gridStyle = null);
        public abstract void DrawLabel(string _name, Vector2 _size, GUIStyle _style = null, GUIStyle _gridStyle = null);
        public abstract void DrawLabel(string _name, Vector2 _nameSize, string _content, Vector2 _contentSize);
        public abstract void DrawLabel(GUIContent _guiContent, Vector2 _size, GUIStyle _style = null, GUIStyle _gridStyle = null);
        public abstract void DrawLabel(GUIContent _guiContent, Vector2 _nameSize, string _content, Vector2 _contentSize);

        #endregion
        
        #region DrawField

        /// <summary>
        /// 集成InputField，包含TextField TextArea 当无多行特性是使用TextField 当有多行特性时使用TextArea 
        /// </summary>
        /// <param name="_nameGuiContent"></param>
        /// <param name="_field"></param>
        /// <param name="_nameSize"></param>
        /// <param name="_contentSize"></param>
        /// <param name="_contentMaxLength"></param>
        /// <param name="_isHorizontal"></param>
        /// <returns></returns>
        public abstract string DrawField(GUIContent _nameGuiContent, string _field, Vector2 _nameSize, Vector2 _contentSize, int _contentMaxLength = 100, bool _isHorizontal = true);
        public abstract string DrawField(GUIContent _guiContent, string _field, RectOffset _rectOffset, Vector2 _size, GUIStyle _borderStyle = null, GUIStyle _lableStyle = null, GUIStyle _fieldStyle = null);
        public abstract string DrawField(string _name, string _field, Vector2 _nameSize, Vector2 _contentSize, int _contentMaxLength = 100, bool _isHorizontal = true);
        public abstract string DrawField(string _name, string _field, RectOffset _rectOffset, Vector2 _size, GUIStyle _borderStyle = null, GUIStyle _lableStyle = null, GUIStyle _fieldStyle = null);

        #endregion
        
        #region DrawGrid

        public abstract void DrawGridVertical(Action delegateEvent, int _width = 0, int _height = 0);
        public abstract void DrawGridHorizontal(Action delegateEvent, int _width = 0, int _height = 0);

        #endregion

        #region DrawProgressBar

        public abstract void ShowProgressBar(float curIndex, float totalIndex, string title, string info, Action finishedEvent = null);

        #endregion

        #region DrawToggle

        public abstract bool DrawToggle(string name, bool value);
        public abstract bool DrawToggle(GUIContent guiContent, bool value);

        #endregion

        #region DrawToolBar

        public abstract int DrawToolbar(string[] names, int selected, UnityEngine.Vector2 nameSize);
        public abstract int DrawToolbar(string[] names, int selected, Vector2 nameSize, int space, bool isHorizontal = false);
        public abstract int DrawToolbar(GUIContent[] guiContents, int selected, UnityEngine.Vector2 nameSize);
        public abstract int DrawToolbar(GUIContent[] guiContents, int selected, Vector2 nameSize, int space, bool isHorizontal = false);

        #endregion

        #region DrawTable

        public abstract UnityEngine.Object[] DrawDragArea(UnityEngine.Vector2 size, string content = "", params UnityEngine.GUILayoutOption[] options);
        public abstract UnityEngine.Object[] DrawDragArea(UnityEngine.Vector2 size, GUIContent guiContent, params UnityEngine.GUILayoutOption[] options);

        #endregion
    }
}