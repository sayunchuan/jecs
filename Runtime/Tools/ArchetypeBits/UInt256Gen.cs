namespace JECS
{
    public static class UInt256Gen
    {
        public static UInt256 New(int t1)
        {
            UInt256 res = new UInt256();
            res.Add((int)t1);

            return res;
        }

        public static UInt256 New(int t1, int t2)
        {
            UInt256 res = new UInt256();
            res.Add((int)t1);
            res.Add((int)t2);

            return res;
        }

        public static UInt256 New(int t1, int t2, int t3)
        {
            UInt256 res = new UInt256();
            res.Add((int)t1);
            res.Add((int)t2);
            res.Add((int)t3);

            return res;
        }

        public static UInt256 New(int t1, int t2, int t3, int t4)
        {
            UInt256 res = new UInt256();
            res.Add((int)t1);
            res.Add((int)t2);
            res.Add((int)t3);
            res.Add((int)t4);

            return res;
        }

        public static UInt256 New(int t1, int t2, int t3, int t4, int t5)
        {
            UInt256 res = new UInt256();
            res.Add((int)t1);
            res.Add((int)t2);
            res.Add((int)t3);
            res.Add((int)t4);
            res.Add((int)t5);

            return res;
        }

        public static UInt256 New(int t1, int t2, int t3, int t4, int t5, int t6)
        {
            UInt256 res = new UInt256();
            res.Add((int)t1);
            res.Add((int)t2);
            res.Add((int)t3);
            res.Add((int)t4);
            res.Add((int)t5);
            res.Add((int)t6);

            return res;
        }

        public static UInt256 New(params int[] compTypes)
        {
            UInt256 res = new UInt256();
            for (int i = 0, imax = compTypes.Length; i < imax; i++)
            {
                res.Add((int)compTypes[i]);
            }

            return res;
        }
    }
}