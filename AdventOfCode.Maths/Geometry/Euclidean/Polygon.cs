using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public sealed partial class Polygon(Point[] points)
    {
        private Rectangle? _boundingBox;
        private (Point First, Point Second)[]? _edges;

        public bool IsClosed = points[0] == points[^1];

        public (Point First, Point Second)[] Edges => _edges ??= points.Zip(points.Skip(1)).ToArray();

        public Point[] Points => points;

        public Rectangle BoundingBox => _boundingBox ??= CalculateBoundary(points);

        private static Rectangle CalculateBoundary(Point[] points)
        {
            if (points is null || 0 >= points.Length) return Rectangle.Empty;

            var minX = int.MaxValue;
            var maxX = int.MinValue;

            int minY = minX, maxY = maxX;

            foreach (var pt in points)
            {
                if (pt.X < minX) minX = pt.X;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Y > maxY) maxY = pt.Y;
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public Polygon Close()
        {
            if (IsClosed) return this;

            var r = points.ToArray();

            Array.Resize(ref r, r.Length + 1);

            r[^1] = r[0];

            return new(r);
        }

        public string Visualise()
        {
            var tlX = BoundingBox.Left;
            var tlY = BoundingBox.Top;

            var brX = BoundingBox.Width;
            var brY = BoundingBox.Height;

            var rc = 1 + brY - tlY;
            var cc = 1 + brX - tlX;

            var visual = Enumerable.Repeat(new string('.', cc), rc).Select(s => s.ToCharArray()).ToArray();

            for (var n = 1; n < points.Length; n++)
            {
                var pv = points[n - 1];
                var pt = points[n];

                visual[pt.Y][pt.X] = pv switch
                {
                    _ when pv.Y > pt.Y => '^',
                    _ when pv.Y < pt.Y => 'v',
                    _ when pv.X < pt.X => '>',
                    _ when pv.X > pt.X => '<',
                    _ => '?'
                };
            }

            visual[tlY][tlX] = 'S';
            visual[brY][brX] = 'E';

            return visual.Select(c => new string(c)).Aggregate(string.Empty, (a, s) => string.Concat(a, s, Environment.NewLine));
        }
    }

    public sealed partial class Polygon
    {
        public sealed class EdgeEqualityComparer
            : IEqualityComparer<(Point fst, Point snd)>
        {
            public readonly static EdgeEqualityComparer BiDirectional = new();

            public bool Equals((Point fst, Point snd) x, (Point fst, Point snd) y) =>
                (x.fst == y.snd && x.snd == y.fst) ||
                (x.fst == y.fst && x.snd == y.snd);

            public int GetHashCode([DisallowNull] (Point fst, Point snd) obj)
                => HashCode.Combine(
                    Math.Min(obj.fst.X, obj.snd.X),
                    Math.Min(obj.fst.Y, obj.snd.Y)
                    );
        }
    }

    public static partial class GeometryExtensions
    {
        public static IEnumerable<Point> ToPoints(this Rectangle rect)
        {
            if (rect.IsEmpty) yield break;

            for (var x = rect.X; x <= (rect.X + rect.Width); x++)
            {
                for (var y = rect.Y; y <= (rect.Y + rect.Height); y++)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }
}
