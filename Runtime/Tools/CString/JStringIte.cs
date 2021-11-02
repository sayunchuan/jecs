using System;

namespace JECS.Tools
{
    public struct JStringIte
    {
        /// <summary>
        /// 目标字符串
        /// </summary>
        private JString s;

        /// <summary>
        /// 分隔选项
        /// </summary>
        private StringSplitOptions opt;

        /// <summary>
        /// 分隔符
        /// </summary>
        private char c;

        private int b;
        private int e;

        private int currIndex;

        public JStringIte(JString str, char separator, StringSplitOptions options = StringSplitOptions.None)
        {
            s = str;
            opt = options;
            c = separator;
            b = 0;
            e = -1;
            currIndex = -1;
        }

        public JString Current => s.Subcstring(b + 1, e - b - 1);

        public bool MoveNext()
        {
            b = e;
            for (e = b + 1; e < s.Length; e++)
            {
                if (s[e] == c)
                {
                    if (opt == StringSplitOptions.RemoveEmptyEntries && e - b == 1)
                    {
                        b = e;
                    }
                    else
                    {
                        currIndex++;
                        return true;
                    }
                }
            }

            currIndex++;
            return opt == StringSplitOptions.RemoveEmptyEntries ? b < s.Length - 1 : b < s.Length;
        }

        /// <summary>
        /// 以数组下标形式读取，但是仅能够正向读取数组内元素
        /// </summary>
        public JString this[int id]
        {
            get
            {
                if (id < currIndex)
                {
                    throw new OverflowException();
                }

                while (currIndex < id)
                {
                    if (!MoveNext())
                    {
                        throw new OverflowException();
                    }
                }

                return Current;
            }
        }
    }
}