using AdventOfCode.Maths.Geometry.Euclidean;
using System.Drawing;

using Graph = AdventOfCode.Utility.LazyDictionary<System.Drawing.Point, System.Collections.Generic.HashSet<System.Drawing.Point>>;

namespace AdventOfCode._2023.Day23
{
    // TODO - replace the Depth First Search with algo at https://www.geeksforgeeks.org/find-longest-path-directed-acyclic-graph/

    public sealed class HikingTrails(Graph graph, Point startAt, Point endAt)
    {
        public static HikingTrails Parse(string[] rawHikingTrailsMap)
        {
            var startAt = new Point(x: rawHikingTrailsMap[0].IndexOf('.'), y: 0);

            var endAt = new Point(x: rawHikingTrailsMap[^1].IndexOf('.'), y: rawHikingTrailsMap.Length - 1);

            var matrix = rawHikingTrailsMap.ToMatrix();

            var graph = new Graph(_ => []);

            foreach (var (pos, _) in matrix)
            {
                graph[pos] = GetAdjacentNodes(matrix, pos);
            }

            return new(graph, startAt, endAt);
        }

        public static HikingTrails ParseWithSlopes(string[] rawHikingTrailsMap)
        {
            var startAt = new Point(x: rawHikingTrailsMap[0].IndexOf('.'), y: 0);

            var endAt = new Point(x: rawHikingTrailsMap[^1].IndexOf('.'), y: rawHikingTrailsMap.Length - 1);

            var matrix = rawHikingTrailsMap.ToMatrix();

            var graph = new Graph(_ => []);

            foreach (var (pos, _) in matrix)
            {
                graph[pos] = GetAdjacentNodesWithSlopes(matrix, pos);
            }

            return new(graph, startAt, endAt);
        }

        public int FindLongestDistance() => FindLongestDistance(graph, startAt, endAt, [], 0);

        private static int FindLongestDistance(Graph graph, Point startAt, Point endAt, HashSet<Point> visited, int distanceAlreadyCovered)
        {
            if (startAt == endAt) return distanceAlreadyCovered;

            visited ??= [];

            var longestDistance = 0;

            foreach (var adjacent in graph[startAt])
            {
                if (visited.Add(adjacent))
                {
                    longestDistance = Math.Max(longestDistance, FindLongestDistance(graph, adjacent, endAt, visited, distanceAlreadyCovered + 1));

                    visited.Remove(adjacent);
                }
            }

            return longestDistance;
        }

        private static HashSet<Point> GetAdjacentNodes(Matrix<char> matrix, Point position) =>
            position.PointsAdjacentTo(Compass.News)
                    .Select(a => a.Point)
                    .Where(adj => matrix.Rectangle.Contains(adj) && matrix[adj] != '#')
                    .ToHashSet();

        private readonly static Dictionary<char, Point> _slopeDirections = new() { { '^', new ( 0, -1) }, { 'v', new ( 0, +1) }, { '<', new (-1,  0) }, { '>', new (+1,  0) }};

        private static HashSet<Point> GetAdjacentNodesWithSlopes(Matrix<char> matrix, Point position)
        {
            IEnumerable<Point> xs = _slopeDirections.TryGetValue(matrix[position], out var direction)
                                    ? new Point[] { new(position.X + direction.X, position.Y + direction.Y) }
                                    : position.PointsAdjacentTo(Compass.News).Select(a => a.Point);

            return xs.Where(adj => matrix.Rectangle.Contains(adj) && matrix[adj] != '#').ToHashSet();
        }
    }
}
