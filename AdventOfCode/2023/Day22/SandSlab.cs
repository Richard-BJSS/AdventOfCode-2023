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

    public sealed class Brick(int id, BoundingBox3D boundingBox)
        : IEquatable<Brick>
    {
        public int Id => id;

        public BoundingBox3D BoundingBox { get; set; } = boundingBox;

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

            // First things first, let us sort the bricks by their lowest point on the Z plane (the plane they are falling down)
            // We will then find it easier to find the other bricks that are touching it once ours has come to rest
            // (we'll traverse the brick collection bottom up - resting bricks to falling bricks, and we'll wait for them to
            // fall on top of us)

            fallingBricks = fallingBricks.OrderBy(brick => brick.BoundingBox.Min(pos => pos.Z));

            var q = new Queue<Brick>(fallingBricks);

            while (q.Count > 0)
            {
                var fallingBrick = q.Dequeue();

                // Check if the brick we have just taken off of the queue can 'fall' (it is not being supported 
                // by another brick lower than it).  If so, let it drop by one unit (the force of gravity) and
                // keep doing so until it hits another brick (or the bottom) and comes to rest.

                var fallingBricksBoundary = fallingBrick.BoundingBox;

                while (!fallingBricksBoundary.Any(pt3D => knownLocationOfRestingBricks.ContainsKey(pt3D) || pt3D.Z <= 0))
                {
                    fallingBrick.BoundingBox = fallingBricksBoundary;

                    fallingBricksBoundary = fallingBricksBoundary.Shift(amount: ForceOfGravity);
                }

                // Our brick has now come to rest on either the floor, or another brick.  Time to build up our graph and add all the edges
                // that describe how our brick connects to any others that might be lying adjacent to it

                foreach (var pointOnFallingBrick in fallingBricksBoundary)
                {
                    if (knownLocationOfRestingBricks.TryGetValue(pointOnFallingBrick, out var adjacentBrick))
                    {
                        bricksAtRest.Add(startsFrom: fallingBrick.Id, endsAt: adjacentBrick.Id);
                    }
                }

                // Last step, keep track of all the points the corners of our brick occupy, so we can use this 
                // information to figure out the bricks resting upon it

                foreach (var pointOnRestingBrick in fallingBrick.BoundingBox)
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
