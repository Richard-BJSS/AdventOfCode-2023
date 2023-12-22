using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public partial class Polygon
    {
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
    }
}
