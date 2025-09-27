using UnityEngine;

namespace Game.UI
{
    public class GameNumberText : GameUIComponent
    {
        public GameNumberText(string name, Transform father = null) : base(name, father)
        {
        }

        public GameNumberText(GameObject obj) : base(obj)
        {
        }
    }
}