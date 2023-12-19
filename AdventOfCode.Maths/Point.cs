using System.Drawing;

namespace AdventOfCode
{
    public static partial class Geometry
    {
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

    }
}
