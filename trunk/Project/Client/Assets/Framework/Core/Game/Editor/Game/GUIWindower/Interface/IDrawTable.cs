using UnityEngine;

namespace Game.UI
{
    public interface IDrawTable
    {
        UnityEngine.Object[] DrawDragArea(UnityEngine.Vector2 size, string content = "", params UnityEngine.GUILayoutOption[] options);
        UnityEngine.Object[] DrawDragArea(UnityEngine.Vector2 size, GUIContent guiContent, params UnityEngine.GUILayoutOption[] options);
    }
}