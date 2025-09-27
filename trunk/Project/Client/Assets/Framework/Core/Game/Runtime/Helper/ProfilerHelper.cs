using System;

namespace Game
{
    public class ProfilerHelper
    {

        public static void BeginSample(string name)
        {
            if (UnityEngine.Profiling.Profiler.enabled)
                UnityEngine.Profiling.Profiler.BeginSample(name);
        }

        public static void EndSample()
        {
            if (UnityEngine.Profiling.Profiler.enabled)
                UnityEngine.Profiling.Profiler.EndSample();
        }
        
        
    }
}