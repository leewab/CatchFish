using System;

namespace Game.Core
{
    /// <summary>
    /// 对于Action注册多次的安全性封装
    ///  注意：注册最好不要使用匿名函数实现，匿名函数在每次注册过程中重新开启内存空间，所以，多次注册仍是累加注册
    /// </summary>
    public struct SafeAction
    {
        private Action mAction;

        public static implicit operator SafeAction(Action a)
        {
            SafeAction res = new SafeAction()
            {
                mAction = a
            };
            return res;
        }

        public static implicit operator Action(SafeAction sa)
        {
            return sa.mAction;
        }

        public static SafeAction operator -(SafeAction sa, Action a)
        {
            SafeAction res = new SafeAction()
            {
                mAction = sa.mAction - a
            };
            return res;
        }

        public static SafeAction operator +(SafeAction sa, Action a)
        {
            sa.mAction -= a;
            SafeAction res = new SafeAction()
            {
                mAction = sa.mAction + a
            };
            return res;
        }

        public void SafeInvoke()
        {
            mAction.SafeInvoke();
        }

        public void Dispose()
        {
            mAction = null;
        }

        public bool IsNull()
        {
            return mAction == null;
        }
    }


    public struct SafeAction<T>
    {
        private Action<T> mAction;

        public static implicit operator SafeAction<T>(Action<T> a)
        {
            SafeAction<T> res = new SafeAction<T>();
            res.mAction = a;
            return res;
        }

        public static implicit operator Action<T>(SafeAction<T> sa)
        {
            return sa.mAction;
        }

        public static SafeAction<T> operator +(SafeAction<T> sa, Action<T> a)
        {
            sa.mAction -= a;
            SafeAction<T> res = new SafeAction<T>();
            res.mAction = sa.mAction + a;
            return res;
        }

        public static SafeAction<T> operator -(SafeAction<T> sa, Action<T> a)
        {
            SafeAction<T> res = new SafeAction<T>();
            res.mAction = sa.mAction - a;
            return res;
        }

        public void SafeInvoke(T t)
        {
            mAction.SafeInvoke(t);
        }

        public void Dispose()
        {
            mAction = null;
        }

        public bool IsNull
        {
            get
            {
                return mAction == null;
            }
        }
    }


    public struct SafeAction<T1, T2>
    {
        private Action<T1, T2> mAction;

        public static implicit operator SafeAction<T1, T2>(Action<T1, T2> a)
        {
            SafeAction<T1, T2> res = new SafeAction<T1, T2>();
            res.mAction = a;
            return res;
        }

        public static implicit operator Action<T1, T2>(SafeAction<T1, T2> sa)
        {
            return sa.mAction;
        }

        public static SafeAction<T1, T2> operator +(SafeAction<T1, T2> sa, Action<T1, T2> a)
        {
            sa.mAction -= a;
            SafeAction<T1, T2> res = new SafeAction<T1, T2>();
            res.mAction = sa.mAction + a;
            return res;
        }

        public static SafeAction<T1, T2> operator -(SafeAction<T1, T2> sa, Action<T1, T2> a)
        {
            SafeAction<T1, T2> res = new SafeAction<T1, T2>();
            res.mAction = sa.mAction - a;
            return res;
        }

        public void SafeInvoke(T1 t1, T2 t2)
        {
            mAction.SafeInvoke(t1, t2);
        }

        public void Dispose()
        {
            mAction = null;
        }

        public bool IsNull
        {
            get
            {
                return mAction == null;
            }
        }
    }
    
    /// <summary>
    /// 能避免重复挂事件的默认委托
    /// </summary>
    public struct SafeAction<T, T2, T3>
    {
        Action<T, T2, T3> act;

        public static implicit operator Action<T, T2, T3>(SafeAction<T, T2, T3> a)
        {
            return a.act;
        }

        public static implicit operator SafeAction<T, T2, T3>(Action<T, T2, T3> a)
        {
            SafeAction<T, T2, T3> res = new SafeAction<T, T2, T3>();
            res.act = a;
            return res;
        }
        public static SafeAction<T, T2, T3> operator +(SafeAction<T, T2, T3> a, Action<T, T2, T3> b)
        {
            a.act -= b;
            SafeAction<T, T2, T3> res = new SafeAction<T, T2, T3>();
            res.act = a.act + b;

            return res;
        }

        public static SafeAction<T, T2, T3> operator -(SafeAction<T, T2, T3> a, Action<T, T2, T3> b)
        {
            SafeAction<T, T2, T3> res = new SafeAction<T, T2, T3>();
            res.act = a.act - b;
            return res;
        }

        public void SafeInvoke(T param, T2 param2, T3 param3)
        {
            act.SafeInvoke(param, param2, param3);
        }

        public bool IsNull
        {
            get
            {
                return act == null;
            }
        }

        public void Dispose()
        {
            act = null;
        }
    }
    
}