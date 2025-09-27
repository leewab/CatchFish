using UnityEngine;

namespace Game.UI
{
    public interface IDrawField
    {
        string DrawField(GUIContent _nameGuiContent, string _field, Vector2 _nameSize, Vector2 _contentSize, int _contentMaxLength = 100, bool _isHorizontal = true);
        string DrawField(GUIContent _guiContent, string _field, RectOffset _rectOffset, Vector2 _size, GUIStyle _borderStyle = null, GUIStyle _lableStyle = null, GUIStyle _fieldStyle = null);
        string DrawField(string _name, string _field, Vector2 _nameSize, Vector2 _contentSize, int _contentMaxLength = 100, bool _isHorizontal = true);
        string DrawField(string _name, string _field, RectOffset _rectOffset, Vector2 _size, GUIStyle _borderStyle = null, GUIStyle _lableStyle = null, GUIStyle _fieldStyle = null);
    }
}