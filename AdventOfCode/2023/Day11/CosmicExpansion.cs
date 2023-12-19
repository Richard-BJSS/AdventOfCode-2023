using AdventOfCode.Maths;

namespace AdventOfCode._2023.Day11
{
    public static class CosmicExpansion
    {
        public static long CalculateTotalManhattanDistanceBetweenGalaxies(string[] rawObsAnalysis, int emptySpaceExpansionMultiplier = 0)
        {
            var emptyRows = new List<int>();
            var emptyCols = new List<int>();

            for (var y = 0; y < rawObsAnalysis.Length; y++) if (rawObsAnalysis[y].All(c => c == '.')) emptyRows.Add(y);

            for (var x = 0; x < rawObsAnalysis[0].Length; x++) if (rawObsAnalysis.All(s => s[x] == '.')) emptyCols.Add(x);

            var mx = rawObsAnalysis.Select(s => s.ToCharArray()).ToArray();

            var matrix = new Matrix<char>(mx);

            var galaxies = matrix.Entries(pt => matrix.ValueAt(pt) == '#').ToArray();

            var edges = from g1 in galaxies
                        from g2 in galaxies
                        where g1 != g2
                        select (fst: g1, snd: g2);

            var total = 0L;

            foreach (var (fst, snd) in edges.Distinct(Geometry.EdgeEqualityComparer.BiDirectional))
            {
                var distance = Geometry.ManhattanDistance(fst, snd);

                var minY = Math.Min(fst.Y, snd.Y);
                var maxY = Math.Max(fst.Y, snd.Y);
                var minX = Math.Min(fst.X, snd.X);
                var maxX = Math.Max(fst.X, snd.X);

                var mtYs = Enumerable.Range(minY, maxY - minY).Where(emptyRows.Contains).Count();
                var mtXs = Enumerable.Range(minX, maxX - minX).Where(emptyCols.Contains).Count();

                total += distance + ((mtYs + mtXs) * (emptySpaceExpansionMultiplier - 1));
            }

            return total;
        }

    }
}
