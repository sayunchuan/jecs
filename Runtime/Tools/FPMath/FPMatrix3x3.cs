using System;
using System.Globalization;

namespace JECS.Tools
{
    public struct FPMatrix3x3 : IEquatable<FPMatrix3x3>
    {
        /*
         * 矩阵说明
         * +-             -+
         * | m00, m01, m02 |
         * | m10, m11, m12 |
         * | m20, m21, m22 |
         * +-             -+
         *
         * 数组位置说明
         * +-       -+
         * | 0, 3, 6 |
         * | 1, 4, 7 |
         * | 2, 5, 8 |
         * +-       -+
         */

        public FPDouble m00;
        public FPDouble m10;
        public FPDouble m20;
        public FPDouble m01;
        public FPDouble m11;
        public FPDouble m21;
        public FPDouble m02;
        public FPDouble m12;
        public FPDouble m22;

        private static readonly FPMatrix3x3 zeroMatrix = new FPMatrix3x3(
            new FPVector3(FPDouble.Zero, FPDouble.Zero, FPDouble.Zero),
            new FPVector3(FPDouble.Zero, FPDouble.Zero, FPDouble.Zero),
            new FPVector3(FPDouble.Zero, FPDouble.Zero, FPDouble.Zero));

        private static readonly FPMatrix3x3 identityMatrix = new FPMatrix3x3(
            new FPVector3(FPDouble.One, FPDouble.Zero, FPDouble.Zero),
            new FPVector3(FPDouble.Zero, FPDouble.One, FPDouble.Zero),
            new FPVector3(FPDouble.Zero, FPDouble.Zero, FPDouble.One));

        public FPMatrix3x3(FPVector3 column0, FPVector3 column1, FPVector3 column2)
        {
            m00 = column0.x;
            m01 = column1.x;
            m02 = column2.x;
            m10 = column0.y;
            m11 = column1.y;
            m12 = column2.y;
            m20 = column0.z;
            m21 = column1.z;
            m22 = column2.z;
        }

        public FPDouble this[int row, int column]
        {
            get => this[row + column * 3];
            set => this[row + column * 3] = value;
        }

        public FPDouble this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return m00;
                    case 1:
                        return m10;
                    case 2:
                        return m20;
                    case 3:
                        return m01;
                    case 4:
                        return m11;
                    case 5:
                        return m21;
                    case 6:
                        return m02;
                    case 7:
                        return m12;
                    case 8:
                        return m22;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        m00 = value;
                        break;
                    case 1:
                        m10 = value;
                        break;
                    case 2:
                        m20 = value;
                        break;
                    case 3:
                        m01 = value;
                        break;
                    case 4:
                        m11 = value;
                        break;
                    case 5:
                        m21 = value;
                        break;
                    case 6:
                        m02 = value;
                        break;
                    case 7:
                        m12 = value;
                        break;
                    case 8:
                        m22 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        public override int GetHashCode()
        {
            FPVector3 column = GetColumn(0);
            int hashCode = column.GetHashCode();
            column = GetColumn(1);
            int num1 = column.GetHashCode() << 2;
            int num2 = hashCode ^ num1;
            column = GetColumn(2);
            int num3 = column.GetHashCode() >> 2;
            return num2 ^ num3;
        }

        public bool Equals(FPMatrix3x3 other)
        {
            int num;
            if (GetColumn(0).Equals(other.GetColumn(0)))
            {
                FPVector3 column = GetColumn(1);
                if (column.Equals(other.GetColumn(1)))
                {
                    column = GetColumn(2);
                    return column.Equals(other.GetColumn(2));
                }
            }

            return false;
        }

        public static FPMatrix3x3 operator *(FPMatrix3x3 lhs, FPMatrix3x3 rhs)
        {
            FPMatrix3x3 matrix3X3;
            matrix3X3.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20;
            matrix3X3.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21;
            matrix3X3.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22;
            matrix3X3.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20;
            matrix3X3.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21;
            matrix3X3.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22;
            matrix3X3.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20;
            matrix3X3.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21;
            matrix3X3.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22;
            return matrix3X3;
        }

        public static FPVector3 operator *(FPMatrix3x3 lhs, FPVector3 vector)
        {
            FPVector3 vector3;
            vector3.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z;
            vector3.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z;
            vector3.z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z;
            return vector3;
        }

        public static bool operator ==(FPMatrix3x3 lhs, FPMatrix3x3 rhs) => lhs.GetColumn(0) == rhs.GetColumn(0) &&
                                                                            lhs.GetColumn(1) == rhs.GetColumn(1) &&
                                                                            lhs.GetColumn(2) == rhs.GetColumn(2);

        public static bool operator !=(FPMatrix3x3 lhs, FPMatrix3x3 rhs) => !(lhs == rhs);

        /// <summary>
        ///   <para>Get a column of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        public FPVector3 GetColumn(int index)
        {
            switch (index)
            {
                case 0:
                    return new FPVector3(m00, m10, m20);
                case 1:
                    return new FPVector3(m01, m11, m21);
                case 2:
                    return new FPVector3(m02, m12, m22);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        /// <summary>
        ///   <para>Returns a row of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        public FPVector3 GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return new FPVector3(m00, m01, m02);
                case 1:
                    return new FPVector3(m10, m11, m12);
                case 2:
                    return new FPVector3(m20, m21, m22);
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
        }

        /// <summary>
        ///   <para>Sets a column of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="column"></param>
        public void SetColumn(int index, FPVector3 column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
        }

        /// <summary>
        ///   <para>Sets a row of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="row"></param>
        public void SetRow(int index, FPVector3 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
        }

        /// <summary>
        ///   <para>Transforms a position by this matrix (generic).</para>
        /// </summary>
        /// <param name="point"></param>
        public FPVector2 MultiplyPoint(FPVector2 point)
        {
            FPVector2 vector2;
            double _x = m00 * point.x + m01 * point.y + m02;
            double _y = m10 * point.x + m11 * point.y + m12;
            double num = 1 / (m20 * point.x + m21 * point.y + m22);
            vector2.x = _x * num;
            vector2.y = _y * num;
            return vector2;
        }

        /// <summary>
        ///   <para>Transforms a position by this matrix (fast).</para>
        /// </summary>
        /// <param name="point"></param>
        public FPVector2 MultiplyPoint2x3(FPVector2 point)
        {
            FPVector2 vector2;
            vector2.x = m00 * point.x + m01 * point.y + m02;
            vector2.y = m10 * point.x + m11 * point.y + m12;
            return vector2;
        }

        /// <summary>
        ///   <para>Transforms a direction by this matrix.</para>
        /// </summary>
        /// <param name="vector"></param>
        public FPVector2 MultiplyVector(FPVector2 vector)
        {
            FPVector2 vector2;
            vector2.x = m00 * vector.x + m01 * vector.y;
            vector2.y = m10 * vector.x + m11 * vector.y;
            return vector2;
        }

        /// <summary>
        ///   <para>Creates a scaling matrix.</para>
        /// </summary>
        /// <param name="vector"></param>
        public static FPMatrix3x3 Scale(FPVector2 vector)
        {
            FPMatrix3x3 matrix3X3;
            matrix3X3.m00 = vector.x;
            matrix3X3.m01 = FPDouble.Zero;
            matrix3X3.m02 = FPDouble.Zero;

            matrix3X3.m10 = FPDouble.Zero;
            matrix3X3.m11 = vector.y;
            matrix3X3.m12 = FPDouble.Zero;

            matrix3X3.m20 = FPDouble.Zero;
            matrix3X3.m21 = FPDouble.Zero;
            matrix3X3.m22 = FPDouble.One;
            return matrix3X3;
        }

        /// <summary>
        ///   <para>Creates a translation matrix.</para>
        /// </summary>
        /// <param name="vector"></param>
        public static FPMatrix3x3 Translate(FPVector2 vector)
        {
            FPMatrix3x3 matrix3X3;
            matrix3X3.m00 = FPDouble.One;
            matrix3X3.m01 = FPDouble.Zero;
            matrix3X3.m02 = vector.x;

            matrix3X3.m10 = FPDouble.Zero;
            matrix3X3.m11 = FPDouble.One;
            matrix3X3.m12 = vector.y;

            matrix3X3.m20 = FPDouble.Zero;
            matrix3X3.m21 = FPDouble.Zero;
            matrix3X3.m22 = FPDouble.One;
            return matrix3X3;
        }

        /// <summary>
        ///   <para>Creates a rotation matrix.</para>
        /// </summary>
        /// <param name="r"></param>
        public static FPMatrix3x3 Rotate(FPDouble r)
        {
            double cos = System.Math.Cos(r);
            double sin = System.Math.Sin(r);
            FPMatrix3x3 matrix3X3;
            matrix3X3.m00 = cos;
            matrix3X3.m01 = -sin;
            matrix3X3.m02 = FPDouble.Zero;

            matrix3X3.m10 = sin;
            matrix3X3.m11 = cos;
            matrix3X3.m12 = FPDouble.Zero;

            matrix3X3.m20 = FPDouble.Zero;
            matrix3X3.m21 = FPDouble.Zero;
            matrix3X3.m22 = FPDouble.One;
            return matrix3X3;
        }

        /// <summary>
        ///   <para>Creates a rotation matrix.</para>
        /// </summary>
        /// <param name="dir"></param>
        public static FPMatrix3x3 Rotate(FPVector2 dir)
        {
            FPMatrix3x3 matrix3X3;
            matrix3X3.m00 = dir.dirCos;
            matrix3X3.m01 = -dir.dirSin;
            matrix3X3.m02 = FPDouble.Zero;

            matrix3X3.m10 = dir.dirSin;
            matrix3X3.m11 = dir.dirCos;
            matrix3X3.m12 = FPDouble.Zero;

            matrix3X3.m20 = FPDouble.Zero;
            matrix3X3.m21 = FPDouble.Zero;
            matrix3X3.m22 = FPDouble.One;
            return matrix3X3;
        }

        /// <summary>
        ///   <para>Returns a matrix with all elements set to zero (Read Only).</para>
        /// </summary>
        public static FPMatrix3x3 zero => zeroMatrix;

        /// <summary>
        ///   <para>Returns the identity matrix (Read Only).</para>
        /// </summary>
        public static FPMatrix3x3 identity => identityMatrix;

        /// <summary>
        ///   <para>Returns a nicely formatted string for this matrix.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString() =>
            string.Format("{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\n", m00,
                m01, m02, m10, m11, m12, m20, m21, m22);

        /// <summary>
        ///   <para>Returns a nicely formatted string for this matrix.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format) => string.Format(
            "{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\n",
            ToInvariantString(format, m00), ToInvariantString(format, m01),
            ToInvariantString(format, m02), ToInvariantString(format, m10),
            ToInvariantString(format, m11), ToInvariantString(format, m12),
            ToInvariantString(format, m20), ToInvariantString(format, m21),
            ToInvariantString(format, m22));

        private string ToInvariantString(string format, FPDouble val) =>
            val.ToString(format, CultureInfo.InvariantCulture.NumberFormat);
    }
}