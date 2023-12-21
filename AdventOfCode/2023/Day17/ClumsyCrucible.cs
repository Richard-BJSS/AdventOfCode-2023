using AdventOfCode.Maths.Geometry.Euclidean;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace AdventOfCode._2023.Day17
{
    public sealed class ClumsyCrucible(string[] _heatLossMap)
    {
        public Matrix<int> HeatLossMap { get; set; } = _heatLossMap.ToMatrix(c => int.Parse([c]));

        public static ClumsyCrucible Parse(string rawHeatLossMap) => new(rawHeatLossMap.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

        public Point[] CalculatePathWithMinimumAmountOfHeatLoss(
            int minStraightLineDistance, 
            int maxStraightLineDistance
            )
        {
            if (maxStraightLineDistance <= minStraightLineDistance) throw new ArgumentOutOfRangeException(nameof(maxStraightLineDistance), "Must be > minStraightLineDistance");
            if (minStraightLineDistance < 0) throw new ArgumentOutOfRangeException(nameof(minStraightLineDistance), "Must be >= 0");

            var sz = HeatLossMap.Size;

            var lavaPool = new Point(0, 0);

            var machinePartsFactory = new Point(sz.Width - 1, sz.Height - 1);

            var pathFinder = new ClumsyCrucible.ConstrainedPathFinder(HeatLossMap, minStraightLineDistance, maxStraightLineDistance);

            return pathFinder.LocatePath(lavaPool, machinePartsFactory, Compass.News);
        }


        private sealed class ConstrainedPathNode : PathNode
        {
            private int? _distanceTravelledInStraightLine;

            // Little bit flaky memoizing the result of this calculation given there is nothing stopping the 
            // Parent property being set afterward.  Can always solve this issue later if there is a need to 
            // do so

            public int DistanceTravelledInStraightLine => _distanceTravelledInStraightLine ??= 
                Parent?.Direction == Direction
                ? ((ConstrainedPathNode)Parent).DistanceTravelledInStraightLine + 1
                : (Parent is null ? 0 : 1);
        }


        // TODO - Potential optimisation .. rather than moving +1 node at a time, we can use minStraightLineDistance to hop
        //        forward as many nodes as we know (within bounds of matrix) we are going to force anyway.  Can't do this
        //        overriding the IsObstacle method but might be able to find a way to do it overridding the NodesAdjacentTo method
        //        instead

        private sealed class ConstrainedPathFinder(Matrix<int> heatLossMap, int minStraightLineDistance, int maxStraightLineDistance) 
            : AStarPathFinder<int, ConstrainedPathNode>(heatLossMap)
        {
            protected override int GetCostOfEdge(ConstrainedPathNode movingFrom, ConstrainedPathNode movingTo) => Matrix.ValueAt(movingTo.Point);

            protected override IEqualityComparer<ConstrainedPathNode> SolvedStateEqualityComparer => new ConstrainedSolvedStateEqualityComparer();

            protected override IEnumerable<ConstrainedPathNode> NodesAdjacentTo(ConstrainedPathNode node, Compass compass)
            {
                var distance = node.DistanceTravelledInStraightLine;

                var pt = node.Point;

                var dir = node.Direction;

                var adjs = new List<ConstrainedPathNode>();

                if (dir == Point.Empty)                        // Starting position -> move forward or turn
                {
                    adjs.Add(new() { Point = new Point(pt.X + 1, pt.Y + 0), Direction = new Point(+1, 0) });
                    adjs.Add(new() { Point = new Point(pt.X + 0, pt.Y + 1), Direction = new Point(0, +1) });
                    adjs.Add(new() { Point = new Point(pt.X + 0, pt.Y - 1), Direction = new Point(0, -1) });
                }
                else if (distance < minStraightLineDistance)   // Can only go straight on
                {
                    adjs.Add(new() { Point = new Point(pt.X + dir.X, pt.Y + dir.Y), Direction = dir });
                }
                else                                           // Can move in any of the 3 supported directions or just turn
                { 
                    if (distance < maxStraightLineDistance)   
                    {
                        adjs.Add(new() { Point = new Point(pt.X + dir.X, pt.Y + dir.Y), Direction = dir });
                    }

                    var cw = dir.RotateCW();
                    var ccw = dir.RotateCCW();

                    adjs.Add(new() { Point = new Point(pt.X + cw.X, pt.Y + cw.Y), Direction = cw });
                    adjs.Add(new() { Point = new Point(pt.X + ccw.X, pt.Y + ccw.Y), Direction = ccw });
                }

                // Last rule ... has to finish on a straight line run that at least meets the minimum length constraint

                var rect = Matrix.Rectangle;

                var endPos = new Point(rect.Width - 1, rect.Height - 1);

                foreach (var adj in adjs)
                {
                    if (adj.Point == endPos)
                    {
                        if (adj.Direction != node.Direction)
                        {
                            if (1 < minStraightLineDistance) continue;
                        }
                        else if ((node.DistanceTravelledInStraightLine + 1) < minStraightLineDistance) continue;
                    }

                    yield return adj;
                }
            }
        }

        private sealed class ConstrainedSolvedStateEqualityComparer
            : EqualityComparer<ConstrainedPathNode>
        {
            public override bool Equals(ConstrainedPathNode? x, ConstrainedPathNode? y)
            {
                if (x is null & y is null) return true;
                if (x is null) return false;
                if (y is null) return false;

                if (x.Point != y.Point) return false;

                if (x.Direction != y.Direction) return false;

                if (x.DistanceTravelledInStraightLine != y.DistanceTravelledInStraightLine) return false;

                return true;
            }

            public override int GetHashCode([DisallowNull] ConstrainedPathNode obj) => obj.GetHashCode();
        }
    }
}
