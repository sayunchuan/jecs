using System.Collections.Generic;

namespace JECS
{
    public class UInt256Compare : IEqualityComparer<UInt256>
    {
        private readonly static UInt256Compare __ins = new UInt256Compare();

        public static UInt256Compare Default => __ins;

        public bool Equals(UInt256 x, UInt256 y)
        {
            return x == y;
        }

        public int GetHashCode(UInt256 obj)
        {
            return obj.GetHashCode();
        }
    }
}