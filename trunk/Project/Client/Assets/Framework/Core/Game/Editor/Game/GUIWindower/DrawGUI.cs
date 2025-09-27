using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    public class DrawGUI : BaseDrawGUI
    {
        #region GUIStyle

        public override GUIStyle BoxGUIStyle => boxGUIStyle ?? (boxGUIStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(0, 0, 0, 0),
        });

        public override GUIStyle LableGUIStyle => lableGUIStyle ?? (lableGUIStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(0, 0, 0, 0)
        });

        public override GUIStyle ButtonGUIStyle => buttonGUIStyle ?? (buttonGUIStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(0, 0, 0, 0)
        });

        public override GUIStyle BtnToggleGUIStyle => btnToggleGUIStyle ?? (btnToggleGUIStyle = new GUIStyle(GUI.skin.button));

        public override GUIStyle GridGUIStyle => gridGUIStyle ?? (gridGUIStyle = new GUIStyle
        {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(0, 0, 0, 0)
        });

        public override GUIStyle TitleGUIStyle => titleGUIStyle ?? (titleGUIStyle = new GUIStyle
        {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(0, 0, 0, 0),
            fontSize = 20,
            fontStyle = FontStyle.Bold
        });
        
        public override GUIStyle GetGUIStyle_WH(int _width, int _height)
        {
            if (_width > 0 && _height > 0)
            {
                return new GUIStyle {fixedWidth = _width, fixedHeight = _height};
            } 
            else if (_width <= 0 && _height > 0)
            {
                return new GUIStyle {fixedHeight = _height};
            }
            else if (_width > 0 && _height <= 0)
            {
                return new GUIStyle {fixedWidth = _width};
            }

            return null;
        }

        #endregion

        #region DrawButton

        public override void DrawButton(GUIContent _btnGUIContent, Action _onClickEvent, GUIStyle _guiStyle = null,
            params GUILayoutOption[] _options)
        {
            if (_btnGUIContent == null) _btnGUIContent = new GUIContent("NULL", "No tips!");
            if (_guiStyle == null) _guiStyle = GUI.skin.button;
            if (GUILayout.Button(_btnGUIContent, _guiStyle, _options))
            {
                _onClickEvent?.Invoke();
            }
        }

        public override void DrawButton(GUIContent _btnGUIContent, Vector2 _btnSize, Action onClickEvent)
        {
            if (_btnGUIContent == null) _btnGUIContent = new GUIContent("NULL", "No tips!");
            if (Math.Abs(_btnSize.x) < 1 && Math.Abs(_btnSize.y) < 1)
            {
                if (GUILayout.Button(_btnGUIContent))
                {
                    onClickEvent?.Invoke();
                }
            }
            else if (Math.Abs(_btnSize.x) < 1 && Math.Abs(_btnSize.y) > 1)
            {
                if (GUILayout.Button(_btnGUIContent, GUILayout.Height(_btnSize.y)))
                {
                    onClickEvent?.Invoke();
                }
            }
            else if (Math.Abs(_btnSize.x) > 1 && Math.Abs(_btnSize.y) < 1)
            {
                if (GUILayout.Button(_btnGUIContent, GUILayout.Width(_btnSize.x)))
                {
                    onClickEvent?.Invoke();
                }
            }
            else if (Math.Abs(_btnSize.x) > 1 && Math.Abs(_btnSize.y) > 1)
            {
                if (GUILayout.Button(_btnGUIContent, GUILayout.Width(_btnSize.x), GUILayout.Height(_btnSize.y)))
                {
                    onClickEvent?.Invoke();
                }
            }
        }

        public override void DrawButton(GUIContent _btnGUIContent, Vector2 _btnSize, Action onClickEvent,
            GUIStyle _gridStyle = null, GUIStyle _btnStyle = null)
        {
            if (_btnGUIContent == null) _btnGUIContent = new GUIContent("NULL", "No tips!");
            if (_gridStyle == null) _gridStyle = GridGUIStyle;
            if (_btnStyle == null) _btnStyle = ButtonGUIStyle;
            GUILayout.BeginHorizontal(_gridStyle); //GUILayout.Width(_size.x), GUILayout.Height(_size.y)
            if (_btnSize.x > 0 && _btnSize.y > 0)
            {
                if (GUILayout.Button(_btnGUIContent, _btnStyle, GUILayout.Width(_btnSize.x),
                    GUILayout.Height(_btnSize.y)))
                {
                    onClickEvent?.Invoke();
                }
            }
            else
            {
                if (GUILayout.Button(_btnGUIContent, _btnStyle))
                {
                    onClickEvent?.Invoke();
                }
            }

            GUILayout.EndHorizontal();
        }

        public override void DrawButton(GUIContent _btnGUIContent, RectOffset _rectOffset, Vector2 _size, Action onClickEvent,
            GUIStyle _gridStyle = null, GUIStyle _btnStyle = null)
        {
            if (_btnGUIContent == null) _btnGUIContent = new GUIContent("NULL", "No tips!");
            if (_gridStyle == null) _gridStyle = GridGUIStyle;
            if (_btnStyle == null) _btnStyle = ButtonGUIStyle;
            GUILayout.Space(_rectOffset.top);
            GUILayout.BeginHorizontal(_gridStyle); //GUILayout.Width(_size.x), GUILayout.Height(_size.y)
            GUILayout.Space(_rectOffset.left);
            if (GUILayout.Button(_btnGUIContent, _btnStyle, GUILayout.Width(_size.x), GUILayout.Height(_size.y),
                GUILayout.ExpandWidth(Math.Abs(_size.x) < 1), GUILayout.ExpandHeight(Math.Abs(_size.y) < 1)))
            {
                onClickEvent?.Invoke();
            }

            GUILayout.Space(_rectOffset.right);
            GUILayout.EndHorizontal();
            GUILayout.Space(_rectOffset.bottom);
        }

        public override void DrawButton(string _btnName, Action _onClickEvent, GUIStyle _guiStyle = null,
            params GUILayoutOption[] _options)
        {
            if (_btnName == null) _btnName = "NULL";
            DrawButton(new GUIContent(_btnName), _onClickEvent, _guiStyle, _options);
        }

        public override void DrawButton(string _btnName, Vector2 _btnSize, Action onClickEvent)
        {
            if (_btnName == null) _btnName = "NULL";
            DrawButton(new GUIContent(_btnName), _btnSize, onClickEvent);
        }

        public override void DrawButton(string _btnName, Vector2 _btnSize, Action onClickEvent, GUIStyle _gridStyle = null,
            GUIStyle _btnStyle = null)
        {
            if (_btnName == null) _btnName = "NULL";
            DrawButton(new GUIContent(_btnName), _btnSize, onClickEvent, _gridStyle, _btnStyle);
        }

        public override void DrawButton(string _btnName, RectOffset _rectOffset, Vector2 _size, Action onClickEvent,
            GUIStyle _gridStyle = null, GUIStyle _btnStyle = null)
        {
            if (_btnName == null) _btnName = "NULL";
            DrawButton(new GUIContent(_btnName), _rectOffset, _size, onClickEvent, _gridStyle, _btnStyle);
        }

        #endregion

        #region DrawLable

        /// <summary>
        /// 绘制标题
        /// </summary>
        /// <param name="_title"></param>
        /// <param name="_lableStyle"></param>
        public override void DrawTitle(string _title, GUIStyle _lableStyle = null)
        {
            if (_lableStyle == null)
            {
                _lableStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 0, 0),
                    fixedHeight = 60,
                    fontSize = 20,
                    fontStyle = FontStyle.Bold
                };
            }

            DrawLabel(_title, _lableStyle, GridGUIStyle);
        }

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        public override void DrawLabels(string[] _names, Vector2 _size)
        {
            for (int i = 0; i < _names.Length; i++)
            {
                DrawLabel(_names[i], _size);
            }
        }

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        public override void DrawLabels(string[] _names, Vector2[] _sizes)
        {
            for (int i = 0; i < _names.Length; i++)
            {
                DrawLabel(_names[i], _sizes[i]);
            }
        }

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        public override void DrawLabels(string[] _names, Vector2[] _sizes, GUIStyle[] _styles, GUIStyle _gridStyle)
        {
            for (int i = 0; i < _names.Length; i++)
            {
                DrawLabel(_names[i], _sizes[i], _styles[i], _gridStyle);
            }
        }

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_guiContent"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        public void DrawLabel(GUIContent _guiContent, GUIStyle _style = null, GUIStyle _gridStyle = null)
        {
            if (_style == null) _style = GUI.skin.label;
            if (_gridStyle == null) _gridStyle = GridGUIStyle;
            GUILayout.BeginVertical(_gridStyle);
            GUILayout.Label(_guiContent, _style);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_guiContent"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        /// <param name="_gridStyle"></param>
        public override void DrawLabel(GUIContent _guiContent, Vector2 _size, GUIStyle _style = null, GUIStyle _gridStyle = null)
        {
            if (_style == null) _style = GUI.skin.label;
            if (_gridStyle == null) _gridStyle = GridGUIStyle;
            GUILayout.BeginVertical(_gridStyle);
            GUILayout.Label(_guiContent, _style, GUILayout.Width(_size.x), GUILayout.Height(_size.y),
                GUILayout.ExpandWidth(Math.Abs(_size.x) < 1), GUILayout.ExpandHeight(Math.Abs(_size.y) < 1));
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制Lable-lable
        /// </summary>
        /// <param name="_guiContent"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        public override void DrawLabel(GUIContent _guiContent, Vector2 _nameSize, string _content, Vector2 _contentSize)
        {
            GUILayout.BeginHorizontal();
            if (Math.Abs(_nameSize.x) < 1 && Math.Abs(_nameSize.y) < 1)
            {
                GUILayout.Label(_guiContent);
            }
            else if (Math.Abs(_nameSize.x) < 1 && Math.Abs(_nameSize.y) > 1)
            {
                GUILayout.Label(_guiContent, GUILayout.Height(_nameSize.y));
            }
            else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) < 1)
            {
                GUILayout.Label(_guiContent, GUILayout.Width(_nameSize.x));
            }
            else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) > 1)
            {
                GUILayout.Label(_guiContent, GUILayout.Width(_nameSize.x), GUILayout.Height(_nameSize.y));
            }

            if (Math.Abs(_contentSize.x) < 1 && Math.Abs(_contentSize.y) < 1)
            {
                GUILayout.Label(_content);
            }
            else if (Math.Abs(_contentSize.x) < 1 && Math.Abs(_contentSize.y) > 1)
            {
                GUILayout.Label(_content, GUILayout.Height(_contentSize.y));
            }
            else if (Math.Abs(_contentSize.x) > 1 && Math.Abs(_contentSize.y) < 1)
            {
                GUILayout.Label(_content, GUILayout.Width(_contentSize.x));
            }
            else if (Math.Abs(_contentSize.x) > 1 && Math.Abs(_contentSize.y) > 1)
            {
                GUILayout.Label(_content, GUILayout.Width(_contentSize.x), GUILayout.Height(_contentSize.y));
            }

            GUILayout.EndHorizontal();
        }

        public override void DrawLabel(string _name, GUIStyle _style = null, GUIStyle _gridStyle = null)
        {
            DrawLabel(new GUIContent(_name), _style, _gridStyle);
        }
        
        public override void DrawLabel(string _name, Vector2 _size, GUIStyle _style = null, GUIStyle _gridStyle = null)
        {
            DrawLabel(new GUIContent(_name), _size, _style, _gridStyle);
        }

        public override void DrawLabel(string _name, Vector2 _nameSize, string _content, Vector2 _contentSize)
        {
            DrawLabel(new GUIContent(_name), _nameSize, _content, _contentSize);
        }

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
        public override string DrawField(GUIContent _nameGuiContent, string _field, Vector2 _nameSize, Vector2 _contentSize,
            int _contentMaxLength = 100, bool _isHorizontal = true)
        {
            if (_nameGuiContent == null) _nameGuiContent = new GUIContent("NULL");
            if (_field == null) _field = "";
            if (_isHorizontal)
            {
                GUILayout.BeginHorizontal();
                _nameSize.y = _contentSize.y;
            }
            else
            {
                GUILayout.BeginVertical();
            }

            if (Math.Abs(_nameSize.x) < 1 && Math.Abs(_nameSize.y) < 1)
            {
                GUILayout.Label(_nameGuiContent);
            }
            else if (Math.Abs(_nameSize.x) < 1 && Math.Abs(_nameSize.y) > 1)
            {
                GUILayout.Label(_nameGuiContent, GUILayout.Height(_nameSize.y));
            }
            else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) < 1)
            {
                GUILayout.Label(_nameGuiContent, GUILayout.Width(_nameSize.x));
            }
            else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) > 1)
            {
                GUILayout.Label(_nameGuiContent, GUILayout.Width(_nameSize.x), GUILayout.Height(_nameSize.y));
            }

            string result = "";
            if (Math.Abs(_contentSize.x) < 1 && Math.Abs(_contentSize.y) < 1)
            {
                result = GUILayout.TextField(_field, _contentMaxLength);
            }
            else if (Math.Abs(_contentSize.x) < 1 && Math.Abs(_contentSize.y) > 1)
            {
                result = GUILayout.TextArea(_field, _contentMaxLength, GUI.skin.textField,
                    GUILayout.Height(_contentSize.y));
            }
            else if (Math.Abs(_contentSize.x) > 1 && Math.Abs(_contentSize.y) < 1)
            {
                result = GUILayout.TextField(_field, _contentMaxLength, GUILayout.Width(_contentSize.x));
            }
            else if (Math.Abs(_contentSize.x) > 1 && Math.Abs(_contentSize.y) > 1)
            {
                result = GUILayout.TextArea(_field, _contentMaxLength, GUI.skin.textField,
                    GUILayout.Width(_contentSize.x), GUILayout.Height(_contentSize.y));
            }

            if (_isHorizontal)
            {
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.EndVertical();
            }

            return result.TrimStart();
        }

        public override string DrawField(GUIContent _guiContent, string _field, RectOffset _rectOffset, Vector2 _size,
            GUIStyle _borderStyle = null, GUIStyle _lableStyle = null, GUIStyle _fieldStyle = null)
        {
            if (_guiContent == null) _guiContent = new GUIContent("NULL");
            if (_field == null) _field = "";
            if (_borderStyle == null)
                _borderStyle = new GUIStyle
                {
                    fixedHeight = 25,
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 0, 0)
                };
            if (_lableStyle == null)
            {
                _lableStyle = GUI.skin.label;
                _lableStyle.alignment = TextAnchor.MiddleRight;
            }

            if (_fieldStyle == null)
            {
                _fieldStyle = new GUIStyle(GUI.skin.textField)
                {
                    fixedHeight = 20,
                    margin = new RectOffset(0, 0, 0, 0),
                    alignment = TextAnchor.MiddleLeft,
                };
            }

            GUILayout.Space(_rectOffset.top);
            GUILayout.BeginHorizontal(_borderStyle);
            GUILayout.Space(_rectOffset.left);
            GUILayout.Label(_guiContent, _lableStyle, GUILayout.Width(_size.x), GUILayout.ExpandWidth(Math.Abs(_size.x) < 1));
            var result = GUILayout.TextField(_field, 200, _fieldStyle, GUILayout.Width(_size.y),
                GUILayout.ExpandWidth(Math.Abs(_size.y) < 1));
            GUILayout.Space(_rectOffset.right);
            GUILayout.EndHorizontal();
            GUILayout.Space(_rectOffset.bottom);
            return result.TrimStart();
        }

        public override string DrawField(string _name, string _field, Vector2 _nameSize, Vector2 _contentSize, int _contentMaxLength = 100, bool _isHorizontal = true)
        {
            return DrawField(new GUIContent(_name), _field, _nameSize, _contentSize, _contentMaxLength, _isHorizontal);
        }

        public override string DrawField(string _name, string _field, RectOffset _rectOffset, Vector2 _size, GUIStyle _borderStyle = null, GUIStyle _lableStyle = null, GUIStyle _fieldStyle = null)
        {
            return DrawField(new GUIContent(_name), _field, _rectOffset, _size, _borderStyle, _lableStyle, _fieldStyle);
        }

        #endregion

        #region DrawGrid

        public override void DrawGridVertical(Action delegateEvent, int _width = 0, int _height = 0)
        {
            var style = GetGUIStyle_WH(_width, _height);
            if (style != null)
            {
                GUILayout.BeginVertical(style);
                delegateEvent?.Invoke();
                GUILayout.EndVertical();  
            }
            else
            {
                GUILayout.BeginVertical();
                delegateEvent?.Invoke();
                GUILayout.EndVertical();  
            }
        }

        public override void DrawGridHorizontal(Action delegateEvent, int _width = 0, int _height = 0)
        {
            var style = GetGUIStyle_WH(_width, _height);
            if (style != null)
            {
                GUILayout.BeginHorizontal(style);
                delegateEvent?.Invoke();
                GUILayout.EndHorizontal();  
            }
            else
            {
                GUILayout.BeginHorizontal();
                delegateEvent?.Invoke();
                GUILayout.EndHorizontal();  
            }
        }

        #endregion

        #region DrawProgressBar

        public override void ShowProgressBar(float curIndex, float totalIndex, string title, string info, Action finishedEvent = null)
        {
            EditorUtility.DisplayProgressBar(title, info, curIndex / totalIndex);
            if (curIndex >= totalIndex)
            {
                EditorUtility.ClearProgressBar();
                finishedEvent?.Invoke();
            }
        }

        #endregion

        #region DrawToggle

        public override bool DrawToggle(string name, bool value)
        {
            if (name == null) name = "";
            GUILayout.BeginHorizontal(new GUIStyle {fixedHeight = 25, alignment = TextAnchor.MiddleCenter});
            var result = GUILayout.Toggle(value, new GUIContent(name));
            GUILayout.EndHorizontal();
            return result;
        }

        public override bool DrawToggle(GUIContent guiContent, bool value)
        {
            GUILayout.BeginHorizontal(new GUIStyle {fixedHeight = 25, alignment = TextAnchor.MiddleCenter});
            var result = GUILayout.Toggle(value, guiContent);
            GUILayout.EndHorizontal();
            return result;
        }

        #endregion
        
        #region DrawToolBar

        public override int DrawToolbar(string[] names, int selected, UnityEngine.Vector2 nameSize)
        {
            if (Math.Abs(nameSize.x) < 1 && Math.Abs(nameSize.y) < 1)
            {
                selected = GUILayout.Toolbar(selected, names);

            }
            else if (Math.Abs(nameSize.x) < 1 && Math.Abs(nameSize.y) > 1)
            {
                selected = GUILayout.Toolbar(selected, names, GUILayout.Height(nameSize.y));

            }
            else if (Math.Abs(nameSize.x) > 1 && Math.Abs(nameSize.y) < 1)
            {
                selected = GUILayout.Toolbar(selected, names,GUILayout.Width(nameSize.x));

            }
            else if (Math.Abs(nameSize.x) > 1 && Math.Abs(nameSize.y) > 1)
            {
                selected = GUILayout.Toolbar(selected, names,GUILayout.Width(nameSize.x), GUILayout.Height(nameSize.y));
            }
            return selected;
        }

        public override int DrawToolbar(string[] names, int selected, Vector2 nameSize, int space, bool isHorizontal = false)
        {
            if (null == names) return selected;
            if(isHorizontal) GUILayout.BeginHorizontal();
            for (int i = 0; i < names.Length; i++)
            {
                GUILayout.Space(space);
                bool result;
                if (i == selected)
                {
                    result = GUILayout.Toggle(true, names[i], GUI.skin.button, GUILayout.Width(nameSize.x), GUILayout.Height(nameSize.y), GUILayout.ExpandWidth(true));
                }
                else
                {
                    result = GUILayout.Toggle(false, names[i], GUI.skin.button, GUILayout.Width(nameSize.x), GUILayout.Height(nameSize.y), GUILayout.ExpandWidth(true));
                }

                if (result)
                {
                    selected = i;
                }
            }
            if(isHorizontal) GUILayout.EndHorizontal();
            return selected;
        }

        public override int DrawToolbar(GUIContent[] guiContents, int selected, UnityEngine.Vector2 nameSize)
        {
            if (Math.Abs(nameSize.x) < 1 && Math.Abs(nameSize.y) < 1)
            {
                selected = GUILayout.Toolbar(selected, guiContents);

            }
            else if (Math.Abs(nameSize.x) < 1 && Math.Abs(nameSize.y) > 1)
            {
                selected = GUILayout.Toolbar(selected, guiContents, GUILayout.Height(nameSize.y));

            }
            else if (Math.Abs(nameSize.x) > 1 && Math.Abs(nameSize.y) < 1)
            {
                selected = GUILayout.Toolbar(selected, guiContents,GUILayout.Width(nameSize.x));

            }
            else if (Math.Abs(nameSize.x) > 1 && Math.Abs(nameSize.y) > 1)
            {
                selected = GUILayout.Toolbar(selected, guiContents,GUILayout.Width(nameSize.x), GUILayout.Height(nameSize.y));
            }
            return selected;
        }

        public override int DrawToolbar(GUIContent[] guiContents, int selected, Vector2 nameSize, int space, bool isHorizontal = false)
        {
            if(isHorizontal) GUILayout.BeginHorizontal();
            for (int i = 0; i < guiContents.Length; i++)
            {
                GUILayout.Space(space);
                bool result;
                if (i == selected)
                {
                    result = GUILayout.Toggle(true, guiContents[i], GUI.skin.button, GUILayout.Width(nameSize.x), GUILayout.Height(nameSize.y), GUILayout.ExpandWidth(true));
                }
                else
                {
                    result = GUILayout.Toggle(false, guiContents[i], GUI.skin.button, GUILayout.Width(nameSize.x), GUILayout.Height(nameSize.y), GUILayout.ExpandWidth(true));
                }

                if (result)
                {
                    selected = i;
                }
            }
            if(isHorizontal) GUILayout.EndHorizontal();
            return selected;
        }

        #endregion

        #region DrawTable

        public override UnityEngine.Object[] DrawDragArea(UnityEngine.Vector2 size, string content = "", params UnityEngine.GUILayoutOption[] options)
        {
            return DrawDragArea(size, new GUIContent(content), options);
        }

        public override UnityEngine.Object[] DrawDragArea(UnityEngine.Vector2 size, GUIContent guiContent, params UnityEngine.GUILayoutOption[] options)
        {
            Event _event = Event.current;
            var dragArea = GUILayoutUtility.GetRect(size.x, size.y, options);
            GUI.Box(dragArea, guiContent);
            List<UnityEngine.Object> temps = new List<UnityEngine.Object>();
            if(dragArea.Contains(_event.mousePosition))
            {
                switch (_event.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Event.current.Use();
                        break;
                    case EventType.DragPerform:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        DragAndDrop.AcceptDrag();
                        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                        {
                            if (DragAndDrop.objectReferences[i] != null)
                            {
                                temps.Add(DragAndDrop.objectReferences[i]);
                            }
                        }
                        Event.current.Use();
                        break;
                }
            }

            return temps.ToArray();
        }

        #endregion
    }
}