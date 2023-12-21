using AdventOfCode._2023.Day13;

namespace AdventOfCode.Tests._2023.Day13
{
    [TestClass]
    public sealed class PointOfIncidenceTests
    {
        [DataTestMethod]
        [DataRow(5, 300, "#.##..##.\r\n..#.##.#.\r\n##......#\r\n##......#\r\n..#.##.#.\r\n..##..##.\r\n#.#.##.#.")]
        [DataRow(405, 400, "#.##..##.\r\n..#.##.#.\r\n##......#\r\n##......#\r\n..#.##.#.\r\n..##..##.\r\n#.#.##.#.\r\n\r\n#...##..#\r\n#....#..#\r\n..##..###\r\n#####.##.\r\n#####.##.\r\n..##..###\r\n#....#..#")]
        [DataRow(400, 100, "#...##..#\r\n#....#..#\r\n..##..###\r\n#####.##.\r\n#####.##.\r\n..##..###\r\n#....#..#")]
        [DataRow(405, 400, "#.##..##.\r\n..#.##.#.\r\n##......#\r\n##......#\r\n..#.##.#.\r\n..##..##.\r\n#.#.##.#.\r\n\r\n#...##..#\r\n#....#..#\r\n..##..###\r\n#####.##.\r\n#####.##.\r\n..##..###\r\n#....#..#")]
        [DataRow(1000, 300, ".....#########.\r\n#.#..#...#..#..\r\n.####...#.#.#..\r\n#####...#.#.#..\r\n#.#..#...#..#..\r\n.....#########.\r\n###..#....####.\r\n.#...###..#...#\r\n...#########.#.\r\n##.##.##.#####.\r\n##.##.##.#####.\r\n...#########.#.\r\n.#...###..#...#\r\n###..#....####.\r\n.....#########.\r\n#.#..#...#..#..\r\n#####...#.#.#..")]
        [DataRow(1, 6, "..#.##.#.\r\n##..#..#.\r\n..##....#\r\n..##....#\r\n##..#..#.\r\n....####.\r\n###.#..#.")]
        [DataRow(8, 0, "#.####...\r\n..#.##.##\r\n##....#..\r\n##....#..\r\n..#..#.##\r\n..####...\r\n#.#..#.##")]
        [DataRow(100, 0, "#####.##.\r\n#####.##.\r\n#...##..#\r\n#.#..##.#\r\n.###..###\r\n..##..###\r\n#....#..#\r\n")]
        [DataRow(600, 0, "#.#.##..#\r\n#....#..#\r\n.###..###\r\n..##..###\r\n#....#.##\r\n#####.##.\r\n#####.##.")]
        public void Patterns_SummaryCalculatedCorrectly(long expectedP1Summary, long expectedP2Summary, string rawPatterns)
        {
            var patterns = rawPatterns.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var p1 = 0L;
            var p2 = 0L;

            foreach (var pattern in patterns)
            {
                var rows = pattern.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                var cols = GeometryExtensions.Rotate90CW(rows);

                var horizontal = PointOfIncidence.ReflectionScores(rows);
                var vertical = PointOfIncidence.ReflectionScores(cols);

                p1 += 100 * horizontal[0] + vertical[0];
                p2 += 100 * horizontal[1] + vertical[1];
            }

            Assert.AreEqual(expectedP1Summary, p1);
            Assert.AreEqual(expectedP2Summary, p2);
        }

        [TestMethod]
        public async Task Patterns_SummaryCalculatedCorrectly_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawPatterns = await File.ReadAllTextAsync("2023/Day13/Point-Of-Incidence-File.txt", cts.Token);

            var rawSplitPatterns = rawPatterns.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var p1 = 0L;
            var p2 = 0L;

            foreach (var rawPattern in rawSplitPatterns)
            {
                var rows = rawPattern.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                var cols = GeometryExtensions.Rotate90CW(rows);

                var horizontal = PointOfIncidence.ReflectionScores(rows);
                var vertical = PointOfIncidence.ReflectionScores(cols);

                p1 += 100 * horizontal[0] + vertical[0];
                p2 += 100 * horizontal[1] + vertical[1];
            }

            Assert.AreEqual(37718, p1);
            Assert.AreEqual(40995, p2);
        }
    }
}
