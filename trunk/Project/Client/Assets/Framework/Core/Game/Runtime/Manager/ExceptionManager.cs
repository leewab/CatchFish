using System;

namespace Game
{
    /// <summary>
    /// 异常播报管理器
    ///     独立模块可以监听此管理器，接受异常播报
    /// </summary>
    public class ExceptionManager : Singleton<ExceptionManager>
    {
        public delegate void ExceptionAction(Exception e);
        public ExceptionAction ExceptionActionCast;
    }
    
}