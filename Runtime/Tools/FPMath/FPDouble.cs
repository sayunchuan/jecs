using System;

namespace JECS.Tools
{
    /// <summary>
    /// 定点数类型
    /// </summary>
    public struct FPDouble : IEquatable<FPDouble>, IComparable<FPDouble>, IFormattable
    {
        private const int E = 16;
        private const long FP_One = 1L << E;
        private const long FP_Mask = ~(-1L << E);

        internal const long FP_PI = 205887L;
        internal const long FP_2_PI = FP_PI << 1;
        internal const long FP_PI_DIV_2 = 102943L;
        internal const long FP_PI_DIV_4 = 51471L;
        internal const long FP_1_DIV_PI = 20861L;

        public static readonly FPDouble Zero = 0;
        public static readonly FPDouble One = new FPDouble(FP_One);
        public static readonly FPDouble MaxValue = new FPDouble(long.MaxValue);
        public static readonly FPDouble MinValue = new FPDouble(long.MinValue);

        internal long _m;

        internal FPDouble(long memory)
        {
            _m = memory;
        }

        public static FPDouble Int64BitsToDouble(long l)
        {
            return new FPDouble(l);
        }

        public static long DoubleToInt64Bits(FPDouble d)
        {
            return d._m;
        }

        public static FPDouble Abs(FPDouble x)
        {
            x._m = x._m > 0 ? x._m : -x._m;
            return x;
        }

        public static FPDouble Max(FPDouble x, FPDouble y)
        {
            return x > y ? x : y;
        }

        public static FPDouble Min(FPDouble x, FPDouble y)
        {
            return x < y ? x : y;
        }

        public static FPDouble Ceiling(FPDouble v)
        {
            v._m = ((v._m + FP_Mask) >> E) << E;
            return v;
        }

        public static FPDouble Floor(FPDouble v)
        {
            v._m = (v._m >> E) << E;
            return v;
        }

        public static FPDouble Clamp(FPDouble x, FPDouble lo, FPDouble hi)
        {
            return x < lo ? lo : x > hi ? hi : x;
        }

        public static FPDouble Clamp01(FPDouble x)
        {
            return x < Zero ? Zero : x > One ? One : x;
        }

        public static FPDouble Sqrt(FPDouble num)
        {
            if (num._m < 0)
            {
                throw new Exception("Can't sqrt a num which smaller than zero.");
            }

            num._m = (long)Math.Sqrt(num._m << E);
            return num;
        }

        /* some sqrt methods which could be selected 
         
        private static long Sqrt1(long baseValue)
        {
            long x = baseValue;
            long val = baseValue >> 1; //初始值
            long last;
            do
            {
                last = val;
                val = (val >> 1) + (x >> 1) / val;
            } while (_abs(last - val) > 1);

            return val;
        }

        private static long _abs(long a)
        {
            return a > 0 ? a : -a;
        }

        private static ulong Sqrt2(ulong a)
        {
            ulong num = 0uL;
            ulong num2 = 0uL;
            for (int i = 0; i < 32; i++)
            {
                num2 <<= 1;
                num <<= 2;
                num += a >> 62;
                a <<= 2;
                if (num2 < num)
                {
                    num2 += 1uL;
                    num -= num2;
                    num2 += 1uL;
                }
            }

            return num2 >> 1 & 0xffffffffu;
        }
        */

        // operator functions
        public static FPDouble operator +(FPDouble x, FPDouble y)
        {
            x._m += y._m;
            return x;
        }

        public static FPDouble operator +(FPDouble x, long y)
        {
            x._m += y << E;
            return x;
        }

        public static FPDouble operator +(long x, FPDouble y)
        {
            y._m = (x << E) + y._m;
            return y;
        }

        public static FPDouble operator -(FPDouble x)
        {
            x._m = -x._m;
            return x;
        }

        public static FPDouble operator -(FPDouble x, FPDouble y)
        {
            x._m -= y._m;
            return x;
        }

        public static FPDouble operator -(FPDouble x, long y)
        {
            x._m -= y << E;
            return x;
        }

        public static FPDouble operator -(long x, FPDouble y)
        {
            y._m = (x << E) - y._m;
            return y;
        }

        public static FPDouble operator *(FPDouble x, FPDouble y)
        {
            x._m = (x._m * y._m) >> E;
            return x;
        }

        public static FPDouble operator *(FPDouble x, long y)
        {
            x._m *= y;
            return x;
        }

        public static FPDouble operator *(long x, FPDouble y)
        {
            y._m *= x;
            return y;
        }

        public static FPDouble operator /(FPDouble x, FPDouble y)
        {
            x._m = (x._m << E) / y._m;
            return x;
        }

        public static FPDouble operator /(FPDouble x, long y)
        {
            x._m /= y;
            return x;
        }

        public static FPDouble operator /(long x, FPDouble y)
        {
            y._m = (x << (E << 1)) / y._m;
            return y;
        }

        public static bool operator ==(FPDouble x, FPDouble y)
        {
            return x._m == y._m;
        }

        public static bool operator ==(FPDouble x, long y)
        {
            return x._m == y << E;
        }

        public static bool operator ==(long x, FPDouble y)
        {
            return x << E == y._m;
        }

        public static bool operator !=(FPDouble x, FPDouble y)
        {
            return x._m != y._m;
        }

        public static bool operator !=(FPDouble x, long y)
        {
            return x._m != y << E;
        }

        public static bool operator !=(long x, FPDouble y)
        {
            return x << E != y._m;
        }

        public static bool operator >(FPDouble x, FPDouble y)
        {
            return x._m > y._m;
        }

        public static bool operator >(FPDouble x, long y)
        {
            return x._m > y << E;
        }

        public static bool operator >(long x, FPDouble y)
        {
            return x > y._m >> E;
        }

        public static bool operator <(FPDouble x, FPDouble y)
        {
            return x._m < y._m;
        }

        public static bool operator <(FPDouble x, long y)
        {
            return x._m >> E < y;
        }

        public static bool operator <(long x, FPDouble y)
        {
            return x << E < y._m;
        }

        public static bool operator >=(FPDouble x, FPDouble y)
        {
            return x._m >= y._m;
        }

        public static bool operator >=(FPDouble x, long y)
        {
            return x._m >> E >= y;
        }

        public static bool operator >=(long x, FPDouble y)
        {
            return x << E >= y._m;
        }

        public static bool operator <=(FPDouble x, FPDouble y)
        {
            return x._m <= y._m;
        }

        public static bool operator <=(FPDouble x, long y)
        {
            return x._m <= y << E;
        }

        public static bool operator <=(long x, FPDouble y)
        {
            return x <= y._m >> E;
        }

        public static implicit operator FPDouble(int x)
        {
            return new FPDouble((long)x << E);
        }

        public static implicit operator FPDouble(long x)
        {
            return new FPDouble(x << E);
        }

        public static implicit operator FPDouble(double x)
        {
            return new FPDouble((long)(x * FP_One + (x >= 0 ? 0.5 : -0.5)));
        }

        public static implicit operator double(FPDouble x)
        {
            return (double)x._m / FP_One;
        }

        public static explicit operator float(FPDouble x)
        {
            return (float)(double)x;
        }

        public static explicit operator int(FPDouble x)
        {
            return (int)(x._m >> E);
        }

        public static explicit operator long(FPDouble x)
        {
            return x._m >> E;
        }

        public static FPDouble Parse(string s)
        {
            return double.Parse(s);
        }

        public static bool TryParse(string s, out FPDouble n)
        {
            double dn = 0;
            bool res = double.TryParse(s, out dn);
            n = dn;
            return res;
        }

        public static FPDouble Parse(CString s)
        {
            return s.ParseDouble();
        }

        public static bool TryParse(CString s, out FPDouble n)
        {
            double dn = 0;
            bool res = s.TryParseDouble(out dn);
            n = dn;
            return res;
        }

        public bool Equals(FPDouble other)
        {
            return _m == other._m;
        }

        public int CompareTo(FPDouble other)
        {
            return _m.CompareTo(other._m);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is FPDouble other)
            {
                return _m == other._m;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _m.GetHashCode();
        }

        public override string ToString()
        {
            return ((double)this).ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ((double)this).ToString(format, formatProvider);
        }

        public string ToCusString()
        {
            return $"{((double)this).ToString()}({_m})";
        }
    }
}