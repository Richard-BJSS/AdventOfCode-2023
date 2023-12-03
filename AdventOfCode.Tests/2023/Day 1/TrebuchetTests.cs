using AdventOfCode._2023.Day_1;

namespace AdventOfCode.Tests._2023.Day_1
{
    /// <summary>
    /// https://adventofcode.com/2023/day/1
    /// </summary>
    [TestClass]
    public sealed class TrebuchetTests
    {
        [DataTestMethod]
        [DataRow("1abc2", 12)]
        [DataRow("pqr3stu8vwx", 38)]
        [DataRow("a1b2c3d4e5f", 15)]
        [DataRow("treb7uchet", 77)]
        public async Task CalibrateUsingDigits_CanBeUsedToTotaliseExampleData(string exampleToCalibrate, int expectedResult)
        {
            var toBeCalibrated = new[] { exampleToCalibrate }.ToAsyncEnumerable();

            var actualResult = await Trebuchet.CalibrateUsingDigitsAsync(toBeCalibrated).SumAsync();

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public async Task CalibrateUsingDigits_CanBeUsedToTotaliseDocument()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var toBeCalibrated = File.ReadLinesAsync("2023/Day 1/Calibration-Input-File.txt", cts.Token);

            var actualResult = await Trebuchet.CalibrateUsingDigitsAsync(toBeCalibrated).SumAsync();

            Assert.AreEqual(54338, actualResult);
        }

        [DataTestMethod]
        [DataRow("two1nine", 29)]
        [DataRow("eightwothree", 83)]
        [DataRow("abcone2threexyz", 13)]
        [DataRow("xtwone3four", 24)]
        [DataRow("4nineeightseven2", 42)]
        [DataRow("zoneight234", 14)]
        [DataRow("7pqrstsixteen", 76)]
        public async Task CalibrateUsingDigitsAndWords_CanBeUsedToTotaliseExampleData(string exampleToCalibrate, int expectedResult)
        {
            var toBeCalibrated = new[] { exampleToCalibrate }.ToAsyncEnumerable();

            var actualResult = await Trebuchet.CalibrateUsingDigitsAndWordsAsync(toBeCalibrated).SumAsync();

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public async Task CalibrateUsingDigitsAndWords_CanBeUsedToTotaliseDocument()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var toBeCalibrated = File.ReadLinesAsync("2023/Day 1/Calibration-Input-File.txt", cts.Token);

            var actualResult = await Trebuchet.CalibrateUsingDigitsAndWordsAsync(toBeCalibrated).SumAsync();

            Assert.AreEqual(53389, actualResult);
        }
    }
}
