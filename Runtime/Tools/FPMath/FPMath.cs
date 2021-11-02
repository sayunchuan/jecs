namespace JECS.Tools
{
    public static class FPMath
    {
        public static readonly FPDouble PI = new FPDouble(FPDouble.FP_PI);
        public static readonly FPDouble TWO_PI = new FPDouble(FPDouble.FP_2_PI);
        public static readonly FPDouble PI_DIV_2 = new FPDouble(FPDouble.FP_PI_DIV_2);
        public static readonly FPDouble PI_DIV_4 = new FPDouble(FPDouble.FP_PI_DIV_4);
        public static readonly FPDouble ONE_DIV_PI = new FPDouble(FPDouble.FP_1_DIV_PI);

        public static FPDouble DirToAngle(FPVector2 dir)
        {
            FPDouble asin = RadianToAngle(Asin(dir.y));
            return dir.x > 0 ? asin : 180 - asin;
        }

        public static FPDouble RadianToAngle(FPDouble r)
        {
            return r * 180 / PI;
        }

        public static FPDouble AngleToRadian(FPDouble r)
        {
            return r * PI / 180;
        }

        public static FPDouble Pow2(FPDouble n)
        {
            return n * n;
        }

        public static FPDouble Sin(FPDouble x)
        {
            long tempAngle = x._m % FPDouble.FP_2_PI;
            if (tempAngle < 0) tempAngle += FPDouble.FP_2_PI;

            if (tempAngle >= FPDouble.FP_PI)
            {
                tempAngle -= FPDouble.FP_PI;
                if (tempAngle >= FPDouble.FP_PI_DIV_2) tempAngle = FPDouble.FP_PI - tempAngle;
                return -(tempAngle >= FPTable.table_length ? FPDouble.One : new FPDouble(FPTable.table[tempAngle]));
            }

            if (tempAngle >= FPDouble.FP_PI_DIV_2) tempAngle = FPDouble.FP_PI - tempAngle;
            return tempAngle >= FPTable.table_length ? FPDouble.One : new FPDouble(FPTable.table[tempAngle]);
        }

        public static FPDouble Cos(FPDouble x)
        {
            return Sin(new FPDouble(x._m + FPDouble.FP_PI_DIV_2));
        }

        public static FPDouble Tan(FPDouble inAngle)
        {
            return Sin(inAngle) / Cos(inAngle);
        }

        public static FPDouble Asin(FPDouble x)
        {
            if (x > FPDouble.One || x < -FPDouble.One) return 0;

            FPDouble res;
            res = FPDouble.One - x * x;
            if (res == FPDouble.Zero) return x > FPDouble.Zero ? PI_DIV_2 : -PI_DIV_2;

            res = x / FPDouble.Sqrt(res);
            res = Atan(res);
            return res;
        }

        public static FPDouble Acos(FPDouble x)
        {
            return PI_DIV_2 - Asin(x);
        }

        public static FPDouble Atan2(FPDouble inY, FPDouble inX)
        {
            FPDouble ax = FPDouble.Abs(inX), ay = FPDouble.Abs(inY);
            FPDouble a = FPDouble.Min(ax, ay) / FPDouble.Max(ax, ay);
            FPDouble s = a * a;
            // double r = ((-0.0464964749 * s + 0.15931422) * s - 0.327622764) * s * a + a;
            FPDouble r = ((new FPDouble(10441) - new FPDouble(3047) * s) * s - new FPDouble(21471)) * s * a + a;
            if (ay > ax) r = PI_DIV_2 - r;
            if (inX < 0) r = PI - r;
            if (inY < 0) r = -r;
            return r;
        }

        public static FPDouble Atan(FPDouble x)
        {
            return Atan2(x, FPDouble.One);
        }

        public static FPDouble Lerp(FPDouble min, FPDouble max, FPDouble percent)
        {
            return min + (max - min) * percent;
        }

        public static FPDouble Lerp01(FPDouble min, FPDouble max, FPDouble percent)
        {
            if (percent >= FPDouble.One) return max;
            if (percent <= FPDouble.Zero) return min;
            return min + (max - min) * percent;
        }

        public static FPDouble AngleNormalize(FPDouble angle)
        {
            FPDouble res = angle % 360;
            return res < 0 ? res + 360 : res;
        }

        public static FPVector2 Angle2Vector(FPDouble angle)
        {
            FPDouble r = AngleToRadian(AngleNormalize(angle));
            return new FPVector2(Cos(r), Sin(r));
        }

        public static bool IsPointInDirLeft(FPVector2 dir, FPVector2 dirBeginPoint, FPVector2 point)
        {
            return IsDirInBaseDirLeft(dir, point - dirBeginPoint);
        }

        public static bool IsDirInBaseDirLeft(FPVector2 baseDir, FPVector2 dir)
        {
            return FPVector2.Cross(baseDir, dir) > 0;
        }

        public static bool IsPointInDirRight(FPVector2 dir, FPVector2 dirBeginPoint, FPVector2 point)
        {
            return IsDirInBaseDirRight(dir, point - dirBeginPoint);
        }

        public static bool IsDirInBaseDirRight(FPVector2 baseDir, FPVector2 dir)
        {
            return FPVector2.Cross(baseDir, dir) < 0;
        }

        public static bool IsPointInDirForward(FPVector2 dir, FPVector2 dirBeginPoint, FPVector2 point)
        {
            return FPVector2.Dot(dir, point - dirBeginPoint) > 0;
        }

        public static bool IsPointInDirBack(FPVector2 dir, FPVector2 dirBeginPoint, FPVector2 point)
        {
            return FPVector2.Dot(dir, point - dirBeginPoint) < 0;
        }

        /// <summary>
        /// 血量取整运算
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FPDouble HPValue(FPDouble value)
        {
            return value >= 0 ? FPDouble.Ceiling(value) : -FPDouble.Ceiling(-value);
        }
    }
}