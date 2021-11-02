using System.Collections.Generic;
using System.Text;

namespace JECS
{
    public static class StringBuilderPool
    {
        private static Queue<StringBuilder> __pool = new Queue<StringBuilder>();

        public static StringBuilder Spawn(int minimunCapacity = 16)
        {
            StringBuilder ret;
            if (__pool.Count <= 0)
            {
                ret = new StringBuilder(minimunCapacity);
            }
            else
            {
                ret = __pool.Dequeue();
            }

            if (ret.Capacity <= minimunCapacity)
            {
                ret.Capacity = minimunCapacity;
            }

            return ret;
        }

        public static string ReleaseRet(StringBuilder ptr, bool clear = true)
        {
            string res = ptr.ToString();
            Release(ptr, clear);
            return res;
        }

        public static void Release(StringBuilder ptr, bool clear = true)
        {
            if (clear)
            {
                ptr.Clear();
            }

            __pool.Enqueue(ptr);
        }
    }
}