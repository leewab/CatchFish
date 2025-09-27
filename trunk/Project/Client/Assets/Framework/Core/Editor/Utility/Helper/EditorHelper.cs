using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Core
{
#if UNITY_EDITOR
   public static class EditorHelper
   {
      private static GUIStyle boxGUIStyle;

      public static GUIStyle BoxGUIStyle
      {
         get
         {
            if (boxGUIStyle == null)
            {
               boxGUIStyle = new GUIStyle(GUI.skin.box)
               {
                  alignment = TextAnchor.MiddleCenter,
                  margin = new RectOffset(0, 0, 0, 0),
               };
            }
            return boxGUIStyle; 
         }
      }
        
      private static GUIStyle lableGUIStyle;

      public static GUIStyle LableGUIStyle
      {
         get
         {
            if (lableGUIStyle == null)
            {
               lableGUIStyle = new GUIStyle(GUI.skin.label)
               {
                  alignment = TextAnchor.MiddleCenter,
                  margin = new RectOffset(0, 0, 0, 0)
               };
            }
            return lableGUIStyle;
         }
      }
        
      private static GUIStyle buttonGUIStyle;

      public static GUIStyle ButtonGUIStyle
      {
         get
         {
            if (buttonGUIStyle == null)
            {
               buttonGUIStyle = new GUIStyle(GUI.skin.button)
               {
                  alignment = TextAnchor.MiddleCenter,
                  margin = new RectOffset(0, 0, 0, 0)
               };
            }
            return buttonGUIStyle;
         }
      }
        
      private static GUIStyle btnToggleGUIStyle;

      public static GUIStyle BtnToggleGUIStyle
      {
         get
         {
            if (btnToggleGUIStyle == null)
            {
               btnToggleGUIStyle = new GUIStyle(GUI.skin.button);
            }
            return btnToggleGUIStyle;
         }
      }
        
      private static GUIStyle gridGUIStyle;

      public static GUIStyle GridGUIStyle
      {
         get
         {
            if (gridGUIStyle == null)
            {
               gridGUIStyle = new GUIStyle
               {
                  alignment = TextAnchor.MiddleCenter,
                  margin = new RectOffset(0, 0, 0, 0)
               };
            }
            return gridGUIStyle;
         }
      }
        
      private static GUIStyle titleGUIStyle = null;

      public static GUIStyle TitleGUIStyle
      {
         get
         {
            if (titleGUIStyle == null)
            {
               titleGUIStyle = new GUIStyle
               {
                  alignment = TextAnchor.MiddleCenter,
                  margin = new RectOffset(0, 0, 0, 0),
                  fontSize = 20,
                  fontStyle = FontStyle.Bold
               };
            }
            return titleGUIStyle;
         }
      }
      
      public static void ShowProgressBar(float curIndex, float totalIndex, string title, string info, Action _finished = null)
      {
         EditorUtility.DisplayProgressBar(title, info, curIndex / totalIndex);
         if (curIndex >= totalIndex)
         {
            EditorUtility.ClearProgressBar();
            _finished?.Invoke();
         }
      }
        
      /// <summary>
      /// 绘制标题
      /// </summary>
      /// <param name="_title"></param>
      /// <param name="_lableStyle"></param>
      public static void DrawTitle(string _title, GUIStyle _lableStyle = null)
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
         DrawLable(_title, _lableStyle, GridGUIStyle);
      }
      
      /// <summary>
      /// 绘制Lable
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLables(string[] _names, Vector2 _size)
      {
         for (int i = 0; i < _names.Length; i++)
         {
            DrawLable(_names[i], _size);
         }
      }
      
      /// <summary>
      /// 绘制Lable
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLables(string[] _names, Vector2[] _sizes)
      {
         for (int i = 0; i < _names.Length; i++)
         {
            DrawLable(_names[i], _sizes[i]);
         }
      }
        
      /// <summary>
      /// 绘制Lable
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLables(string[] _names, Vector2[] _sizes, GUIStyle[] _styles, GUIStyle _gridStyle)
      {
         for (int i = 0; i < _names.Length; i++)
         {
            DrawLable(_names[i], _sizes[i], _styles[i], _gridStyle);
         }
      }
      
      /// <summary>
      /// 绘制Lable
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLable(string _name, GUIStyle _style = null, GUIStyle _gridStyle = null)
      {
         if (_style == null) _style = GUI.skin.label; 
         if (_gridStyle == null) _gridStyle = GridGUIStyle; 
         GUILayout.BeginVertical(_gridStyle);
         GUILayout.Label(_name, _style);
         GUILayout.EndVertical();
      }

      /// <summary>
      /// 绘制Lable
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLable(string _name, Vector2 _size, GUIStyle _style = null, GUIStyle _gridStyle = null)
      {
         if (_style == null) _style = GUI.skin.label; 
         if (_gridStyle == null) _gridStyle = GridGUIStyle; 
         GUILayout.BeginVertical(_gridStyle);
         GUILayout.Label(_name, _style, GUILayout.Width(_size.x), GUILayout.Height(_size.y), GUILayout.ExpandWidth(Math.Abs(_size.x) < 1), GUILayout.ExpandHeight(Math.Abs(_size.y) < 1));
         GUILayout.EndVertical();
      }
      
      /// <summary>
      /// 绘制Lable-lable
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLable(string _name, Vector2 _nameSize, string _content, Vector2 _contentSize)
      {
         GUILayout.BeginHorizontal();
         if (Math.Abs(_nameSize.x) < 1 && Math.Abs(_nameSize.y) < 1)
         {
            GUILayout.Label(_name);
         }
         else if (Math.Abs(_nameSize.x) < 1 && Math.Abs(_nameSize.y) > 1)
         {
            GUILayout.Label(_name, GUILayout.Height(_nameSize.y));
         }
         else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) < 1)
         {
            GUILayout.Label(_name, GUILayout.Width(_nameSize.x));
         }
         else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) > 1)
         {
            GUILayout.Label(_name, GUILayout.Width(_nameSize.x), GUILayout.Height(_nameSize.y));
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

        
      public static bool DrawToggle(string _name, bool _value)
      {
         if (_name == null) _name = "";
         GUILayout.BeginHorizontal(new GUIStyle {fixedHeight = 25, alignment = TextAnchor.MiddleCenter});
         var result = GUILayout.Toggle(_value, _name);
         GUILayout.EndHorizontal();
         return result;
      }
      
      /// <summary>
      /// 按钮页签式 Unity自带的Toolbar 只能水平分布
      /// </summary>
      /// <param name="_names"></param>
      /// <param name="_selected"></param>
      /// <param name="_nameSize"></param>
      /// <returns></returns>
      public static int DrawToolbar(string[] _names, int _selected, Vector2 _nameSize)
      {
         if (Math.Abs(_nameSize.x) < 1 && Math.Abs(_nameSize.y) < 1)
         {
            _selected = GUILayout.Toolbar(_selected, _names);

         }
         else if (Math.Abs(_nameSize.x) < 1 && Math.Abs(_nameSize.y) > 1)
         {
            _selected = GUILayout.Toolbar(_selected, _names, GUILayout.Height(_nameSize.y));

         }
         else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) < 1)
         {
            _selected = GUILayout.Toolbar(_selected, _names,GUILayout.Width(_nameSize.x));

         }
         else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) > 1)
         {
            _selected = GUILayout.Toolbar(_selected, _names,GUILayout.Width(_nameSize.x), GUILayout.Height(_nameSize.y));
         }
         return _selected;
      }
      
      /// <summary>
      /// 按钮页签式（解决了页（Toolbar）签式分类无法竖直排列的问题）
      /// </summary>
      /// <param name="_names"></param>
      /// <param name="_nameSize"></param>
      public static int DrawToolbar(string[] _names, int _selected, Vector2 _nameSize, int _space, bool _isHorizontal = false)
      {
         if (null == _names) return _selected;
         if(_isHorizontal) GUILayout.BeginHorizontal();
         for (int i = 0; i < _names.Length; i++)
         {
            GUILayout.Space(_space);
            bool result;
            if (i == _selected)
            {
               result = GUILayout.Toggle(true, _names[i], GUI.skin.button, GUILayout.Width(_nameSize.x), GUILayout.Height(_nameSize.y), GUILayout.ExpandWidth(true));
            }
            else
            {
               result = GUILayout.Toggle(false, _names[i], GUI.skin.button, GUILayout.Width(_nameSize.x), GUILayout.Height(_nameSize.y), GUILayout.ExpandWidth(true));
            }

            if (result)
            {
               _selected = i;
            }
         }
         if(_isHorizontal) GUILayout.EndHorizontal();
         return _selected;
      }
      
      /// <summary>
      /// 集成InputField，包含TextField TextArea 当无多行特性是使用TextField 当有多行特性时使用TextArea 
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="__field"></param>
      /// <param name="_nameSize"></param>
      /// <param name="_contentSize"></param>
      /// <param name="_contentMaxLength"></param>
      /// <param name="_isHorizontal"></param>
      /// <returns></returns>
      public static string DrawField(string _name, string __field, Vector2 _nameSize, Vector2 _contentSize, int _contentMaxLength = 100, bool _isHorizontal = true)
      {
         if (_name == null) _name = "";
         if (__field == null) __field = "";
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
            GUILayout.Label(_name);
         }
         else if (Math.Abs(_nameSize.x) < 1 && Math.Abs(_nameSize.y) > 1)
         {
            GUILayout.Label(_name, GUILayout.Height(_nameSize.y));
         } 
         else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) < 1)
         {
            GUILayout.Label(_name, GUILayout.Width(_nameSize.x));
         }
         else if (Math.Abs(_nameSize.x) > 1 && Math.Abs(_nameSize.y) > 1)
         {
            GUILayout.Label(_name, GUILayout.Width(_nameSize.x), GUILayout.Height(_nameSize.y));
         }

         string result = "";
         if (Math.Abs(_contentSize.x) < 1 && Math.Abs(_contentSize.y) < 1 )
         {
            result = GUILayout.TextField(__field, _contentMaxLength);
         }
         else if (Math.Abs(_contentSize.x) < 1 && Math.Abs(_contentSize.y) > 1 )
         {
            result = GUILayout.TextArea(__field, _contentMaxLength, GUI.skin.textField, GUILayout.Height(_contentSize.y));
         }
         else if (Math.Abs(_contentSize.x) > 1 && Math.Abs(_contentSize.y) < 1 )
         {
            result = GUILayout.TextField(__field, _contentMaxLength, GUILayout.Width(_contentSize.x));
         }
         else if (Math.Abs(_contentSize.x) > 1 && Math.Abs(_contentSize.y) > 1 )
         {
            result = GUILayout.TextArea(__field, _contentMaxLength, GUI.skin.textField, GUILayout.Width(_contentSize.x), GUILayout.Height(_contentSize.y));
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

      public static string DrawField(string _name, string __field, RectOffset _rectOffset,Vector2 _size, GUIStyle _borderStyle = null, GUIStyle _lableStyle = null, GUIStyle _fieldStyle = null)
      {
         if (_name == null) _name = "";
         if (__field == null) __field = "";
         if (_borderStyle == null) _borderStyle = new GUIStyle
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
         GUILayout.Label(_name, _lableStyle, GUILayout.Width(_size.x), GUILayout.ExpandWidth(Math.Abs(_size.x) < 1));
         var result = GUILayout.TextField(__field, 200, _fieldStyle, GUILayout.Width(_size.y), GUILayout.ExpandWidth(Math.Abs(_size.y) < 1));
         GUILayout.Space(_rectOffset.right);
         GUILayout.EndHorizontal();
         GUILayout.Space(_rectOffset.bottom);
         return result.TrimStart();
      }

      public static void DrawButton(string _btnName, Action _onClickEvent, GUIStyle _guiStyle = null, params GUILayoutOption[] _options)
      {
         if (_btnName == null) _btnName = "";
         if (_guiStyle == null) _guiStyle = GUI.skin.button;
         if (GUILayout.Button(_btnName, _guiStyle, _options))
         {
            _onClickEvent?.Invoke();
         }
      }

      public static void DrawButton(string _btnName, Vector2 _btnSize, Action onClickEvent)
      {
         if (_btnName == null) _btnName = "";
         if (Math.Abs(_btnSize.x) < 1 && Math.Abs(_btnSize.y) < 1)
         {
            if (GUILayout.Button(_btnName))
            {
               onClickEvent?.Invoke();
            }
         }
         else if (Math.Abs(_btnSize.x) < 1 && Math.Abs(_btnSize.y) > 1)
         {
            if (GUILayout.Button(_btnName, GUILayout.Height(_btnSize.y)))
            {
               onClickEvent?.Invoke();
            }
         }
         else if (Math.Abs(_btnSize.x) > 1 && Math.Abs(_btnSize.y) < 1)
         {
            if (GUILayout.Button(_btnName, GUILayout.Width(_btnSize.x)))
            {
               onClickEvent?.Invoke();
            }
         }
         else if (Math.Abs(_btnSize.x) > 1 && Math.Abs(_btnSize.y) > 1)
         {
            if (GUILayout.Button(_btnName, GUILayout.Width(_btnSize.x), GUILayout.Height(_btnSize.y)))
            {
               onClickEvent?.Invoke();
            }
         }
      } 

      public static void DrawButton(string _btnName, Vector2 _btnSize, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null)
      {
         if (_btnName == null) _btnName = "";
         if (_gridStyle == null) _gridStyle = GridGUIStyle;
         if (_btnStyle == null) _btnStyle = ButtonGUIStyle;
         GUILayout.BeginHorizontal(_gridStyle);  //GUILayout.Width(_size.x), GUILayout.Height(_size.y)
         if (_btnSize.x > 0 && _btnSize.y > 0)
         {
            if (GUILayout.Button(_btnName, _btnStyle, GUILayout.Width(_btnSize.x), GUILayout.Height(_btnSize.y)))
            {
               onClickEvent?.Invoke();
            }
         }
         else
         {
            if (GUILayout.Button(_btnName, _btnStyle))
            {
               onClickEvent?.Invoke();
            }
         }
         GUILayout.EndHorizontal();
      } 
        
      public static void DrawButton(string _btnName, RectOffset _rectOffset, Vector2 _size, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null)
      {
         if (_btnName == null) _btnName = "";
         if (_gridStyle == null) _gridStyle = GridGUIStyle;
         if (_btnStyle == null) _btnStyle = ButtonGUIStyle;
         GUILayout.Space(_rectOffset.top);
         GUILayout.BeginHorizontal(_gridStyle);  //GUILayout.Width(_size.x), GUILayout.Height(_size.y)
         GUILayout.Space(_rectOffset.left);
         if (GUILayout.Button(_btnName, _btnStyle, GUILayout.Width(_size.x), GUILayout.Height(_size.y), GUILayout.ExpandWidth(Math.Abs(_size.x) < 1), GUILayout.ExpandHeight(Math.Abs(_size.y) < 1)))
         {
            onClickEvent?.Invoke();
         }
         GUILayout.Space(_rectOffset.right);
         GUILayout.EndHorizontal();
         GUILayout.Space(_rectOffset.bottom);
      }

      public static void DrawTable()
      {
         GUILayout.BeginVertical();
            
         GUILayout.EndVertical();
      }

      public static void DrawGridVertical(Action delegateEvent, int _width = 0, int _height = 0)
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
        
      public static void DrawGridHorizontal(Action delegateEvent, int _width = 0, int _height = 0)
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

      private static GUIStyle GetGUIStyle_WH(int _width, int _height)
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


      private static GUIStyle GetLableStyle =>
         new GUIStyle
         {
            fixedWidth = 120,
            fixedHeight = 20,
            alignment = TextAnchor.MiddleRight,
            margin = new RectOffset(0, 0, 0, 0)
         };

      public static UnityEngine.Object[] DrawDragArea(Vector2 _size, string _content = "", params GUILayoutOption[] _options)
      {
         Event _event = Event.current;
         var dragArea = GUILayoutUtility.GetRect(_size.x, _size.y, _options);
         GUIContent title = new GUIContent(_content);
         GUI.Box(dragArea, title);
         List<Object> temps = new List<Object>();
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

      /// <summary>
      /// 运行项目的当前目录（在window下的 \ ）
      /// </summary>
      public static string CurrentDirectoryWin => System.Environment.CurrentDirectory;

      /// <summary>
      /// 运行项目的当前目录（在window下的 / ）
      /// </summary>
      public static string CurrentDirectoryUnity => System.Environment.CurrentDirectory.Replace(@"\", "/");
   }
   
#endif
}