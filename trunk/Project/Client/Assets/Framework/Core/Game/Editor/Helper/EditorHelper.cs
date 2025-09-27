using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.UI
{
#if UNITY_EDITOR
   public static class EditorHelper
   {
      private static BaseDrawGUI _mBaseDrawGui = new DrawGUI();

      #region GUIStyle

      public static GUIStyle BoxGUIStyle => _mBaseDrawGui.BoxGUIStyle;

      public static GUIStyle LableGUIStyle => _mBaseDrawGui.LableGUIStyle;

      public static GUIStyle ButtonGUIStyle => _mBaseDrawGui.ButtonGUIStyle;

      public static GUIStyle BtnToggleGUIStyle => _mBaseDrawGui.BtnToggleGUIStyle;

      public static GUIStyle GridGUIStyle => _mBaseDrawGui.GridGUIStyle;

      public static GUIStyle TitleGUIStyle => _mBaseDrawGui.TitleGUIStyle;
      
      public static GUIStyle GetGUIStyle_WH(int _width, int _height) { return _mBaseDrawGui.GetGUIStyle_WH(_width, _height); }

      #endregion
      
      #region DrawLabel

      /// <summary>
      /// 绘制标题
      /// </summary>
      /// <param name="_title"></param>
      /// <param name="_lableStyle"></param>
      public static void DrawTitle(string _title, GUIStyle _lableStyle = null)
      {
         _mBaseDrawGui.DrawTitle(_title, _lableStyle);
      }
      
      /// <summary>
      /// 绘制Label
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLabels(string[] _names, Vector2 _size)
      {
         _mBaseDrawGui.DrawLabels(_names, _size);
      }
      
      /// <summary>
      /// 绘制Label
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLabels(string[] _names, Vector2[] _sizes)
      {
         _mBaseDrawGui.DrawLabels(_names, _sizes);
      }
        
      /// <summary>
      /// 绘制Lable
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLabels(string[] _names, Vector2[] _sizes, GUIStyle[] _styles, GUIStyle _gridStyle)
      {
         _mBaseDrawGui.DrawLabels(_names, _sizes, _styles, _gridStyle);
      }
      
      /// <summary>
      /// 绘制Label
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLabel(string _name, GUIStyle _style = null, GUIStyle _gridStyle = null)
      {
         _mBaseDrawGui.DrawLabel(_name, _style, _gridStyle);
      }

      /// <summary>
      /// 绘制Label
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLabel(string _name, Vector2 _size, GUIStyle _style = null, GUIStyle _gridStyle = null)
      {
         _mBaseDrawGui.DrawLabel(_name, _size, _style, _gridStyle);
      }
      
      /// <summary>
      /// 绘制Label-label
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_size"></param>
      /// <param name="_style"></param>
      public static void DrawLabel(string _name, Vector2 _nameSize, string _content, Vector2 _contentSize)
      {
         _mBaseDrawGui.DrawLabel(_name, _nameSize, _content, _contentSize);
      }

      #endregion

      #region DrawButton

      public static void DrawButton(GUIContent _btnGUIContent, Action _onClickEvent, GUIStyle _guiStyle = null, params GUILayoutOption[] _options)
      {
         _mBaseDrawGui.DrawButton(_btnGUIContent, _onClickEvent, _guiStyle, _options);
      }
      
      public static void DrawButton(GUIContent _btnGUIContent, Vector2 _btnSize, Action _onClickEvent)
      {
         _mBaseDrawGui.DrawButton(_btnGUIContent, _btnSize, _onClickEvent);
      } 

      public static void DrawButton(GUIContent _btnGUIContent, Vector2 _btnSize, Action _onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null)
      {
         _mBaseDrawGui.DrawButton(_btnGUIContent, _btnSize, _onClickEvent, _gridStyle, _btnStyle);
      } 
        
      public static void DrawButton(GUIContent _btnGUIContent, RectOffset _rectOffset, Vector2 _size, Action _onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null)
      {
         _mBaseDrawGui.DrawButton(_btnGUIContent, _rectOffset, _size, _onClickEvent, _gridStyle, _btnStyle);
      }
      
      public static void DrawButton(string _btnName, Action _onClickEvent, GUIStyle _guiStyle = null, params GUILayoutOption[] _options)
      {
         _mBaseDrawGui.DrawButton(_btnName, _onClickEvent, _guiStyle, _options);
      }

      public static void DrawButton(string _btnName, Vector2 _btnSize, Action _onClickEvent)
      {
         _mBaseDrawGui.DrawButton(_btnName, _btnSize, _onClickEvent);
      } 

      public static void DrawButton(string _btnName, Vector2 _btnSize, Action _onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null)
      {
         _mBaseDrawGui.DrawButton(_btnName, _btnSize, _onClickEvent, _gridStyle, _btnStyle);
      } 
        
      public static void DrawButton(string _btnName, RectOffset _rectOffset, Vector2 _size, Action _onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null)
      {
         _mBaseDrawGui.DrawButton(_btnName, _rectOffset, _size, _onClickEvent, _gridStyle, _btnStyle);
      }

      #endregion

      #region DrawField

      /// <summary>
      /// 集成InputField，包含TextField TextArea 当无多行特性是使用TextField 当有多行特性时使用TextArea 
      /// </summary>
      /// <param name="_name"></param>
      /// <param name="_field"></param>
      /// <param name="_nameSize"></param>
      /// <param name="_contentSize"></param>
      /// <param name="_contentMaxLength"></param>
      /// <param name="_isHorizontal"></param>
      /// <returns></returns>
      public static string DrawField(string _name, string _field, Vector2 _nameSize, Vector2 _contentSize, int _contentMaxLength = 100, bool _isHorizontal = true)
      {
         return _mBaseDrawGui.DrawField(_name, _field, _nameSize, _contentSize, _contentMaxLength, _isHorizontal);
      }

      public static string DrawField(string _name, string _field, RectOffset _rectOffset,Vector2 _size, GUIStyle _borderStyle = null, GUIStyle _lableStyle = null, GUIStyle _fieldStyle = null)
      {
         return _mBaseDrawGui.DrawField(_name, _field, _rectOffset, _size, _borderStyle, _lableStyle, _fieldStyle);
      }
      
      public static string DrawField(GUIContent _nameGuiContent, string _field, Vector2 _nameSize, Vector2 _contentSize, int _contentMaxLength = 100, bool _isHorizontal = true)
      {
         return _mBaseDrawGui.DrawField(_nameGuiContent, _field, _nameSize, _contentSize, _contentMaxLength, _isHorizontal);
      }

      public static string DrawField(GUIContent _nameGuiContent, string _field, RectOffset _rectOffset,Vector2 _size, GUIStyle _borderStyle = null, GUIStyle _lableStyle = null, GUIStyle _fieldStyle = null)
      {
         return _mBaseDrawGui.DrawField(_nameGuiContent, _field, _rectOffset, _size, _borderStyle, _lableStyle, _fieldStyle);
      }

      #endregion

      #region DrawGrid

      public static void DrawGridVertical(Action delegateEvent, int _width = 0, int _height = 0)
      {
         _mBaseDrawGui.DrawGridVertical(delegateEvent, _width, _height);
      }
        
      public static void DrawGridHorizontal(Action delegateEvent, int _width = 0, int _height = 0)
      {
         _mBaseDrawGui.DrawGridHorizontal(delegateEvent, _width, _height);
      }

      #endregion

      #region DrawProgressBar

      public static void ShowProgressBar(float curIndex, float totalIndex, string title, string info, Action finishedEvent = null)
      {
         _mBaseDrawGui.ShowProgressBar(curIndex, totalIndex, title, info, finishedEvent);
      }

      #endregion

      #region DrawToggle

      public static bool DrawToggle(string name, bool value)
      {
         return _mBaseDrawGui.DrawToggle(name, value);
      }
      
      public static bool DrawToggle(GUIContent guiContent, bool value)
      {
         return _mBaseDrawGui.DrawToggle(guiContent, value);
      }

      #endregion

      #region DrawToolbar

      /// <summary>
      /// 按钮页签式 Unity自带的Toolbar 只能水平分布
      /// </summary>
      /// <param name="_names"></param>
      /// <param name="_selected"></param>
      /// <param name="_nameSize"></param>
      /// <returns></returns>
      public static int DrawToolbar(string[] _names, int _selected, Vector2 _nameSize)
      {
         return _mBaseDrawGui.DrawToolbar(_names, _selected, _nameSize);
      }
      
      public static int DrawToolbar(GUIContent[] _guiContents, int _selected, Vector2 _nameSize)
      {
         return _mBaseDrawGui.DrawToolbar(_guiContents, _selected, _nameSize);
      }
      
      /// <summary>
      /// 按钮页签式（解决了页（Toolbar）签式分类无法竖直排列的问题）
      /// </summary>
      /// <param name="_names"></param>
      /// <param name="_nameSize"></param>
      public static int DrawToolbar(string[] _names, int _selected, Vector2 _nameSize, int _space, bool _isHorizontal = false)
      {
         return _mBaseDrawGui.DrawToolbar(_names, _selected, _nameSize, _space, _isHorizontal);
      }
      
      public static int DrawToolbar(GUIContent[] _guiContents, int _selected, Vector2 _nameSize, int _space, bool _isHorizontal = false)
      {
         return _mBaseDrawGui.DrawToolbar(_guiContents, _selected, _nameSize, _space, _isHorizontal);
      }

      #endregion

      #region DrawArea

      public static UnityEngine.Object[] DrawDragArea(Vector2 _size, string _content = "", params GUILayoutOption[] _options)
      {
         return _mBaseDrawGui.DrawDragArea(_size, _content, _options);
      }
      
      public static UnityEngine.Object[] DrawDragArea(Vector2 _size, GUIContent guiContent, params GUILayoutOption[] _options)
      {
         return _mBaseDrawGui.DrawDragArea(_size, guiContent, _options);
      }

      #endregion
      
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