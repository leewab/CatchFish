using System;

namespace Game.Core
{
    /// <summary>
    /// 异常处理类，用于各种异常的处理
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// 异常通知委托
        /// </summary>
        public delegate void ExceptionAction(Exception e);

        /// <summary>
        /// 异常通知委托
        /// </summary>
        public delegate void ExceptionActionString(string eStr);

        /// <summary>
        /// 类型检查 抛出空对象异常
        /// </summary>
        /// <param name="t">类型对象</param>
        /// <param name="paramName">参数检查名称</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        public static T ThrowNullException<T>(this T t)
        {
            if (t == null)
                throw new ArgumentNullException($"{typeof(T).FullName}", "Value can not be null!!!");
            return t;
        }

        /// <summary>
        /// 字符串为空或null异常
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ThrowEmptyOrNullException(this string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException($"string 字段", "Value can not be null or empty!!!");
        }


        /// <summary>
        /// 注册Lua抛出事件
        /// </summary>
        public static ExceptionActionString LuaExceptionActionString;

        /// <summary>
        /// 异常抛出广播
        /// </summary>
        /// <param name="exceptionStr"></param>
        public static void ThrowExceptionToBroadcast(string exceptionStr)
        {
            // Lua模块监听事件
            LuaExceptionActionString?.Invoke(exceptionStr);
        }

    }
}
