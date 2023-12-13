using AdventOfCode._2023.Day11;

namespace AdventOfCode.Tests._2023.Day11
{
    [TestClass]
    public sealed class CosmicExpansionTests
    {
        [TestMethod]
        public void TotalSteps_ForAllCrossGalaxyJourneys_CalculatedCorrectly()
        {
            var rawObsAnalysis = new string[]
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

            var total = CosmicExpansion.CalculateTotalManhattanDistanceBetweenGalaxies(rawObsAnalysis, emptySpaceExpansionMultiplier: 2);

            Assert.AreEqual(374, total);            
        }

        [DataTestMethod]
        [DataRow(2, 9403026)]
        [DataRow(1000000, 543018317006)]
        public async Task TotalSteps_ForAllCrossGalaxyJourneys_CalculatedCorrectly_WithExpansion_FromFile(int expansionMultiplier, long expectedTotal)
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawObsAnalysis = await File.ReadAllLinesAsync("2023/Day11/Cosmic-Expansion-File.txt", cts.Token);

            var total = CosmicExpansion.CalculateTotalManhattanDistanceBetweenGalaxies(rawObsAnalysis, emptySpaceExpansionMultiplier: expansionMultiplier);

            Assert.AreEqual(expectedTotal, total);
        }
    }
}
