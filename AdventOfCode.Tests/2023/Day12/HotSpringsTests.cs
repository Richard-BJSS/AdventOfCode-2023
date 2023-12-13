using AdventOfCode._2023.Day12;

namespace AdventOfCode.Tests._2023.Day12
{
    [TestClass]
    public sealed class HotSpringsTests
    {
        [DataTestMethod]
        [DataRow(1, "???.### 1,1,3")]
        [DataRow(4, ".??..??...?##. 1,1,3")]
        [DataRow(1, "?#?#?#?#?#?#?#? 1,3,1,6")]
        [DataRow(1, "????.#...#... 4,1,1")]
        [DataRow(4, "????.######..#####. 1,6,5")]
        [DataRow(10, "?###???????? 3,2,1")]
        public void SpringRecord_CorrectlyCalculates_PossibleArrangements(long expectedCount, string rawRecord)
        {
            var sr = HotSprings.SpringConditionRecord.Parse(rawRecord);

            var actualCount = sr.CalculateNumberOfPossibleArrangements();

            Assert.AreEqual(expectedCount, actualCount);
        }

        [DataTestMethod]
        [DataRow(1, 1, "???.### 1,1,3")]
        [DataRow(1, 4, ".??..??...?##. 1,1,3")]
        [DataRow(1, 1, "?#?#?#?#?#?#?#? 1,3,1,6")]
        [DataRow(1, 1, "????.#...#... 4,1,1")]
        [DataRow(1, 4, "????.######..#####. 1,6,5")]
        [DataRow(1, 10, "?###???????? 3,2,1")]
        [DataRow(1, 6, "#.#.### 1,1,3\r\n.#...#....###. 1,1,3\r\n.#.###.#.###### 1,3,1,6\r\n####.#...#... 4,1,1\r\n#....######..#####. 1,6,5\r\n.###.##....# 3,2,1")]
        [DataRow(1, 21, "???.### 1,1,3\r\n.??..??...?##. 1,1,3\r\n?#?#?#?#?#?#?#? 1,3,1,6\r\n????.#...#... 4,1,1\r\n????.######..#####. 1,6,5\r\n?###???????? 3,2,1")]
        //[DataRow(5, 1, "???.### 1,1,3")]
        //[DataRow(5, 16384, ".??..??...?##. 1,1,3")]
        //[DataRow(5, 1, "?#?#?#?#?#?#?#? 1,3,1,6")]
        //[DataRow(5, 16, "????.#...#... 4,1,1")]
        //[DataRow(5, 2500, "????.######..#####. 1,6,5")]
        //[DataRow(5, 506250, "?###???????? 3,2,1")]
        public void ArrangementPossibilities_AreCalculatedCorrectly_WithUnfold(int unfoldFactor, long expectedCount, string rawRecords)
        {
            var rawSpringConditionRecords = rawRecords.Split(Environment.NewLine);

            var records = HotSprings.Parse(rawSpringConditionRecords, unfoldFactor: unfoldFactor);

            var actualCount = records.CalculateTotalNumberOfPossibleArrangementsAcrossAllRecords();

            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod]
        public async Task ArrangementPossibilities_AreCalculatedCorrectly_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawSpringConditionRecords = await File.ReadAllLinesAsync("2023/Day12/Hot-Springs-File.txt", cts.Token);

            var records = HotSprings.Parse(rawSpringConditionRecords);

            var actualCount = records.CalculateTotalNumberOfPossibleArrangementsAcrossAllRecords();

            Assert.AreEqual(7007, actualCount);
        }

        [TestMethod]
        public async Task ArrangementPossibilities_AreCalculatedCorrectly_WithUnfold_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawSpringConditionRecords = await File.ReadAllLinesAsync("2023/Day12/Hot-Springs-File.txt", cts.Token);

            var records = HotSprings.Parse(rawSpringConditionRecords, unfoldFactor: 5);

            var actualCount = records.CalculateTotalNumberOfPossibleArrangementsAcrossAllRecords();

            Assert.AreEqual(7007, actualCount);
        }
    }
}
