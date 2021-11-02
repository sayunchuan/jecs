namespace JECS
{
    public struct UInt256
    {
        private ulong b0;
        private ulong b1;
        private ulong b2;
        private ulong b3;

        private const ulong One = 1;

        public unsafe bool Add(int index)
        {
            fixed (ulong* tmp = &b0)
            {
                ulong mask = One << (index & 0x3f);
                ulong* p = tmp + (index >> 6);
                if ((*p & mask) != 0) return false;
                *p |= mask;
                return true;
            }
        }

        public unsafe bool Del(int index)
        {
            fixed (ulong* tmp = &b0)
            {
                ulong mask = One << (index & 0x3f);
                ulong* p = tmp + (index >> 6);
                if ((*p & mask) == 0) return false;
                *p &= ~mask;
                return true;
            }
        }

        public void Clear()
        {
            b0 = 0;
            b1 = 0;
            b2 = 0;
            b3 = 0;
        }

        public static bool operator !=(UInt256 a, UInt256 b)
        {
            return a.b0 != b.b0 || a.b1 != b.b1 || a.b2 != b.b2 || a.b3 != b.b3;
        }

        public bool Contain(UInt256 n)
        {
            return (b0 & n.b0) == n.b0 && (b1 & n.b1) == n.b1 && (b2 & n.b2) == n.b2 && (b3 & n.b3) == n.b3;
        }

        public unsafe bool Contain(int index)
        {
            fixed (ulong* tmp = &b0)
            {
                return (*(tmp + (index >> 6)) & One << (index & 0x3f)) != 0;
            }
        }

        public static bool operator ==(UInt256 a, UInt256 b)
        {
            return a.b0 == b.b0 && a.b1 == b.b1 && a.b2 == b.b2 && a.b3 == b.b3;
        }

        public override bool Equals(object obj)
        {
            return obj is UInt256 && this == (UInt256)obj;
        }

        public override int GetHashCode()
        {
            unsafe
            {
                ulong tmp = b0 ^ b1 ^ b2 ^ b3;
                int* p = (int*)&tmp;
                return *p ^ *(p + 1);
            }
        }

        public override string ToString()
        {
            return string.Format("{0:x16}{1:x16}{2:x16}{3:x16} | {4}", b0, b1, b2, b3,
                UInt256Iterator.UInt256ToString(this));
        }
    }
}