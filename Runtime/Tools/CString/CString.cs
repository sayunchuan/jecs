using System;

namespace JECS.Tools
{
    public struct CString
    {
        private string str;
        private readonly int off;
        private readonly int len;
        private int hash;

        public int Length => len;

        public CString(string m) : this(m, 0, m.Length)
        {
        }

        public CString(string m, int off, int len)
        {
            str = m;
            this.off = off;
            this.len = len;
            hash = 0;
        }

        public char this[int index] => str[off + index];

        /// <summary>
        /// 分隔字符串，并返回分隔符位置，若未分隔则返回-1
        /// </summary>
        public int Split(char separator, out CString first, out CString other)
        {
            for (int i = 0, index = off; i < len; i++, index++)
            {
                if (str[index] == separator)
                {
                    first = new CString(str, off, i);
                    other = new CString(str, index + 1, len - i - 1);
                    return i;
                }
            }

            first = new CString(str, off, len);
            other = new CString("", 0, 0);
            return -1;
        }

        public CStringIte Split(char separator, StringSplitOptions options = StringSplitOptions.None)
        {
            return new CStringIte(this, separator, options);
        }

        public bool StartsWith(string value)
        {
            if (len < value.Length) return false;

            for (int i = 0, imax = value.Length; i < imax; i++)
            {
                if (str[off + i] != value[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool StartsWith(CString value)
        {
            if (len < value.Length) return false;

            for (int i = 0, imax = value.Length; i < imax; i++)
            {
                if (str[off + i] != value[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool EndsWith(string value)
        {
            if (len < value.Length) return false;

            for (int i = value.Length - 1, index = off + len - 1; i >= 0; i--, index--)
            {
                if (str[index] != value[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool EndsWith(CString value)
        {
            if (len < value.Length) return false;

            for (int i = value.Length - 1, index = off + len - 1; i >= 0; i--, index--)
            {
                if (str[index] != value[i])
                {
                    return false;
                }
            }

            return true;
        }

        public string Substring(int startIndex)
        {
            return str.Substring(startIndex + off, len - startIndex);
        }

        public string Substring(int startIndex, int length)
        {
            return str.Substring(startIndex + off, length);
        }

        public CString Subcstring(int startIndex)
        {
            return new CString(str, off + startIndex, len - startIndex);
        }

        public CString Subcstring(int startIndex, int length)
        {
            return new CString(str, startIndex + off, length);
        }

        public CString Trim()
        {
            return TrimEnd().TrimStart();
        }

        public CString TrimStart()
        {
            for (int i = off, imax = off + len; i < imax; i++)
            {
                if (str[i] != ' ')
                {
                    return new CString(str, i, len - i + off);
                }
            }

            return new CString(str, off + len - 1, 0);
        }

        public CString TrimEnd()
        {
            for (int i = off + len - 1; i >= off; i--)
            {
                if (str[i] != ' ')
                {
                    return new CString(str, off, i - off + 1);
                }
            }

            return new CString(str, off, 0);
        }

        public int IndexOf(char value) => IndexOf(value, 0, Length);

        public int IndexOf(char value, int startIndex) => IndexOf(value, startIndex, Length - startIndex);

        public int IndexOf(char value, int startIndex, int count)
        {
            if (startIndex < 0 || startIndex > len)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Cannot be negative and must be< 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "< 0");
            if (startIndex > this.len - count)
                throw new ArgumentOutOfRangeException(nameof(count), "startIndex + count > this.m_stringLength");
            return startIndex == 0 && len == 0 || startIndex == len || count == 0
                ? -1
                : str.IndexOf(value, startIndex + off, count);
        }

        public static bool SamePrefix(CString s1, CString s2, out CString prefix)
        {
            int minLength = s1.Length < s2.Length ? s1.Length : s2.Length;
            for (int i = 0; i < minLength; i++)
            {
                if (s1[i] != s2[i])
                {
                    if (i == 0)
                    {
                        prefix = new CString("", 0, 0);
                        return false;
                    }

                    prefix = s1.Subcstring(0, i);
                    return true;
                }
            }

            prefix = s1.Subcstring(0, minLength);
            return true;
        }

        public static bool ValidParamName(CString s)
        {
            if (s.len <= 0) return false;

            char tmp = s[0];
            return tmp == '_' || tmp == '@' || tmp == '$' || (tmp >= 'a' && tmp <= 'z') || (tmp >= 'A' && tmp <= 'Z');
        }

        #region operator

        public static implicit operator string(CString x)
        {
            return x.ToString();
        }

        public static implicit operator CString(string x)
        {
            return new CString(x);
        }

        public static bool operator ==(CString a1, CString a2)
        {
            return a1.Equals(a2);
        }

        public static bool operator !=(CString a1, CString a2)
        {
            if (a1.Length != a2.Length)
            {
                return true;
            }

            for (int i = 0, imax = a1.Length; i < imax; i++)
            {
                if (a1[i] != a2[i])
                {
                    return true;
                }
            }

            return false;
        }

        public static bool operator ==(CString a1, string a2)
        {
            return a2 != null && a1 == new CString(a2);
        }

        public static bool operator !=(CString a1, string a2)
        {
            return a2 == null || a1 != new CString(a2);
        }

        public static bool operator ==(string a1, CString a2)
        {
            return a1 != null && a2 == new CString(a1);
        }

        public static bool operator !=(string a1, CString a2)
        {
            return a1 == null || a2 != new CString(a1);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is CString chs) return Equals(chs);
            return false;
        }

        public bool Equals(CString other)
        {
            if (Length != other.Length) return false;

            for (int i = 0, imax = Length; i < imax; i++)
            {
                if (this[i] != other[i]) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (hash != 0 || len <= 0) return hash;

            for (int i = 0; i < len; i++) hash = 31 * hash + this[i];
            return hash;
        }

        public override string ToString()
        {
            return str.Substring(off, len);
        }

        #region Parse

        public bool TryParseByte(out byte res)
        {
            res = 0;
            int l = str.Length;
            if (str[l - 1] != '\'')
            {
                return false;
            }

            if (l == 3)
            {
                if (str[1] == '\"' || str[1] == '\'')
                {
                    return false;
                }

                res = (byte)str[1];
                return true;
            }

            if (l == 4)
            {
                if (str[1] != '\\')
                {
                    return false;
                }

                switch (str[2])
                {
                    case 'a':
                        res = (byte)'\a';
                        return true;
                    case 'b':
                        res = (byte)'\b';
                        return true;
                    case 'f':
                        res = (byte)'\f';
                        return true;
                    case 'n':
                        res = (byte)'\n';
                        return true;
                    case 'r':
                        res = (byte)'\r';
                        return true;
                    case 't':
                        res = (byte)'\t';
                        return true;
                    case 'v':
                        res = (byte)'\v';
                        return true;
                    case '\\':
                        res = (byte)'\\';
                        return true;
                    case '\'':
                        res = (byte)'\'';
                        return true;
                    case '\"':
                        res = (byte)'\"';
                        return true;
                    case '0':
                        res = (byte)'\0';
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        public int ParseByte()
        {
            byte res;
            if (TryParseByte(out res))
            {
                return res;
            }

            throw new OverflowException();
        }

        public bool TryParseInt32(out int res)
        {
            if (len <= 0)
            {
                res = 0;
                return false;
            }

            res = 0;
            int index = off;
            int end = off + len;
            if (str[index] == '0')
            {
                if (len == 1)
                {
                    res = 0;
                    return true;
                }

                index++;

                if (str[index] == 'x' || str[index] == 'X')
                {
                    // 解析16进制
                    if (len < 3)
                    {
                        res = 0;
                        return false;
                    }

                    index++;
                    while (index < end)
                    {
                        char c = str[index];
                        int tmp = _ToNum_F(c);
                        if (tmp < 0)
                        {
                            res = 0;
                            return false;
                        }

                        res = (res << 4) + tmp;
                        index++;
                    }
                }
                else
                {
                    // 解析8进制
                    while (index < end)
                    {
                        char c = str[index];
                        int tmp = _ToNum_8(c);
                        if (tmp < 0)
                        {
                            res = 0;
                            return false;
                        }

                        res = (res << 3) + tmp;
                        index++;
                    }
                }
            }
            else
            {
                int flag;
                flag = str[index] == '-' ? -1 : 1;
                if (str[index] == '-')
                {
                    index++;
                }

                while (index < end)
                {
                    char c = str[index];
                    if (c >= '0' && c <= '9')
                    {
                        index++;
                        res = res * 10 + _ToNum(c);
                    }
                    else
                    {
                        res = 0;
                        return false;
                    }
                }

                res *= flag;
            }

            return true;
        }

        public int ParseInt32()
        {
            int res;
            if (TryParseInt32(out res))
            {
                return res;
            }

            throw new OverflowException();
        }

        public bool TryParseInt64(out long res)
        {
            if (len <= 0)
            {
                res = 0;
                return false;
            }

            res = 0;
            int index = off;
            int end = off + len;
            if (str[index] == '0')
            {
                if (len == 1)
                {
                    res = 0;
                    return true;
                }

                index++;

                if (str[index] == 'x' || str[index] == 'X')
                {
                    // 解析16进制
                    if (len < 3)
                    {
                        res = 0;
                        return false;
                    }

                    index++;
                    while (index < end)
                    {
                        char c = str[index];
                        int tmp = _ToNum_F(c);
                        if (tmp < 0)
                        {
                            res = 0;
                            return false;
                        }

                        res = (res << 4) + tmp;
                        index++;
                    }
                }
                else
                {
                    // 解析8进制
                    while (index < end)
                    {
                        char c = str[index];
                        int tmp = _ToNum_8(c);
                        if (tmp < 0)
                        {
                            res = 0;
                            return false;
                        }

                        res = (res << 3) + tmp;
                        index++;
                    }
                }
            }
            else
            {
                int flag;
                flag = str[index] == '-' ? -1 : 1;
                if (str[index] == '-')
                {
                    index++;
                }

                while (index < end)
                {
                    char c = str[index];
                    if (c >= '0' && c <= '9')
                    {
                        index++;
                        res = res * 10 + _ToNum(c);
                    }
                    else
                    {
                        res = 0;
                        return false;
                    }
                }

                res *= flag;
            }

            return true;
        }

        public long ParseInt64()
        {
            long res;
            if (TryParseInt64(out res))
            {
                return res;
            }

            throw new OverflowException();
        }

        public bool TryParseDouble(out double res)
        {
            double val = 0, power = 1;
            int flag = 1;

            int index = 0;
            //跳过空格
            while (index < len && this[index] == ' ') index++;

            if (index >= len) goto PARSE_DOUBLE_ERROR;

            flag = this[index] == '-' ? -1 : 1;
            if (this[index] == '-' || this[index] == '+')
            {
                index++;
            }

            while (index < len)
            {
                char c = this[index];
                if (c >= '0' && c <= '9')
                {
                    val = val * 10 + c - '0';
                    index++;
                    continue;
                }

                break;
            }

            if (index < len && this[index] == '.') index++;

            while (index < len)
            {
                char c = this[index];
                if (c >= '0' && c <= '9')
                {
                    val = val * 10 + c - '0';
                    power *= 10;
                    index++;
                    continue;
                }

                break;
            }

            val /= power;

            if (index != len) goto PARSE_DOUBLE_ERROR;

            res = flag * val;
            return true;

            PARSE_DOUBLE_ERROR:
            res = 0;
            return false;
        }

        public double ParseDouble()
        {
            double res;
            if (TryParseDouble(out res))
            {
                return res;
            }

            throw new OverflowException();
        }

        private int _ToNum(char c)
        {
            return c - '0';
        }

        private int _ToNum_8(char c)
        {
            return c >= '0' && c <= '7' ? c - '0' : -1;
        }

        private int _ToNum_F(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            return -1;
        }

        public static int Parse_Byte(string str)
        {
            return new CString(str).ParseByte();
        }

        public static int Parse_Int32(string str)
        {
            return new CString(str).ParseInt32();
        }

        public static long Parse_Int64(string str)
        {
            return new CString(str).ParseInt64();
        }

        public static double Parse_Double(string str)
        {
            return new CString(str).ParseDouble();
        }

        #endregion
    }
}