using System.Drawing;

namespace AdventOfCode
{
    public static partial class Geometry
    {
        [Flags]
        public enum Compass
        {
            North = 1,
            NorthEast = 2, 
            East = 4, 
            SouthEast = 8,
            South = 16,
            SouthWest = 32,
            West = 64,
            NorthWest = 128,

            Default = North | NorthEast | East | SouthEast | South | SouthWest | West | NorthWest,
        }

        public static class PathFinder
        {
            private sealed class PathLocation(Point point)
            { 
                public Point Point = point;
                public int? F => G + H;
                public int? G;
                public int? H;
                public PathLocation? Parent;
            }

            public static Point[] LocateShortestPath<T>(T[,] map, (Point fst, Point snd) edge, Compass compass = Compass.Default)
            {
                var start = new PathLocation(edge.fst);
                var target = new PathLocation(edge.snd);
                
                var open = new List<PathLocation>([start]);
                var closed = new List<Point>();

                var g = 0;

                var current = default(PathLocation);

                while (0 < open.Count)
                {
                    var minF = open.Min(l => l.F);

                    current = open.First(l => l.F == minF);

                    if (current.Point == target.Point) break;

                    closed.Add(current.Point);

                    open.Remove(current);

                    var adjacents = LocationsAdjacentTo(map, current, compass).Where(_ => true); // TODO - eliminate obstacles, walls etc

                    g++;

                    foreach(var adj in adjacents)
                    {
                        if (closed.Contains(adj.Point)) continue;

                        if (open.Contains(adj))
                        {
                            if (g + adj.H < adj.F)
                            {
                                adj.G = g;
                                adj.Parent = current;
                            }
                        }
                        else
                        {
                            adj.G = g;
                            adj.H = CalculateH(adj.Point, target.Point);
                            adj.Parent = current;

                            open.Insert(0, adj);
                        }
                    }
                }

                if (current is null) return [];

                var lst = new List<Point>([current.Point]);

                while (current.Parent is not null) { lst.Insert(0, current.Parent.Point); current = current.Parent; }

                return [..lst];   
            }

            private static int CalculateH(Point pt, Point target) => Math.Abs(target.X - pt.X) + Math.Abs(target.Y - pt.Y);

            private static IEnumerable<PathLocation> LocationsAdjacentTo<T>(T[,] map, PathLocation loc, Compass compass)
            {
                var w = map.GetLength(0);
                var h = map.GetLength(1);

                var rect = new Rectangle(0, 0, w, h);

                var x = loc.Point.X;
                var y = loc.Point.Y;

                if (rect.Contains(x + 0, y - 1) && ((compass & Compass.North) == Compass.North))         yield return new(new(x + 0, y - 1));
                if (rect.Contains(x + 1, y - 1) && ((compass & Compass.NorthEast) == Compass.NorthEast)) yield return new(new(x + 1, y - 1));
                if (rect.Contains(x + 1, y + 0) && ((compass & Compass.East) == Compass.East))           yield return new(new(x + 1, y + 0));
                if (rect.Contains(x + 1, y + 1) && ((compass & Compass.SouthEast) == Compass.SouthEast)) yield return new(new(x + 1, y + 1));
                if (rect.Contains(x + 0, y + 1) && ((compass & Compass.South) == Compass.South))         yield return new(new(x + 0, y + 1));
                if (rect.Contains(x - 1, y + 1) && ((compass & Compass.SouthWest) == Compass.SouthWest)) yield return new(new(x - 1, y + 1));
                if (rect.Contains(x - 1, y + 0) && ((compass & Compass.West) == Compass.West))           yield return new(new(x - 1, y + 0));
                if (rect.Contains(x - 1, y - 1) && ((compass & Compass.NorthWest) == Compass.NorthWest)) yield return new(new(x - 1, y - 1));
            }
        }
    }
}
