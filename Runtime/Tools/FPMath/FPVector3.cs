using System;

namespace JECS.Tools
{
    public struct FPVector3 : IEquatable<FPVector3>
    {
        public FPDouble x;
        public FPDouble y;
        public FPDouble z;

        public FPVector3(FPDouble x, FPDouble y, FPDouble z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(FPVector3 other) => this.x == other.x && this.y == other.y && this.z == other.z;

        public static bool operator ==(FPVector3 lhs, FPVector3 rhs) =>
            lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;

        public static bool operator !=(FPVector3 lhs, FPVector3 rhs) => !(lhs == rhs);
    }
}