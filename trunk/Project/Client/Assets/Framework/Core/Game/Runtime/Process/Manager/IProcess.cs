using System;

namespace Game
{
    public interface IProcess
    {
        public string PreloadDesc { get; }
        public Action OnFinishedEvent { get; set; }
        public abstract void Start();
    }
}