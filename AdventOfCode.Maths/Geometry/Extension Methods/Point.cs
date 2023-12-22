using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public static partial class PointExtensions
    {
        public static int ManhattanDistance(Point a, Point b) => Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
        public static int ManhattanDistance(Point3D a, Point3D b) => Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y) + Math.Abs(b.Z - a.Z);

        public static int ChebyshevDistance(Point a, Point b) => Math.Max(Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y));
        public static int ChebyshevDistance(Point3D a, Point3D b)
        {            
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);
            var dz = Math.Abs(a.Z - b.Z);

            return new[] { dx, dy, dz }.Max();
        }

        public static Point RotateCCW(this Point pointToRotate, Point centreOfRotation = default) => new(pointToRotate.Y - centreOfRotation.Y + centreOfRotation.X, centreOfRotation.X - pointToRotate.X + centreOfRotation.Y);

        public static Point RotateCW(this Point pointToRotate, Point centreOfRotation = default) => new(centreOfRotation.Y - pointToRotate.Y + centreOfRotation.X, pointToRotate.X - centreOfRotation.X + centreOfRotation.Y);

        public static Point Rotate(this Point pointToRotate, Point centreOfRotation, double angleToRotateInDegrees) 
        {
            var angleInRadians = angleToRotateInDegrees * (Math.PI / 180);

            var cosTheta = Math.Cos(angleInRadians);
            var sinTheta = Math.Sin(angleInRadians);

            return new Point
            {
                X = (int)(cosTheta * (pointToRotate.X - centreOfRotation.X) - sinTheta * (pointToRotate.Y - centreOfRotation.Y) + centreOfRotation.X),
                Y = (int)(sinTheta * (pointToRotate.X - centreOfRotation.X) + cosTheta * (pointToRotate.Y - centreOfRotation.Y) + centreOfRotation.Y)
            };
        }

        public static IEnumerable<(Point Point, Point Direction)> PointsAdjacentTo(this Point pt, Compass compass)
        {
            var x = pt.X;
            var y = pt.Y;

            if (((compass & Compass.North)     == Compass.North))     yield return (new(x + 0, y - 1), new( 0, -1));
            if (((compass & Compass.NorthEast) == Compass.NorthEast)) yield return (new(x + 1, y - 1), new(+1, -1));
            if (((compass & Compass.East)      == Compass.East))      yield return (new(x + 1, y + 0), new(+1,  0));
            if (((compass & Compass.SouthEast) == Compass.SouthEast)) yield return (new(x + 1, y + 1), new(+1, +1));
            if (((compass & Compass.South)     == Compass.South))     yield return (new(x + 0, y + 1), new( 0, +1));
            if (((compass & Compass.SouthWest) == Compass.SouthWest)) yield return (new(x - 1, y + 1), new(-1, +1));
            if (((compass & Compass.West)      == Compass.West))      yield return (new(x - 1, y + 0), new(-1,  0));
            if (((compass & Compass.NorthWest) == Compass.NorthWest)) yield return (new(x - 1, y - 1), new(-1, -1));
        }
    }
}
