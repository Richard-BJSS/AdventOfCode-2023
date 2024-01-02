using AdventOfCode.Maths.Geometry.Euclidean;

namespace AdventOfCode._2023.Day22
{
    public static class Bricks
    {
        public static IReadOnlyDictionary<int, Brick> Parse(string[] rawBricks)
        {
            var r = new Dictionary<int, Brick>(rawBricks.Length);

            for (var n = 0; n < rawBricks.Length; n++)
            {
                var xs = rawBricks[n].Split('~');

                var coords = xs[0].Split(',').Select(int.Parse).ToArray();

                var vl = new Point3D(coords[0], coords[1], coords[2]);

                coords = xs[1].Split(',').Select(int.Parse).ToArray();

                var vr = new Point3D(coords[0], coords[1], coords[2]);

                r[n] = new(n, new([vl, vr], inclusive: true));
            }

            return r;
        }
    }

    public sealed class Brick(int id, Box3D surface)
        : IEquatable<Brick>
    {
        public int Id => id;

        public Box3D Surface { get; set; } = surface;

        bool IEquatable<Brick>.Equals(Brick? other) => other?.Id == id;

        public override bool Equals(object? obj) => obj is Brick other && Equals(other);

        public override int GetHashCode() => Id.GetHashCode();
    }

    public static class SandSlab
    {
        private static readonly Point3D ForceOfGravity = new(0, 0, -1);

        public static DirectedGraph<int> WaitUntilAllBricksHaveComeToRestOnEachOther(IEnumerable<Brick> fallingBricks)
        {
            var knownLocationOfRestingBricks = new Dictionary<Point3D, Brick>();

            var bricksAtRest = new DirectedGraph<int>();

            fallingBricks = fallingBricks.OrderBy(brick => brick.Surface.Min(pos => pos.Z));

            var q = new Queue<Brick>(fallingBricks);

            while (q.Count > 0)
            {
                var fallingBrick = q.Dequeue();

                var fallingBricksBoundary = fallingBrick.Surface;

                while (!fallingBricksBoundary.Any(pt3D => knownLocationOfRestingBricks.ContainsKey(pt3D) || pt3D.Z <= 0))
                {
                    fallingBrick.Surface = fallingBricksBoundary;

                    fallingBricksBoundary = fallingBricksBoundary.Shift(amount: ForceOfGravity);
                }

                foreach (var pointOnFallingBrick in fallingBricksBoundary)
                {
                    if (knownLocationOfRestingBricks.TryGetValue(pointOnFallingBrick, out var adjacentBrick))
                    {
                        bricksAtRest.Add(startsFrom: fallingBrick.Id, endsAt: adjacentBrick.Id);
                    }
                }

                foreach (var pointOnRestingBrick in fallingBrick.Surface)
                {
                    knownLocationOfRestingBricks.Add(pointOnRestingBrick, fallingBrick);
                }
            }

            return bricksAtRest;
        }

        public static int CalculateNumberOfSupportedBricks(IEnumerable<Brick> bricksInTheHeap, DirectedGraph<int> bricksAtRest)
        {
            var sum = 0;

            foreach (var brick in bricksInTheHeap)
            {
                var q = new Queue<int>([brick.Id]);

                var restingBrickIds = new HashSet<int>([brick.Id]);

                while (q.Count > 0)
                {
                    var brickToExamine = q.Dequeue();


                    foreach (var adjBrick in bricksAtRest.EdgeEndPoints[brickToExamine])
                    {
                        if (bricksAtRest.EdgeStartPoint[adjBrick].All(restingBrickIds.Contains))
                        {
                            q.Enqueue(adjBrick);

                            restingBrickIds.Add(adjBrick);
                        }
                    }
                }

                sum += restingBrickIds.Count - 1;
            }

            return sum;
        }
    }
}
