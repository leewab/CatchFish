using UnityEngine;

namespace Game.UI
{
    public interface IToggle
    {
        bool DrawToggle(string name, bool value);
        bool DrawToggle(GUIContent guiContent, bool value);
    }
}