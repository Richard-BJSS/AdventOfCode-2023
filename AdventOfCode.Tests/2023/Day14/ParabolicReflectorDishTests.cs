using AdventOfCode._2023.Day14;

namespace AdventOfCode.Tests._2023.Day14
{
    [TestClass]
    public sealed class ParabolicReflectorDishTests
    {
        [DataTestMethod]
        [DataRow(Compass.North, "O....#....\r\nO.OO#....#\r\n.....##...\r\nOO.#O....O\r\n.O.....O#.\r\nO.#..O.#.#\r\n..O..#O..O\r\n.......O..\r\n#....###..\r\n#OO..#....", "OOOO.#.O..\r\nOO..#....#\r\nOO..O##..O\r\nO..#.OO...\r\n........#.\r\n..#....#.#\r\n..O..#.O.O\r\n..O.......\r\n#....###..\r\n#....#....")]
        [DataRow(Compass.South, "O....#....\r\nO.OO#....#\r\n.....##...\r\nOO.#O....O\r\n.O.....O#.\r\nO.#..O.#.#\r\n..O..#O..O\r\n.......O..\r\n#....###..\r\n#OO..#....", ".....#....\r\n....#....#\r\n...O.##...\r\n...#......\r\nO.O....O#O\r\nO.#..O.#.#\r\nO....#....\r\nOO....OO..\r\n#OO..###..\r\n#OO.O#...O")]
        [DataRow(Compass.West,  "O....#....\r\nO.OO#....#\r\n.....##...\r\nOO.#O....O\r\n.O.....O#.\r\nO.#..O.#.#\r\n..O..#O..O\r\n.......O..\r\n#....###..\r\n#OO..#....", "O....#....\r\nOOO.#....#\r\n.....##...\r\nOO.#OO....\r\nOO......#.\r\nO.#O...#.#\r\nO....#OO..\r\nO.........\r\n#....###..\r\n#OO..#....")]
        [DataRow(Compass.East,  "O....#....\r\nO.OO#....#\r\n.....##...\r\nOO.#O....O\r\n.O.....O#.\r\nO.#..O.#.#\r\n..O..#O..O\r\n.......O..\r\n#....###..\r\n#OO..#....", "....O#....\r\n.OOO#....#\r\n.....##...\r\n.OO#....OO\r\n......OO#.\r\n.O#...O#.#\r\n....O#..OO\r\n.........O\r\n#....###..\r\n#..OO#....")]
        public void SlideRocks_CorrectlyAppliesOrientation_WhenCalculatingNewPositionOfRocks(Compass compass, string rawPlatform, string expectedRawPlatform)
        {
            var platform = rawPlatform.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            platform = ParabolicReflectorDish.SlideRocks(platform, compass);

            var actualRawPlatform = ParabolicReflectorDish.ToCacheKey(platform);

            expectedRawPlatform = ParabolicReflectorDish.ToCacheKey(expectedRawPlatform.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

            Assert.AreEqual(expectedRawPlatform, actualRawPlatform);
        }

        [DataTestMethod]
        [DataRow("O....#....\r\nO.OO#....#\r\n.....##...\r\nOO.#O....O\r\n.O.....O#.\r\nO.#..O.#.#\r\n..O..#O..O\r\n.......O..\r\n#....###..\r\n#OO..#....", ".....#....\r\n....#...O#\r\n...OO##...\r\n.OO#......\r\n.....OOO#.\r\n.O#...O#.#\r\n....O#....\r\n......OOOO\r\n#...O###..\r\n#..OO#....", ".....#....\r\n....#...O#\r\n.....##...\r\n..O#......\r\n.....OOO#.\r\n.O#...O#.#\r\n....O#...O\r\n.......OOO\r\n#..OO###..\r\n#.OOO#...O", ".....#....\r\n....#...O#\r\n.....##...\r\n..O#......\r\n.....OOO#.\r\n.O#...O#.#\r\n....O#...O\r\n.......OOO\r\n#...O###.O\r\n#.OOO#...O")]
        public void SpinThroughOneCycle_CorrectlyRotatesThePlatformThrough360DegreesWithSliding(string rawPlatform, params string[] expectedCycledRawPlatforms)
        {
            var platform = rawPlatform.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            for (var n = 0; n < expectedCycledRawPlatforms.Length; n++)
            {
                platform = ParabolicReflectorDish.SpinThroughOneCycle(platform);

                var actualCycledRawPlatform = ParabolicReflectorDish.ToCacheKey(platform);

                var expectedCycledRawPlatform = ParabolicReflectorDish.ToCacheKey(expectedCycledRawPlatforms[n].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

                Assert.AreEqual(expectedCycledRawPlatform, actualCycledRawPlatform);
            }
        }

        [DataTestMethod]
        [DataRow(64, 1000000000, "O....#....\r\nO.OO#....#\r\n.....##...\r\nOO.#O....O\r\n.O.....O#.\r\nO.#..O.#.#\r\n..O..#O..O\r\n.......O..\r\n#....###..\r\n#OO..#....")]
        [DataRow(136, 0, "O....#....\r\nO.OO#....#\r\n.....##...\r\nOO.#O....O\r\n.O.....O#.\r\nO.#..O.#.#\r\n..O..#O..O\r\n.......O..\r\n#....###..\r\n#OO..#....")]
        public void TotalLoadOfPlatform_CalculatedCorrectly_WithSpinCycle(int expectedTotalLoad, int cycles, string rawPlatform)
        {
            var platform = rawPlatform.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var actualTotalLoad = ParabolicReflectorDish.CalculateTotalLoad(platform, cycles);

            Assert.AreEqual(expectedTotalLoad, actualTotalLoad);
        }

        [DataTestMethod]
        [DataRow(0L, 109654L)]
        [DataRow(1000000000L, 94876L)]
        public async Task TotalLoadOfPlatform_CalculatedCorrectly_FromFile(long cycles, long expectedTotalLoad)
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var platform = await File.ReadAllLinesAsync("2023/Day14/Parabolic-Reflection-Dish-File.txt", cts.Token);

            var actualTotalLoad = ParabolicReflectorDish.CalculateTotalLoad(platform, cycles);

            Assert.AreEqual(expectedTotalLoad, actualTotalLoad);
        }
    }
}
