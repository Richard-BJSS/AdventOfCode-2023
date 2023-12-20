using AdventOfCode.Maths;
using System.Diagnostics.CodeAnalysis;
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
            News = North | East | South | West,
        }

        // This class represents the edge (Parent.Point, Point) and its a direction, thus is a vector
        // It is also commonly referred to as the 'State' of a node in the algorithm, that is used to 
        // determine the equality with other 'states' that have already been collected, and thus the 
        // arbitrator of when we can terminate one of the paths we are considering

        public class PathNode
        {
            private PathNode? _parent;
            private int? _distanceTravelled;

            public int DistanceTravelled => _distanceTravelled ??= Parent?.DistanceTravelled + 1 ?? 0;


            public int? TotalCost => RollingCost + Heuristic;   // F in Djikstra's Algo
            public int RollingCost = int.MaxValue;              // G in Djikstra's Algo
            public int? Heuristic;                              // H in A* Algo ... difference between Djikstra and A* is the use of the heuristic

            public int? Weight;                                 // The weight of the edge (Parent.Point, Point) - cost of navigating to this node from the parent
            public Point Direction;                             // The direction of travel into the node

            public Point Point = Point.Empty;                   // The location of this node
            public PathNode? Parent                             // The previous node in the path (if there is one)
            {
                get { return _parent; }
                set { _parent = value; _distanceTravelled = default; }
            }

            public IEnumerable<Point> BacktrackToOrigin()
            {
                yield return Point;

                var parent = Parent;

                while (parent is not null)
                {
                    yield return parent.Point;

                    parent = parent.Parent;
                }
            }

            public override bool Equals(object? obj) => obj is PathNode && Point == ((PathNode)obj).Point;

            public override int GetHashCode() => Point.GetHashCode();

            public override string ToString() => $"[{Point}] = C({RollingCost}) + H({Heuristic}) = {TotalCost}";
        }

        public abstract class PathFinder<T, N>(Matrix<T> matrix)
            where N : PathNode, new()
        {
            public Matrix<T> Matrix { get; init; } = matrix;

            public abstract Point[] LocatePath(Point startFrom, Point endAt, Compass compass = Compass.Default);

            protected virtual IEnumerable<N> NodesAdjacentTo(N node, Compass compass)
            {
                var x = node.Point.X;
                var y = node.Point.Y;

                if (((compass & Compass.North) == Compass.North))         yield return new N() { Point = new(x + 0, y - 1), Direction = new( 0, -1) };
                if (((compass & Compass.NorthEast) == Compass.NorthEast)) yield return new N() { Point = new(x + 1, y - 1), Direction = new(+1, -1) };
                if (((compass & Compass.East) == Compass.East))           yield return new N() { Point = new(x + 1, y + 0), Direction = new(+1,  0) };
                if (((compass & Compass.SouthEast) == Compass.SouthEast)) yield return new N() { Point = new(x + 1, y + 1), Direction = new(+1, +1) };
                if (((compass & Compass.South) == Compass.South))         yield return new N() { Point = new(x + 0, y + 1), Direction = new( 0, +1) };
                if (((compass & Compass.SouthWest) == Compass.SouthWest)) yield return new N() { Point = new(x - 1, y + 1), Direction = new(-1, +1) };
                if (((compass & Compass.West) == Compass.West))           yield return new N() { Point = new(x - 1, y + 0), Direction = new(-1,  0) };
                if (((compass & Compass.NorthWest) == Compass.NorthWest)) yield return new N() { Point = new(x - 1, y - 1), Direction = new(-1, -1) };
            }

            protected virtual bool IsObstacle(N movingFrom, N movingTo) => false;

            protected virtual int GetCostOfEdge(N movingFrom, N movingTo) => 0;
        }

        public class AStarPathFinder<T>(Matrix<T> matrix) : AStarPathFinder<T, PathNode>(matrix) { }

        public class AStarPathFinder<T, N>(Matrix<T> matrix) : PathFinder<T, N>(matrix)
            where N : PathNode, new()
        { 
            public override Point[] LocatePath(Point startFrom, Point endAt, Compass compass = Compass.Default)
            {
                var start = new N { 
                    Point = startFrom,
                    Direction = Point.Empty,
                    RollingCost = 0, 
                    Heuristic = Geometry.ChebyshevDistance(startFrom, endAt) 
                };

                var open = new PriorityQueue<N, int?>([(start, default)]);

                var solved = new HashSet<N>(SolvedStateEqualityComparer);

                var lowestCostNode = default(N);

                var bounds = Matrix.Rectangle;

                while (0 < open.Count)
                {
                    lowestCostNode = open.Dequeue();

                    if (lowestCostNode.Point == endAt) break;

                    solved.Add(lowestCostNode);

                    var adjacents = NodesAdjacentTo(lowestCostNode, compass)
                                        .Where(adj => bounds.Contains(adj.Point))
                                        .Where(adj => !IsObstacle(lowestCostNode, adj));

                    foreach (var adj in adjacents)
                    {
                        // TODO: Good idea to add checks for circular paths, or do we let them go on the understanding that 
                        //       their cost will always exceed (trending toward infinity) and it will never get to the end?

                        // Detect paths that are backtracking from whence we came; disqualify them as this algo is for 
                        // one way directional graphs

                        if (adj.Point == lowestCostNode.Parent?.Point) continue;

                        var costOfGettingToAdjNodeFromLowestCostNode = GetCostOfEdge(lowestCostNode, adj);

                        if (costOfGettingToAdjNodeFromLowestCostNode > adj.RollingCost) continue;

                        if (lowestCostNode.RollingCost + costOfGettingToAdjNodeFromLowestCostNode >= adj.RollingCost) continue;

                        // Because we support moving diagonally we use the Chebyshev Distance (movement pattern of a King in Chess) over 
                        // the Manhattan Distance (appropriate heuristic if only moving NEWS on the compass)

                        adj.Weight = costOfGettingToAdjNodeFromLowestCostNode;
                        adj.Heuristic = Geometry.ChebyshevDistance(adj.Point, endAt);
                        adj.RollingCost = lowestCostNode.RollingCost + costOfGettingToAdjNodeFromLowestCostNode;
                        adj.Parent = lowestCostNode;

                        if (solved.Contains(adj)) continue;
                                                
                        open.Enqueue(adj, adj.TotalCost);
                    }
                }

                if (lowestCostNode is null || lowestCostNode.Point != endAt) return [];

                return lowestCostNode.BacktrackToOrigin().Reverse().ToArray();
            }

            protected virtual IEqualityComparer<N> SolvedStateEqualityComparer { get; } = new SolvedPathNodeEqualityComparer();

            private sealed class SolvedPathNodeEqualityComparer
                : EqualityComparer<N>
            {
                public override bool Equals(N? x, N? y)
                {
                    if (x is null & y is null) return true;
                    if (x is null) return false;
                    if (y is null) return false;

                    if (x.Point != y.Point) return false;

                    return true;
                }

                public override int GetHashCode([DisallowNull] N obj) => obj.GetHashCode();
            }
        }
    }
}
