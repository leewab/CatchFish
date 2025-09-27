using UnityEngine;

namespace Game.UI
{
    public interface IDrawLabel
    {
        /// <summary>
        /// 绘制标题
        /// </summary>
        /// <param name="_title"></param>
        /// <param name="_lableStyle"></param>
        void DrawTitle(string _title, GUIStyle _lableStyle = null);

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        void DrawLabels(string[] _names, Vector2 _size);

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        void DrawLabels(string[] _names, Vector2[] _sizes);

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        void DrawLabels(string[] _names, Vector2[] _sizes, GUIStyle[] _styles, GUIStyle _gridStyle);

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        void DrawLabel(string _name, GUIStyle _style = null, GUIStyle _gridStyle = null);

        /// <summary>
        /// 绘制Lable
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        void DrawLabel(string _name, Vector2 _size, GUIStyle _style = null, GUIStyle _gridStyle = null);

        /// <summary>
        /// 绘制Lable-lable
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_size"></param>
        /// <param name="_style"></param>
        void DrawLabel(string _name, Vector2 _nameSize, string _content, Vector2 _contentSize);
        
        void DrawLabel(GUIContent _guiContent, Vector2 _size, GUIStyle _style = null, GUIStyle _gridStyle = null);

        void DrawLabel(GUIContent _guiContent, Vector2 _nameSize, string _content, Vector2 _contentSize);
    }
}