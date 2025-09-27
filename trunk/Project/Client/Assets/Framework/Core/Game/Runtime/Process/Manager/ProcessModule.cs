using System.Collections.Generic;

namespace Game
{
    public class ProcessModule
    {
        private static bool mIsCalling = false;
        private static Queue<IProcess> mProcessQueue = new Queue<IProcess>();
        
        public static void AddProcess(IProcess process)
        {
            if (mProcessQueue == null) mProcessQueue = new Queue<IProcess>();
            mProcessQueue.Enqueue(process);
            if (!mIsCalling) CallProcess();
        }

        private static void CallProcess()
        {
            if (mProcessQueue == null || mProcessQueue.Count == 0)
            {
                mIsCalling = false;
                return;
            }
            
            mIsCalling = true;
            var process = mProcessQueue.Dequeue();
            if (process == null) return;
            process.OnFinishedEvent = CallProcess;
            process.Start();
        }

    }
}