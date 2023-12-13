using AdventOfCode._2023.Day13;

namespace AdventOfCode.Tests._2023.Day13
{
    [TestClass]
    public sealed class PointOfIncidenceTests
    {
        [DataTestMethod]
        [DataRow(5, "#.##..##.\r\n..#.##.#.\r\n##......#\r\n##......#\r\n..#.##.#.\r\n..##..##.\r\n#.#.##.#.")]
        [DataRow(400, "#...##..#\r\n#....#..#\r\n..##..###\r\n#####.##.\r\n#####.##.\r\n..##..###\r\n#....#..#")]
        [DataRow(405, "#.##..##.\r\n..#.##.#.\r\n##......#\r\n##......#\r\n..#.##.#.\r\n..##..##.\r\n#.#.##.#.\r\n\r\n#...##..#\r\n#....#..#\r\n..##..###\r\n#####.##.\r\n#####.##.\r\n..##..###\r\n#....#..#")]
        [DataRow(1000, ".....#########.\r\n#.#..#...#..#..\r\n.####...#.#.#..\r\n#####...#.#.#..\r\n#.#..#...#..#..\r\n.....#########.\r\n###..#....####.\r\n.#...###..#...#\r\n...#########.#.\r\n##.##.##.#####.\r\n##.##.##.#####.\r\n...#########.#.\r\n.#...###..#...#\r\n###..#....####.\r\n.....#########.\r\n#.#..#...#..#..\r\n#####...#.#.#..")]
        [DataRow(1, "..#.##.#.\r\n##..#..#.\r\n..##....#\r\n..##....#\r\n##..#..#.\r\n....####.\r\n###.#..#.")]
        [DataRow(8, "#.####...\r\n..#.##.##\r\n##....#..\r\n##....#..\r\n..#..#.##\r\n..####...\r\n#.#..#.##")]
        [DataRow(100, "#####.##.\r\n#####.##.\r\n#...##..#\r\n#.#..##.#\r\n.###..###\r\n..##..###\r\n#....#..#\r\n")]
        [DataRow(600, "#.#.##..#\r\n#....#..#\r\n.###..###\r\n..##..###\r\n#....#.##\r\n#####.##.\r\n#####.##.")]
        public void Patterns_SummaryCalculatedCorrectly(long expectedSummary, string rawPatterns)
        {
            var patterns = rawPatterns.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var actualSummary = patterns.Select(PointOfIncidence.EvaluateRawPattern).Sum();

            Assert.AreEqual(expectedSummary, actualSummary);
        }

        [TestMethod]
        public async Task Patterns_SummaryCalculatedCorrectly_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawPatterns = await File.ReadAllTextAsync("2023/Day13/Point-Of-Incidence-File.txt", cts.Token);

            var rawSplitPatterns = rawPatterns.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var actualSummary = rawSplitPatterns.Select(PointOfIncidence.EvaluateRawPattern).Sum();

            Assert.AreEqual(37718, actualSummary);
        }
    }
}
