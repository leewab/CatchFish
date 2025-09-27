using System;
using UnityEngine;

namespace Game
{
    public abstract class PreloadProcess : IProcess
    {
        public string PreloadDesc { get; }
        public Action OnFinishedEvent { get; set; }
        public abstract void Start();
        protected void Log(string content)
        {
            Debug.Log("预加载 Start ===> " + content);
        }
        protected void Finish(string content)
        {
            Debug.Log("预加载 End ===> " + content);
            this.OnFinishedEvent?.Invoke();
            this.OnFinishedEvent = null;
        }
    }
}