using System;

namespace JECS.Tools
{
    public struct FPVector2 : IEquatable<FPVector2>
    {
        public FPDouble x;
        public FPDouble y;

        public static readonly FPVector2 down = new FPVector2(0, -1);

        public static readonly FPVector2 left = new FPVector2(-1, 0);

        public static readonly FPVector2 one = new FPVector2(1, 1);

        public static readonly FPVector2 right = new FPVector2(1, 0);

        public static readonly FPVector2 up = new FPVector2(0, 1);

        public static readonly FPVector2 zero = new FPVector2(0, 0);

        //
        // Properties
        //
        public FPDouble magnitude
        {
            get
            {
                long lx = FPDouble.DoubleToInt64Bits(x);
                long ly = FPDouble.DoubleToInt64Bits(y);
                long res = (long)System.Math.Sqrt(lx * lx + ly * ly);
                return FPDouble.Int64BitsToDouble(res);
            }
        }

        public FPVector2 normalized
        {
            get
            {
                FPDouble tmp = magnitude;
                return tmp <= 0 ? zero : new FPVector2(x / tmp, y / tmp);
            }
        }

        public FPDouble sqrMagnitude => x * x + y * y;

        public FPDouble dirSin => y;
        public FPDouble dirCos => x;

        /// <summary>
        /// 向量所处象限，值为1-4
        /// </summary>
        public int Quadrant => dirSin >= 0 ? dirCos >= 0 ? 1 : 2 : dirCos > 0 ? 4 : 3;

        //
        // Constructors
        //
        public FPVector2(FPDouble x, FPDouble y)
        {
            this.x = x;
            this.y = y;
        }

        //
        // Static Methods
        //
        public static FPVector2 ClampMagnitude(FPVector2 vector, FPDouble maxLength)
        {
            FPDouble multiple = maxLength / vector.magnitude;
            return vector * multiple;
        }

        public static FPDouble Distance(FPVector2 a, FPVector2 b)
        {
            FPVector2 delta = a - b;
            return delta.magnitude;
        }

        /// <summary>
        /// 二维向量叉乘值，因二维向量叉乘固定为Z轴方向，因为该值恒等于 |b| * |e| * sin
        /// sin值为以b开始逆时针转向e的角度的sin值
        /// </summary>
        public static FPDouble Cross(FPVector2 b, FPVector2 e)
        {
            return b.x * e.y - b.y * e.x;
        }

        /// <summary>
        /// 二维向量点乘值，该值恒等于 |lhs| * |rhs| * cos
        /// </summary>
        public static FPDouble Dot(FPVector2 lhs, FPVector2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        public static FPVector2 Lerp(FPVector2 a, FPVector2 b, FPDouble t)
        {
            t = FPDouble.Min(FPDouble.Max(t, 0), 1);
            return LerpUnclamped(a, b, t);
        }

        public static FPVector2 LerpUnclamped(FPVector2 a, FPVector2 b, FPDouble t)
        {
            return new FPVector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        public static FPVector2 Max(FPVector2 lhs, FPVector2 rhs)
        {
            return new FPVector2(FPDouble.Max(lhs.x, rhs.x), FPDouble.Max(lhs.y, rhs.y));
        }

        public static FPVector2 Min(FPVector2 lhs, FPVector2 rhs)
        {
            return new FPVector2(FPDouble.Min(lhs.x, rhs.x), FPDouble.Min(lhs.y, rhs.y));
        }

        //
        // Methods
        //
        public override bool Equals(object other)
        {
            if (other != null && other is FPVector2)
            {
                return this == (FPVector2)other;
            }

            return false;
        }

        public void Normalize()
        {
            FPDouble multiple = magnitude;
            if (multiple != 0)
            {
                x /= multiple;
                y /= multiple;
            }
            else
            {
                x = FPDouble.Zero;
                y = FPDouble.Zero;
            }
        }

        public void Set(FPDouble newX, FPDouble newY)
        {
            x = newX;
            y = newY;
        }

        public void FastNormalize(out FPVector2 nor, out FPDouble mag)
        {
            mag = magnitude;
            if (mag != FPDouble.Zero)
            {
                nor = new FPVector2(x / mag, y / mag);
            }
            else
            {
                nor = new FPVector2(FPDouble.Zero, FPDouble.Zero);
            }
        }

        public static FPDouble Angle(FPVector2 from, FPVector2 to)
        {
            FPDouble num = FPDouble.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (num <= 0) return 0;

            FPDouble val = Dot(from, to) / num;
            val = FPDouble.Clamp(val, -1, 1);
            return FPMath.Acos(val) * 180 * FPMath.ONE_DIV_PI;
        }

        public override string ToString()
        {
            return string.Format("({0:F6}, {1:F6})", x, y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x.GetHashCode() * 397) ^ y.GetHashCode();
            }
        }

        //
        // Operators
        //
        public static FPVector2 operator +(FPVector2 a, FPVector2 b)
        {
            return new FPVector2(a.x + b.x, a.y + b.y);
        }

        public static FPVector2 operator -(FPVector2 a, FPVector2 b)
        {
            return new FPVector2(a.x - b.x, a.y - b.y);
        }

        public static FPVector2 operator -(FPVector2 a)
        {
            return new FPVector2(-a.x, -a.y);
        }

        public static FPVector2 operator *(FPDouble d, FPVector2 a)
        {
            return new FPVector2(a.x * d, a.y * d);
        }

        public static FPVector2 operator *(FPVector2 a, FPDouble d)
        {
            return new FPVector2(a.x * d, a.y * d);
        }

        public static FPVector2 operator /(FPVector2 a, FPDouble d)
        {
            if (d != 0)
            {
                return new FPVector2(a.x / d, a.y / d);
            }

            return zero;
        }

        public static bool operator ==(FPVector2 lhs, FPVector2 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(FPVector2 lhs, FPVector2 rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y;
        }

        // Static methods

        /// <summary>
        /// 通过输入角度的sin与cos值，将oldPos以原点进行旋转
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="sin"></param>
        /// <param name="cos"></param>
        /// <returns></returns>
        public static FPVector2 GetRotateNewPos(FPVector2 oldPos, FPDouble sin, FPDouble cos)
        {
            return new FPVector2(oldPos.x * cos - oldPos.y * sin, oldPos.x * sin + oldPos.y * cos);
        }

        /// <summary>
        /// 通过dir所表示的角度，将oldPos以原点进行旋转
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static FPVector2 GetRotateNewPos(FPVector2 oldPos, FPVector2 dir)
        {
            return GetRotateNewPos(oldPos, dir.dirSin, dir.dirCos);
        }

        /// <summary>
        /// 通过dir所表示的角度，将oldPos以原点进行反向旋转
        /// </summary>
        /// <param name="newPos"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static FPVector2 RevertRotatePos(FPVector2 newPos, FPVector2 dir)
        {
            return GetRotateNewPos(newPos, -dir.dirSin, dir.dirCos);
        }

        /// <summary>
        /// 通过输入角度的sin与cos值，将oldPos围绕oPoint进行旋转
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="oPoint"></param>
        /// <param name="sin"></param>
        /// <param name="cos"></param>
        /// <returns></returns>
        public static FPVector2 GetRotateNewPosWithPoint(FPVector2 oldPos, FPVector2 oPoint, FPDouble sin, FPDouble cos)
        {
            return new FPVector2(oldPos.x * cos - oldPos.y * sin + (1 - cos) * oPoint.x + sin * oPoint.y,
                oldPos.x * sin + oldPos.y * cos + (1 - cos) * oPoint.y - sin * oPoint.x);
        }

        /// <summary>
        /// 判断矢量1与矢量2会相交
        /// </summary>
        public static bool IsVecCross(FPVector2 pos1, FPVector2 dir1, FPVector2 pos2, FPVector2 dir2)
        {
            FPVector2 pos12 = pos2 - pos1;
            FPDouble sinDir1 = Cross(dir1, pos12);
            FPDouble sinDir2 = Cross(dir2, -pos12);
            FPDouble sinDir1Dir2 = Cross(dir1, dir2);
            return (sinDir1 < 0 && sinDir2 > 0 && sinDir1Dir2 > 0) || (sinDir1 > 0 && sinDir2 < 0 && sinDir1Dir2 < 0);
        }

        /// <summary>
        /// 求经过c1点垂线与v0v1交点坐标
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="c">p0p1直线外的点</param>
        /// <returns></returns>
        public static FPVector2 CalculateVerticalLinePoint(FPVector2 v0, FPVector2 v1, FPVector2 c)
        {
            FPVector2 c1 = -1 * GetRotateNewPos(c, 1, 0) + c;
            return FindIntersection(v0, v1, c, c1);
        }

        /// <summary>
        /// 求c点在p0和p1所在直线投影
        /// </summary>
        public static FPVector2 CalculateProjectPoint(FPVector2 p0, FPVector2 p1, FPVector2 c)
        {
            FPVector2 p0p1 = (p0 - p1).normalized;
            FPVector2 p1c = c - p1;
            FPDouble dot = Dot(p0p1, p1c);
            return dot * p0p1 + p1;
        }

        /// <summary>
        /// 求c点在p0和p1线段投影，若不在线段内则返回对应线段订点
        /// </summary>
        public static FPVector2 GetProjectPoint(FPVector2 p0, FPVector2 p1, FPVector2 c)
        {
            FPVector2 p01 = p1 - p0;
            FPVector2 p01n = p01.normalized;
            FPVector2 p0c = c - p0;
            FPDouble dot = Dot(p01n, p0c);
            if (dot <= 0) return p0;
            if (dot * dot >= p01.sqrMagnitude) return p1;
            return dot * p01n + p0;
        }

        /// <summary>
        /// 求c点与线段p0 p1的距离
        /// </summary>
        public static FPDouble LengthWithVector(FPVector2 p0, FPVector2 p1, FPVector2 c)
        {
            return (GetProjectPoint(p0, p1, c) - c).magnitude;
        }

        /// <summary>
        /// 求c点与线段p0 p1的距离平方
        /// </summary>
        public static FPDouble SqrLengthWithVector(FPVector2 p0, FPVector2 p1, FPVector2 c)
        {
            return (GetProjectPoint(p0, p1, c) - c).sqrMagnitude;
        }

        /// <summary>
        /// 寻找交叉点
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="e1"></param>
        /// <param name="s2"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public static FPVector2 FindIntersection(FPVector2 s1, FPVector2 e1, FPVector2 s2, FPVector2 e2)
        {
            FPDouble a1 = e1.y - s1.y;
            FPDouble b1 = s1.x - e1.x;
            FPDouble c1 = a1 * s1.x + b1 * s1.y;

            FPDouble a2 = e2.y - s2.y;
            FPDouble b2 = s2.x - e2.x;
            FPDouble c2 = a2 * s2.x + b2 * s2.y;

            FPDouble delta = a1 * b2 - a2 * b1;

            return delta == 0 ? s1 : new FPVector2((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
        }

        /// <summary>
        /// 判断线段p1p2与线段q1q2是否有交点
        /// </summary>
        public static bool HadIntersection(FPVector2 p1, FPVector2 p2, FPVector2 q1, FPVector2 q2,
            out FPVector2 intersection)
        {
            FPVector2 vp = p2 - p1;
            FPVector2 vpNor = vp.normalized;

            FPMatrix3x3 rRotate = FPMatrix3x3.Rotate(new FPVector2(vpNor.dirCos, -vpNor.dirSin));
            FPMatrix3x3 rTranslate = FPMatrix3x3.Translate(-p1);

            FPVector2 rq1 = rRotate.MultiplyPoint2x3(rTranslate.MultiplyPoint2x3(q1));
            FPVector2 rq2 = rRotate.MultiplyPoint2x3(rTranslate.MultiplyPoint2x3(q2));

            if (rq1.y * rq2.y > 0)
            {
                intersection = FPVector2.zero;
                return false;
            }

            FPDouble sqrVPLen = vp.sqrMagnitude;
            FPDouble resX;

            if (rq1.y == rq2.y)
            {
                // 此种状态下尽可能两y值为0
                FPDouble minX = FPDouble.Min(rq1.x, rq2.x);
                FPDouble maxX = rq1.x + rq2.x - minX;
                if (maxX < 0 || (minX >= 0 && minX * minX > sqrVPLen))
                {
                    intersection = FPVector2.zero;
                    return false;
                }

                resX = minX < 0 ? 0 : minX;
            }
            else
            {
                resX = (rq1.y * rq2.x - rq1.x * rq2.y) / (rq1.y - rq2.y);

                if (resX < 0 || resX * resX > sqrVPLen)
                {
                    intersection = FPVector2.zero;
                    return false;
                }
            }

            FPMatrix3x3 rotate = FPMatrix3x3.Rotate(vpNor);
            FPMatrix3x3 translate = FPMatrix3x3.Translate(p1);
            FPVector2 rRes = new FPVector2(resX, FPDouble.Zero);
            intersection = translate.MultiplyPoint2x3(rotate.MultiplyPoint2x3(rRes));
            return true;
        }

        /// <summary>
        /// 判断线段p1p2与线段q1q2是否有交点
        /// </summary>
        public static bool HadIntersection(FPVector2 p1, FPVector2 p2, FPVector2 q1, FPVector2 q2)
        {
            FPVector2 p12 = p2 - p1;
            FPVector2 q12 = q2 - q1;

            return Cross(p12, q1 - p1) * Cross(p12, q2 - p1) < 0 && Cross(q12, p1 - q1) * Cross(q12, p2 - q1) < 0;
        }

        public bool Equals(FPVector2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }
    }
}