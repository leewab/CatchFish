using System.Collections.Generic;

namespace Game
{
    public static class DotUtil
    {
        public static T Pop<T>(this List<T> list, bool isTop = false)
        {
            T data = default(T);
            int index = isTop ? 0 : list.Count - 1;
            if (index >= 0)
            {
                data = list[index];
                list.RemoveAt(index);
            }

            return data;
        }

        public static T Peek<T>(this List<T> list, bool isTop = false)
        {
            T data = default(T);
            int index = isTop ? 0 : list.Count - 1;
            if (index >= 0)
            {
                data = list[index];
            }

            return data;
        }
    }
}