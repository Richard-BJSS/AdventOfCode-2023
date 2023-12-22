using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public static class MatrixExtensions
    {
        public static PeriodicMatrix<T> ToPeriodicMatrix<T>(this Matrix<T> source) => new PeriodicMatrix<T>(source);

        public static IEnumerable<(Point Point, Point Direction)> PointsAdjacentTo<T>(this Matrix<T> source, Point pt, Compass compass) => source.Rectangle.PointsAdjacentTo(pt, compass);

        public static IEnumerable<(Point Point, Point Direction)> PointsAdjacentTo<T>(this PeriodicMatrix<T> source, Point pt, Compass compass) => pt.PointsAdjacentTo(compass);
    }
}
