namespace JECS
{
    public struct UInt256Iterator
    {
        private UInt256 __flag;
        private int __index;

        public UInt256Iterator(UInt256 flag)
        {
            __flag = flag;
            __index = -1;
        }

        public int Current => __index;

        public bool MoveNext()
        {
            do
            {
                __index++;
            } while (__index < 256 && !__flag.Contain(__index));

            return __index < 256;
        }

        public static string UInt256ToString(UInt256 f)
        {
            var sb = StringBuilderPool.Spawn();
            UInt256Iterator ite = new UInt256Iterator(f);
            sb.Append("[");
            while (ite.MoveNext())
            {
                sb.Append(ite.Current).Append(",");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return StringBuilderPool.ReleaseRet(sb);
        }
    }
}