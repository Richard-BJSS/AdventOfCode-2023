using System.Drawing;

namespace AdventOfCode
{
    public static partial class Geometry
    {
        public sealed class Polygon(Point[] points)
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
        }

        public static class WindingNumberAlgorithm
        {
            public static bool IsPointLocatedInsidePolygon(Point pt, Polygon polygon)
            {
                if (polygon.Points.Contains(pt)) return false;

                var windingNumber = 0;

                foreach (var edge in polygon.Close().Edges)
                {
                    var (fst, snd) = edge;

                    if (fst.Y <= pt.Y)
                    {
                        if (snd.Y > pt.Y)   
                            if (IsPointToTheLeftOfTheEdge(edge, pt) > 0) 
                               ++windingNumber;
                    }
                    else if (snd.Y <= pt.Y) 
                    {
                        if (IsPointToTheLeftOfTheEdge(edge, pt) < 0) 
                            --windingNumber;
                    }
                }

                return windingNumber != 0;
            }

            private static double IsPointToTheLeftOfTheEdge((Point fst, Point snd) edge, Point pt) =>
                ((edge.snd.X - edge.fst.X) * (pt.Y - edge.fst.Y) -
                 (pt.X - edge.fst.X) * (edge.snd.Y - edge.fst.Y));
        }           

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
