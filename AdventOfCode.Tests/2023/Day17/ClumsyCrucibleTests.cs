using AdventOfCode._2023.Day17;
using System.Drawing;

namespace AdventOfCode.Tests._2023.Day17
{
    [TestClass]
    public sealed class ClumsyCrucibleTests
    {
        [DataTestMethod]
        [DataRow(102, 0, 3, "2413432311323\r\n3215453535623\r\n3255245654254\r\n3446585845452\r\n4546657867536\r\n1438598798454\r\n4457876987766\r\n3637877979653\r\n4654967986887\r\n4564679986453\r\n1224686865563\r\n2546548887735\r\n4322674655533")]
        [DataRow(71, 4, 10, "111111111111\r\n999999999991\r\n999999999991\r\n999999999991\r\n999999999991")]
        [DataRow(94, 4, 10, "2413432311323\r\n3215453535623\r\n3255245654254\r\n3446585845452\r\n4546657867536\r\n1438598798454\r\n4457876987766\r\n3637877979653\r\n4654967986887\r\n4564679986453\r\n1224686865563\r\n2546548887735\r\n4322674655533")]
        public void HeatLoss_IsMinimised_WhenCalculatingMostEfficientPath(int expectedHeatLossOnJourney, int minStraightLineDistance, int maxStraigtLineDistance, string rawHeatLossMap)
        {
            var clumsyCrucible = ClumsyCrucible.Parse(rawHeatLossMap);

            var path = clumsyCrucible.CalculatePathWithMinimumAmountOfHeatLoss(minStraightLineDistance, maxStraigtLineDistance);

            var mtrx = clumsyCrucible.HeatLossMap;

            var actualHeatLossOnJourney = path.Skip(1).Select(mtrx.ValueAt).Sum();

            var visual = new Geometry.Polygon(path).Visualise();

            Assert.AreEqual(expectedHeatLossOnJourney, actualHeatLossOnJourney);
        }

        [DataTestMethod]
        [DataRow(956, 0, 3)]   
        [DataRow(1106, 4, 10)]
        public async Task HeatLoss_IsMinimised_WhenCalculatingMostEfficientPath_FromFile(
            int expectedHeatLossOnJourney, 
            int minStraightLineDistance,
            int maxStraightLineDistance
            )
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawHeatLossMap = await File.ReadAllTextAsync("2023/Day17/Clumsy-Crucible-File.txt", cts.Token);

            var clumsyCrucible = ClumsyCrucible.Parse(rawHeatLossMap);

            var mtrx = clumsyCrucible.HeatLossMap;

            var path = clumsyCrucible.CalculatePathWithMinimumAmountOfHeatLoss(minStraightLineDistance, maxStraightLineDistance);

            var actualHeatLossOnJourney = path.Skip(1).Select(mtrx.ValueAt).Sum();

            Assert.AreEqual(expectedHeatLossOnJourney, actualHeatLossOnJourney); 
        }
    }
}
