namespace AdventOfCode.Tests._2023.Day_9
{
    [TestClass]
    public sealed class MirageMaintenanceTests
    {
        [DataTestMethod]
        [DataRow("0 3 6 9 12 15", 18)]
        [DataRow("1 3 6 10 15 21", 28)]
        [DataRow("10 13 16 21 30 45", 68)]
        public void Interpolation_CorrectlyPredictsNextValue(string rawSensorOutput, long expectedPrediction)
        {
            var seq = rawSensorOutput.Split(' ').Select(long.Parse).ToArray();

            var actualPrediction = LinearInterpolation.Predict(seq);

            Assert.AreEqual(expectedPrediction, actualPrediction);
        }

        [TestMethod]
        public async Task SumOfPredictedValues_FromSequencesInFile_CorrectlyCalculated()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawSensorOutputs = File.ReadLinesAsync("2023/Day 9/Mirage-Maintenance-File.txt", cts.Token);

            var seqs = rawSensorOutputs.SelectAwait(o => ValueTask.FromResult(o.Split(' ').Select(long.Parse).ToArray()));

            var predictions = seqs.SelectAwait(LinearInterpolation.PredictAsync);

            var sum = await predictions.SumAsync();

            Assert.AreEqual(1584748274, sum);
        }

        [TestMethod]
        public async Task SumOfPredictedValues_FromSequencesInFile_CorrectlyCalculated_WhenReversed()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawSensorOutputs = File.ReadLinesAsync("2023/Day 9/Mirage-Maintenance-File.txt", cts.Token);

            var seqs = rawSensorOutputs.SelectAwait(o => ValueTask.FromResult(o.Split(' ').Select(long.Parse).Reverse().ToArray()));

            var predictions = seqs.SelectAwait(LinearInterpolation.PredictAsync);

            var sum = await predictions.SumAsync();

            Assert.AreEqual(1026, sum);
        }
    }
}