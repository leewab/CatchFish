using UnityEngine;

namespace Game.UI
{
    public interface ICanvasMaker
    {
        Canvas Canvas { get; }
        
        int SortingOrder { get; set; }
    }
}