using System;

namespace Game.UI
{
    public interface IProgressBar
    {
        void ShowProgressBar(float curIndex, float totalIndex, string title, string info, Action finishedEvent = null);
    }
}