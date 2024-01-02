using AdventOfCode._2023.Day22;

namespace AdventOfCode.Tests._2023.Day22
{
    [TestClass]
    public sealed class SandSlabTests
    {
        [TestMethod]
        public async Task Bricks_CorrectlyCategorised_WhenAtRest_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawSnapshotOfFallingBricks = await File.ReadAllLinesAsync("2023/Day22/Sand-Slab-File.txt", cts.Token);

            var bricks = Bricks.Parse(rawSnapshotOfFallingBricks).Values;

            var restingBricks = SandSlab.WaitUntilAllBricksHaveComeToRestOnEachOther(bricks);

            var unsupportedBricks = bricks.Select(brick => restingBricks.EdgeEndPoints[brick.Id].All(supported => restingBricks.EdgeStartPoint[supported].Count > 1));

            var actualPart1Result = unsupportedBricks.Count();

            Assert.AreEqual(1329, actualPart1Result);

            var actualPart2Result = SandSlab.CalculateNumberOfSupportedBricks(bricks, restingBricks);

            Assert.AreEqual(69915, actualPart2Result);
        }
    }
}
