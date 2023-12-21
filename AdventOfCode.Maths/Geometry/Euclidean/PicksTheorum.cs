namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public sealed partial class Polygon
    {
        public static class PicksTheorum
        {
            public static long InternalAreaOfSimplePolygon(long area, long perimeter) => area - (perimeter / 2) + 1;
            public static long InternalAreaOfSimplePolygon(Polygon polygon, long perimeter) => InternalAreaOfSimplePolygon(ShoelaceFormulae.AreaOfSimplePolygon(polygon), perimeter);
        }
    }
}
