using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public static partial class RectangleExtensions
    {
        public static IEnumerable<(Point Point, Point Direction)> PointsAdjacentTo(this Rectangle rect, Point pt, Compass compass)
        {
            foreach (var adj in pt.PointsAdjacentTo(compass))
            {
                if (rect.Contains(adj.Point)) yield return adj;
            }
        }

    }
}
