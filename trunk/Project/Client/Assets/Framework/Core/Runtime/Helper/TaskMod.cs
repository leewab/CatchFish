using System;
using MUEngine;

namespace Game.Core
{
    public class TaskMod : Singleton<TaskMod>
    {
        private TaskArgs mTmpArgs = new TaskArgs();

        /// <summary>
        /// 创建一个task
        /// </summary>
        /// <param name="func">Func.</param>
        public void CreateTask(Action func)
        {
            ITaskMgr task_mgr = MUEngine.MURoot.TaskMgr;
            task_mgr.AddTask(func);
        }
        
        public void CreateTask( Action<object[]> func, TaskArgs args )
        {
            ITaskMgr task_mgr = MUEngine.MURoot.TaskMgr;
            task_mgr.AddTask(func, args);
        }
        
        public TaskArgs GetTempArgs()
        {
            mTmpArgs.Clear();
            return mTmpArgs;
        }
		
		public void Release()
		{
            mTmpArgs.Clear();
		}
        
	}
    
}

