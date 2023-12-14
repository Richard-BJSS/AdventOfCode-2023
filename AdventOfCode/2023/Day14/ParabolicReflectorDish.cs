namespace AdventOfCode._2023.Day14
{
    public static class ParabolicReflectorDish
    {
        public static long CalculateTotalLoad(string[] platform, long cycles = 0)
        {
            if (0 >= cycles)
            {
                platform = SlideRocks(platform);
            }
            else
            {
                var cache = new Dictionary<string, long>();

                for (var cycle = 0L; cycle < cycles; cycle++)
                {
                    platform = SpinThroughOneCycle(platform);

                    var cacheKey = ToCacheKey(platform);

                    if (cache.TryGetValue(cacheKey, out var i))
                    {
                        var d = cycle - i;

                        cycle += d * ((cycles - cycle) / d);
                    }

                    cache[cacheKey] = cycle;
                }
            }

            var q =
                from x in Enumerable.Range(0, platform[0].Length)
                from y in Enumerable.Range(0, platform.Length)
                where platform[y][x] == 'O'
                select platform.Length - y;

            return q.Sum();
        }

        public static string ToCacheKey(IEnumerable<string> platform) => platform.Aggregate(string.Empty, (a, s) => string.Concat(a, ' ', s).Trim());

        public static string[] SpinThroughOneCycle(string[] platform)
        {
            platform = SlideRocks(platform, Geometry.Compass.North);
            platform = SlideRocks(platform, Geometry.Compass.West);
            platform = SlideRocks(platform, Geometry.Compass.South);
            platform = SlideRocks(platform, Geometry.Compass.East);

            return platform;
        }

        public static string[] SlideRocks(string[] platform, Geometry.Compass compass = Geometry.Compass.North)
        {
            var reverser = default(Func<IEnumerable<string>, IEnumerable<string>>);

            if (compass == Geometry.Compass.North)
            {
                platform = Geometry.Rotate90CCW(platform);
                reverser = Geometry.Rotate90CW;
            }
            else if (compass == Geometry.Compass.West)
            {
            }
            else if (compass == Geometry.Compass.South)
            {
                platform = Geometry.Rotate90CW(platform);
                reverser = Geometry.Rotate90CCW;
            }
            else if (compass == Geometry.Compass.East)
            {
                platform = Geometry.Rotate180(platform);
                reverser = Geometry.Rotate180;
            }

            platform = platform.Select(s =>
            {
                while (s.Contains(".O")) s = s.Replace(".O", "O.");

                return s;

            }).ToArray();

            return (reverser is null) ? platform : reverser(platform).ToArray();
        }
    }
}
