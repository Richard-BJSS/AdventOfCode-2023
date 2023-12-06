namespace AdventOfCode._2023.Day_6
{
    public static class WaitForIt
    {
        public static IEnumerable<long> TickingClock(long numOfTicksToStopClockAfter)
        {
            for (var n = 0; n < numOfTicksToStopClockAfter; n++) { yield return n; }
        }

        public static long[,] Parse(string[] xs)
        {
            var time = xs[0][5..].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse).ToArray();
            var dist = xs[1][9..].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse).ToArray();

            var r = new long[time.Length, 2];

            for (var n = 0; n < time.Length; n++)
            {
                r[n, 0] = time[n];
                r[n, 1] = dist[n];
            }

            return r;
        }
    }
}
