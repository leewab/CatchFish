using System;

namespace Game.UI
{
    public interface IDrawGrid
    {
        void DrawGridVertical(Action delegateEvent, int _width = 0, int _height = 0);
        void DrawGridHorizontal(Action delegateEvent, int _width = 0, int _height = 0);
    }
}