using AdventOfCode.Maths;

namespace AdventOfCode.Tests._2023.Day11
{
    [TestClass]
    public sealed class CosmicExpansionTests
    {
        [TestMethod]
        public void TotalSteps_ForAllCrossGalaxyJourneys_CalculatedCorrectly()
        {
            var rawObsAnalysis = new List<string>
            {
                "...#......",
                ".......#..",
                "#.........",
                "..........",
                "......#...",
                ".#........",
                ".........#",
                "..........",
                ".......#..",
                "#...#.....",
            };

            var total = CalculateTotalDistance(2, rawObsAnalysis);

            Assert.AreEqual(374, total);            
        }

        [DataTestMethod]
        [DataRow(2, 9403026)]
        [DataRow(1000000, 543018317006)]
        public async Task TotalSteps_ForAllCrossGalaxyJourneys_CalculatedCorrectly_WithExpansion_FromFile(int expansionMultiplier, long expectedTotal)
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawObsAnalysis = (await File.ReadAllLinesAsync("2023/Day11/Cosmic-Expansion-File.txt", cts.Token)).ToList();

            var total = CalculateTotalDistance(expansionMultiplier, rawObsAnalysis);

            Assert.AreEqual(expectedTotal, total);
        }

        private static long CalculateTotalDistance(int expansionMultiplier, List<string> rawObsAnalysis)
        {
            var emptyRows = new List<int>();
            var emptyCols = new List<int>();

            for (var y = 0; y < rawObsAnalysis.Count; y++) if (rawObsAnalysis[y].All(c => c == '.')) emptyRows.Add(y);

            for (var x = 0; x < rawObsAnalysis[0].Length; x++) if (rawObsAnalysis.All(s => s[x] == '.')) emptyCols.Add(x);

            var mx = rawObsAnalysis.Select(s => s.ToCharArray()).ToArray();

            var galaxies = new Matrix<char>(mx).Cells(e => e == '#').ToArray();

            // TODO - twice as many edges as needed.  Refactor when we have a comparison method that considers 
            //        both the forward and backward journeys to be the same

            var edges = from g1 in galaxies
                        from g2 in galaxies
                        where g1 != g2
                        select (fst: g1, snd: g2);

            var total = 0L;

            foreach (var (fst, snd) in edges)
            {
                var distance = Geometry.ManhattanDistance(fst, snd);

                var minY = Math.Min(fst.Y, snd.Y);
                var maxY = Math.Max(fst.Y, snd.Y);
                var minX = Math.Min(fst.X, snd.X);
                var maxX = Math.Max(fst.X, snd.X);

                var mtYs = Enumerable.Range(minY, maxY - minY).Where(emptyRows.Contains).Count();
                var mtXs = Enumerable.Range(minX, maxX - minX).Where(emptyCols.Contains).Count();

                total += distance + ((mtYs + mtXs) * (expansionMultiplier - 1));
            }

            return total / 2;
        }
    }
}
