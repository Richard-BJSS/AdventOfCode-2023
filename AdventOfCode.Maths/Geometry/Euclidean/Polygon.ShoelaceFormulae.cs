namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public sealed partial class Polygon
    {
        public static class ShoelaceFormulae
        {
            public static long AreaOfSimplePolygon(Polygon polygon)
            {
                // Only works on Simple closed Polygons - where edges do not cross over each other
                // http://en.wikipedia.org/wiki/Polygon#Area_and_centroid

                // A = 1/2 * sum(x[i]*y[i+1] - x[i+1]*y[i])

                // NB - added the mod (%) check in case the polygon isn't closed (rather than creating another array to close it off)

                var area = 0L;

                var points = polygon.Points;

                var len = points.Length;

                for (var n = 0; n < len; n++)
                {
                    area += ((long)points[n].X * (long)points[(n + 1) % len].Y) - ((long)points[(n + 1) % len].X * (long)points[n].Y);
                }

                return Math.Abs(area) / 2;
            }
        }
    }
}
