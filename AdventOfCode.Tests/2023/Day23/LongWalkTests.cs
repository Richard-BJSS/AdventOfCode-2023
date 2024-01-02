using AdventOfCode._2023.Day23;

namespace AdventOfCode.Tests._2023.Day23
{

    [TestClass]
    public sealed class LongWalkTests
    {
        [TestMethod]
        public void LongestPath_CalculatedCorrectly()
        {
            var rawHikingTrailsMap = "#.#####################\r\n#.......#########...###\r\n#######.#########.#.###\r\n###.....#.>.>.###.#.###\r\n###v#####.#v#.###.#.###\r\n###.>...#.#.#.....#...#\r\n###v###.#.#.#########.#\r\n###...#.#.#.......#...#\r\n#####.#.#.#######.#.###\r\n#.....#.#.#.......#...#\r\n#.#####.#.#.#########v#\r\n#.#...#...#...###...>.#\r\n#.#.#v#######v###.###v#\r\n#...#.>.#...>.>.#.###.#\r\n#####v#.#.###v#.#.###.#\r\n#.....#...#...#.#.#...#\r\n#.#########.###.#.#.###\r\n#...###...#...#...#.###\r\n###.###.#.###v#####v###\r\n#...#...#.#.>.>.#.>.###\r\n#.###.###.#.###.#.#v###\r\n#.....###...###...#...#\r\n#####################.#".Split(Environment.NewLine);

            var hikingTrails = HikingTrails.ParseWithSlopes(rawHikingTrailsMap);

            var actualResult = hikingTrails.FindLongestDistance();

            Assert.AreEqual(94, actualResult);

            hikingTrails = HikingTrails.Parse(rawHikingTrailsMap);

            actualResult = hikingTrails.FindLongestDistance();

            Assert.AreEqual(154, actualResult);
        }

        [TestMethod]
        public async Task LongestPath_CalculatedCorrectly_FromFile()
        { 
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawHikingTrailsMap = await File.ReadAllLinesAsync("2023/Day23/Long-Walk-File.txt", cts.Token);

            var hikingTrails = HikingTrails.ParseWithSlopes(rawHikingTrailsMap);

            var actualResult = hikingTrails.FindLongestDistance();

            Assert.AreEqual(2282, actualResult);

            hikingTrails = HikingTrails.Parse(rawHikingTrailsMap);

            actualResult = hikingTrails.FindLongestDistance();

            Assert.AreEqual(6646, actualResult);
        }         
    }
}