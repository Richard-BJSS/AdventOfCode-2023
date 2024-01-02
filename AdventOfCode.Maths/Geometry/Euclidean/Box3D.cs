using System.Collections;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    // Used to represent the bounding box that encapsulates a 3D 'box shape' on the X, Y and Z planes

    public readonly struct Box3D
        : IEnumerable<Point3D>,
          IEquatable<Box3D>
    {
        public Box3D(IEnumerable<Point3D> extents, bool inclusive)
        {
            var delta = inclusive ? 0 : 1;

            Min = new Point3D(
                x: extents.Min(p => p.X) - delta,
                y: extents.Min(p => p.Y) - delta,
                z: extents.Min(p => p.Z) - delta
                );

            Max = new Point3D(
                x: extents.Max(p => p.X) + delta,
                y: extents.Max(p => p.Y) + delta,
                z: extents.Max(p => p.Z) + delta
                );
        }

        private Box3D(Point3D min, Point3D max) { Min = min; Max = max; }

        public Point3D Min { get; init; }
        public Point3D Max { get; init; }

        public int MinX => Min.X;
        public int MaxX => Max.X;
        public int MinY => Min.Y;
        public int MaxY => Max.Y;
        public int MinZ => Min.Z;
        public int MaxZ => Max.Z;

        public int LengthX => MaxX - MinX + 1;
        public int LengthY => MaxY - MinY + 1;
        public int LengthZ => MaxZ - MinZ + 1;

        public Box3D Shift(Point3D amount) => new(min: Min + amount, max: Max + amount);

        //public Box3D(int xMin, int xMax, int yMin, int yMax, int zMin, int zMax)
        //{
        //    Min = new Point3D(xMin, yMin, zMin);
        //    Max = new Point3D(xMax, yMax, zMax);
        //}

        //public long Volume => (long)XLength * YLength * ZLength;
        //public Point3D Center => new(x: (XMin + XMax) / 2, y: (YMin + YMax) / 2, z: (ZMin + ZMax) / 2);

        //public static bool Intersection(Box3D a, Box3D b, out Box3D intersect)
        //{
        //    var hasOverlap =
        //        a.XMax >= b.XMin && a.XMin <= b.XMax &&
        //        a.YMax >= b.YMin && a.YMin <= b.YMax &&
        //        a.ZMax >= b.ZMin && a.ZMin <= b.ZMax;
        //
        //    intersect = default;
        //
        //    if (!hasOverlap) return false;
        //
        //    var xLimits = new[] { a.XMin, a.XMax, b.XMin, b.XMax }.Order().ToList();
        //    var yLimits = new[] { a.YMin, a.YMax, b.YMin, b.YMax }.Order().ToList();
        //    var zLimits = new[] { a.ZMin, a.ZMax, b.ZMin, b.ZMax }.Order().ToList();
        //
        //    intersect = new Box3D(
        //        xMin: xLimits[1],
        //        xMax: xLimits[2],
        //        yMin: yLimits[1],
        //        yMax: yLimits[2],
        //        zMin: zLimits[1],
        //        zMax: zLimits[2]);
        //    return true;
        //}

        //public long GetSurfaceArea() => 2L * (ZLength * XLength + YLength * XLength + YLength * ZLength);

        //public bool Contains(Point3D pos, bool inclusive) => inclusive ? ContainsInclusive(pos) : ContainsExclusive(pos);

        //private bool ContainsInclusive(Point3D pos) => pos.X >= XMin && pos.X <= XMax && pos.Y >= YMin && pos.Y <= YMax && pos.Z >= ZMin && pos.Z <= ZMax;

        //private bool ContainsExclusive(Point3D pos) => pos.X > XMin && pos.X < XMax && pos.Y > YMin && pos.Y < YMax && pos.Z > ZMin && pos.Z < ZMax;

        //public static Box3D CubeCenteredAt(Point3D center, int extent)
        //{
        //    if (extent < 0) throw new ArgumentOutOfRangeException(nameof(extent), extent, "must be > 0");
        //
        //    return new Box3D(
        //        xMin: center.X - extent,
        //        xMax: center.X + extent,
        //        yMin: center.Y - extent,
        //        yMax: center.Y + extent,
        //        zMin: center.Z - extent,
        //        zMax: center.Z + extent
        //        );
        //}

        public IEnumerator<Point3D> GetEnumerator()
        {
            for (var x = MinX; x <= MaxX; x++)
                for (var y = MinY; y <= MaxY; y++)
                    for (var z = MinZ; z <= MaxZ; z++)
                        yield return new(x, y, z);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Box3D other) => MinX == other.MinX && MaxX == other.MaxX && MinY == other.MinY && MaxY == other.MaxY && MinZ == other.MinZ && MaxZ == other.MaxZ;
        public override bool Equals(object? obj) => obj is Box3D other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(MinX, MaxX, MinY, MaxY, MinZ, MaxZ);

        public static bool operator ==(Box3D lhs, Box3D rhs) => lhs.Equals(rhs);

        public static bool operator !=(Box3D lhs, Box3D rhs) => !lhs.Equals(rhs);

        public override string ToString() => $"[X={MinX}..{MaxX}, Y={MinY}..{MaxY}, Z={MinZ}..{MaxZ}]";
    }
}
