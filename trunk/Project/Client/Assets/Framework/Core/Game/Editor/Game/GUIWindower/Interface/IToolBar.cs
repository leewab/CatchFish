
using UnityEngine;

namespace Game.UI
{
    public interface IToolBar
    {
        int DrawToolbar(string[] names, int selected, UnityEngine.Vector2 nameSize);
        int DrawToolbar(string[] names, int selected, Vector2 nameSize, int space, bool isHorizontal = false);
        int DrawToolbar(GUIContent[] guiContents, int selected, UnityEngine.Vector2 nameSize);
        int DrawToolbar(GUIContent[] guiContents, int selected, Vector2 nameSize, int space, bool isHorizontal = false);
    }
}