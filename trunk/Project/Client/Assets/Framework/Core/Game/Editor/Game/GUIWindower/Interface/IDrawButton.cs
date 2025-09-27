using System;
using UnityEngine;

namespace Game.UI
{
    public interface IDrawButton
    {
        void DrawButton(GUIContent _btnGUIContent, Action _onClickEvent, GUIStyle _guiStyle = null, params GUILayoutOption[] _options);

        void DrawButton(GUIContent _btnGUIContent, Vector2 _btnSize, Action onClickEvent);

        void DrawButton(GUIContent _btnGUIContent, Vector2 _btnSize, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null);

        void DrawButton(GUIContent _btnGUIContent, RectOffset _rectOffset, Vector2 _size, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null);

        void DrawButton(string _btnName, Action _onClickEvent, GUIStyle _guiStyle = null, params GUILayoutOption[] _options);

        void DrawButton(string _btnName, Vector2 _btnSize, Action onClickEvent);

        void DrawButton(string _btnName, Vector2 _btnSize, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null);

        void DrawButton(string _btnName, RectOffset _rectOffset, Vector2 _size, Action onClickEvent, GUIStyle _gridStyle = null, GUIStyle _btnStyle = null);
    }
}