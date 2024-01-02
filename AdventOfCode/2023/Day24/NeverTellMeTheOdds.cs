using System.Collections;

namespace AdventOfCode._2023.Day24
{
    public readonly record struct Hailstone(Vector Position, Vector Velocity, int Id);

    public readonly record struct Vector(double X, double Y, double Z);

    public sealed class Hailstones(IEnumerable<Hailstone> Hailstones)
        : IEnumerable<Hailstone>
    {
        public static Hailstones Parse(IEnumerable<string> rawHailstones)
        {
            var id = 0;

            var hailstones = new List<Hailstone>();

            foreach (var rawHailstone in rawHailstones)
            {
                id++;

                var nums = rawHailstone.Split('@').SelectMany(s => s.Split(',')).Select(long.Parse).ToArray();

                var hailstone = new Hailstone(
                    new(nums[0], nums[1], nums[2]),
                    new(nums[3], nums[4], nums[5]),
                    id
                );

                hailstones.Add(hailstone);
            }

            return new Hailstones(hailstones);
        }

        public long CountOfFutureHailstoneCollisions((double Min, double Max) scene)
        {
            var hs = Hailstones.Select(h => (Hailstone: h, Hailstones.Where(p => h != p)));

            var visited = new HashSet<(Hailstone, Hailstone)>();

            var sum = 0L;

            foreach (var (hailstone, otherHailstones) in hs)
            {
                foreach (var otherHailstone in otherHailstones)
                {
                    if (visited.Contains((hailstone, otherHailstone)) || visited.Contains((otherHailstone, hailstone))) continue;

                    visited.Add((otherHailstone, hailstone));

                    var intersect = FindIntersect(hailstone, otherHailstone);

                    if (!intersect.HasValue) continue;

                    var isInBoundsX = scene.Min <= intersect.Value.X && intersect.Value.X <= scene.Max;
                    var isInBoundsY = scene.Min <= intersect.Value.Y && intersect.Value.Y <= scene.Max;

                    if (isInBoundsX && isInBoundsY && !IsInThePast(hailstone, intersect.Value) && !IsInThePast(otherHailstone, intersect.Value)) sum++;
                }
            }

            return sum;
        }

        private static Vector? FindIntersect(Hailstone a, Hailstone b)
        {
            var ma = a.Velocity.Y / a.Velocity.X;
            var mb = b.Velocity.Y / b.Velocity.X;

            if (ma == mb) return default;       // Parallel lines have the same slope and do not intersect

            var ca = a.Position.Y - (ma * a.Position.X);
            var cb = b.Position.Y - (mb * b.Position.X);

            var intersectX = (cb - ca) / (ma - mb);
            var intersectY = (ma * intersectX) + ca;

            return new Vector(intersectX, intersectY, 0);
        }

        private static bool IsInThePast(Hailstone hailstone, Vector intersect)
        {
            bool inPastX = false, inPastY = false;

            if (hailstone.Velocity.X < 0)
            {
                if (intersect.X > hailstone.Position.X) inPastX = true;
            }
            else
            {
                if (intersect.X < hailstone.Position.X) inPastX = true;
            }

            if (hailstone.Velocity.Y < 0)
            {
                if (intersect.Y > hailstone.Position.Y) inPastY = true;
            }
            else
            {
                if (intersect.Y < hailstone.Position.Y) inPastY = true;
            }

            return inPastX && inPastY;
        }

        public IEnumerator<Hailstone> GetEnumerator() => Hailstones.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Hailstones.GetEnumerator();
    }
}
