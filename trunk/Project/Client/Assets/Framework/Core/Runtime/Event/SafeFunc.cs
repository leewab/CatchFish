using System;

namespace Game.Core
{
    public struct SafeFunc<T>
    {
        private Func<T> mFunc;

        public static implicit operator SafeFunc<T>(Func<T> f)
        {
            SafeFunc<T> res = new SafeFunc<T>();
            res.mFunc = f;
            return res;
        }

        public static implicit operator Func<T>(SafeFunc<T> sf)
        {
            return sf.mFunc;
        }

        public static SafeFunc<T> operator +(SafeFunc<T> sf, Func<T> f)
        {
            sf.mFunc -= f;
            SafeFunc<T> res = new SafeFunc<T>();
            res.mFunc = sf.mFunc + f;
            return res;
        }

        public static SafeFunc<T> operator -(SafeFunc<T> sf, Func<T> f)
        {
            SafeFunc<T> res = new SafeFunc<T>();
            res.mFunc = sf.mFunc - f;
            return res;
        }

        public void SafeInvoke()
        {
            mFunc.SafeInvoke();
        }

        public void Dispose()
        {
            mFunc = null;
        }

        public bool IsNull
        {
            get
            {
                return mFunc == null;
            }
        }
    }

    public struct SafeFunc<T1, T2>
    {
        private Func<T1, T2> mFunc;

        public static implicit operator SafeFunc<T1, T2>(Func<T1, T2> f)
        {
            SafeFunc<T1, T2> res = new SafeFunc<T1, T2>();
            res.mFunc = f;
            return res;
        }

        public static implicit operator Func<T1, T2>(SafeFunc<T1, T2> sf)
        {
            return sf.mFunc;
        }

        public static SafeFunc<T1, T2> operator +(SafeFunc<T1, T2> sf, Func<T1, T2> f)
        {
            sf.mFunc -= f;
            SafeFunc<T1, T2> res = new SafeFunc<T1, T2>();
            res.mFunc = sf.mFunc + f;
            return res;
        }

        public static SafeFunc<T1, T2> operator -(SafeFunc<T1, T2> sf, Func<T1, T2> f)
        {
            SafeFunc<T1, T2> res = new SafeFunc<T1, T2>();
            res.mFunc = sf.mFunc - f;
            return res;
        }

        public void SafeInvoke(T1 t)
        {
            mFunc.SafeInvoke(t);
        }

        public void Dispose()
        {
            mFunc = null;
        }

        public bool IsNull
        {
            get
            {
                return mFunc == null;
            }
        }
    }

    public struct SafeFunc<T1, T2, T3>
    {
        private Func<T1, T2, T3> mFunc;

        public static implicit operator SafeFunc<T1, T2, T3>(Func<T1, T2, T3> f)
        {
            SafeFunc<T1, T2, T3> res = new SafeFunc<T1, T2, T3>();
            res.mFunc = f;
            return res;
        }

        public static implicit operator Func<T1, T2, T3>(SafeFunc<T1, T2, T3> sf)
        {
            return sf.mFunc;
        }

        public static SafeFunc<T1, T2, T3> operator +(SafeFunc<T1, T2, T3> sf, Func<T1, T2, T3> f)
        {
            sf.mFunc -= f;
            SafeFunc<T1, T2, T3> res = new SafeFunc<T1, T2, T3>();
            res.mFunc = sf.mFunc + f;
            return res;
        }

        public static SafeFunc<T1, T2, T3> operator -(SafeFunc<T1, T2, T3> sf, Func<T1, T2, T3> f)
        {
            SafeFunc<T1, T2, T3> res = new SafeFunc<T1, T2, T3>();
            res.mFunc = sf.mFunc - f;
            return res;
        }

        public void SafeInvoke(T1 t1, T2 t2)
        {
            mFunc.SafeInvoke(t1, t2);
        }

        public void Dispose()
        {
            mFunc = null;
        }

        public bool IsNull
        {
            get
            {
                return mFunc == null;
            }
        }
    }
}
