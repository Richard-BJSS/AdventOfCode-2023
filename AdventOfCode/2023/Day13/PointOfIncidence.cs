namespace AdventOfCode._2023.Day13
{
    public static class PointOfIncidence
    {
        public static int EvaluateRawPattern(string rawPattern)
        {
            var rows = rawPattern.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var rowsAbove = EvaluatePattern(rows);

            var cols = Geometry.Rotate90CW(rows);

            var colsToLeft = EvaluatePattern(cols.ToArray());

            return colsToLeft + (100 * rowsAbove);
        }

        private static int EvaluatePattern(string[] pattern)
        {
            var rowPairs = pattern.Zip(pattern.Skip(1)).Select((p, n) => (Pair: p, Idx: n)).Where(p => p.Pair.First == p.Pair.Second).Select(p => (p.Idx, p.Idx + 1));

            var indexes = rowPairs.Select(p => p.Item2).ToArray();

            return DeterminePointOfIncidence(indexes, pattern);
        }

        private static int DeterminePointOfIncidence(int[] indexes, string[] data)
        {
            if (indexes is null || 0 >= indexes.Length) return 0;

            foreach (var idx in indexes)
            {
                var isPointOfIncidence = false;

                var m = Math.Min(idx, data.Length - idx); 

                for (var n = 0; n < m; n++)
                {
                    if (!(isPointOfIncidence = data[idx - n - 1] == data[idx + n]))
                    {
                        break;
                    }
                }

                if (isPointOfIncidence) return idx;
            }

            return 0;
        }
    }
}
