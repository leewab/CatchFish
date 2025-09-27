namespace Framework.Core
{
    using UnityEngine;
    using UnityEditor;
    using System;

    public class BaseEditorView
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

        public virtual void OnGUI(BaseEditorModule module)
        {
            InitStyle();
        }

        public virtual void OnGUI(BaseEditorModule module, params Action[] actions)
        {
            InitStyle();
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

        #region common function

        protected enum DirectionState
        {
            Left,
            Right,
            Up,
            Down
        }

        protected int pCustomWidth = 100;

        /// <summary>
        /// 更加字体大小 颜色 创建一个GUIStyle
        /// </summary>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        protected GUIStyle GetGUIStyle(int size, Color color)
        {
            return new GUIStyle
            {
                normal = new GUIStyleState
                {
                    textColor = color,
                },
                fontSize = size
            };
        }

        /// <summary>
        /// 空置行数
        /// </summary>
        /// <param name="num"></param>
        protected void EditorGUISpace(int height = 1)
        {
            for (int i = 0; i < height; i++)
            {
                EditorGUILayout.Space();
            }
        }

        protected void LableField(string txt, float offsetWidth = 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(txt, GUILayout.Width(pCustomWidth + offsetWidth));
            EditorGUILayout.EndHorizontal();
        }

        protected void LableField(string txt, GUIStyle style)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(txt, style);
            EditorGUILayout.EndHorizontal();
        }

        protected void LableField(string txt, GUIStyle style, float offsetWidth = 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(txt, style, GUILayout.Width(pCustomWidth + offsetWidth));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Button -1表示无长度限制
        /// </summary>
        /// <param name="name"></param>
        /// <param name="width"></param>
        protected void GUIButton(string name, Action onClickCallBack, int width = -1, int height = 20)
        {
            if (width >= 0)
            {
                if (GUILayout.Button(name, GUILayout.Width(width), GUILayout.Height(height)))
                {
                    onClickCallBack?.Invoke();
                }
            }
            else
            {
                if (GUILayout.Button(name, GUILayout.Height(height)))
                {
                    onClickCallBack?.Invoke();
                }
            }
        }

        protected void Str_Str_InputField(string name, string content, float offseContenttWidth = 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(name, content, GUILayout.Width(pCustomWidth + offseContenttWidth));
            EditorGUILayout.EndHorizontal();
        }

        protected void Str_Int_InputField(string name, int content, float offseContenttWidth = 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.IntField(name, content, GUILayout.Width(pCustomWidth + offseContenttWidth));
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}