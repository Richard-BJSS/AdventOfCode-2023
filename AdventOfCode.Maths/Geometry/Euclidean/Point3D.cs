using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public readonly struct Point3D(int x, int y, int z) 
        : IEquatable<Point3D>
    {
        public static readonly Point3D Empty = new(x: 0, y: 0, z: 0);

        public int X => x;
        public int Y => y;
        public int Z => z;

        public Point3D(Point pt, int z) : this(pt.X, pt.Y, z) {}

        public static implicit operator Point3D(Point pt) => new(pt.X, pt.Y, 0);

        public static Point3D operator +(Point3D lhs, Point3D rhs) => new(x: lhs.X + rhs.X, y: lhs.Y + rhs.Y, z: lhs.Z + rhs.Z);
        public static Point3D operator -(Point3D lhs, Point3D rhs) => new(x: lhs.X - rhs.X, y: lhs.Y - rhs.Y, z: lhs.Z - rhs.Z);
        public static Point3D operator *(int lhs, Point3D rhs) => new(x: lhs * rhs.X, y: lhs * rhs.Y, z: lhs * rhs.Z);
        public static bool operator ==(Point3D lhs, Point3D rhs) => lhs.Equals(rhs);
        public static bool operator !=(Point3D lhs, Point3D rhs) => !(lhs == rhs);

        public bool Equals(Point3D other) => X == other.X && Y == other.Y && Z == other.Z;
        public override bool Equals(object? obj) => obj is Point3D other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override string ToString() => $"[{X},{Y},{Z}]";

        // North   = new(x:  0, y: +1, z:  0);
        // South   = new(x:  0, y: -1, z:  0);
        // East    = new(x: -1, y:  0, z:  0);
        // West    = new(x: +1, y:  0, z:  0);
        // Forward = new(x:  0, y:  0, z: +1);
        // Back    = new(x:  0, y:  0, z: -1);

        // ManhattanAdjacentSet => [ this + North, this + South, this + East, this + West, this + Forward, this + Back ];

        // ChebyshevAdjacentSet =>
        //
        //    var r = new HashSet<Point3D>();

        //    for (var dx = -1; dx <= 1; dx++)
        //        for (var dy = -1; dy <= 1; dy++)
        //            for (var dz = -1; dz <= 1; dz++)
        //                r.Add(new Point3D(
        //                    x: X + dx,
        //                    y: Y + dy,
        //                    z: Z + dz));

        //    r.Remove(item: this);
    }
}
