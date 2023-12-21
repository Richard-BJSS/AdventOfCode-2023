using AdventOfCode._2023.Day20;

namespace AdventOfCode.Tests._2023.Day20
{
    [TestClass]
    public sealed class PulsePropagationTests
    {
        [TestMethod]
        public async Task NetworkSimulator_CorrectlyRecordsPulses()
        {
            var rawModuleConfig = "broadcaster -> a, b, c\r\n%a -> b\r\n%b -> c\r\n%c -> inv\r\n&inv -> a";

            var rawModules = rawModuleConfig.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var network = await Network.ParseAsync(rawModules.ToAsyncEnumerable());

            var simResult = network.Simulate(1);

            Assert.AreEqual(4, simResult.Pulses.Where(kvp => kvp.Key).Sum(kvp => kvp.Value));
            Assert.AreEqual(8, simResult.Pulses.Where(kvp => !kvp.Key).Sum(kvp => kvp.Value));

            var actualResult = simResult.Pulses.Values.Aggregate(1L, (product, cnt) => product * cnt);

            Assert.AreEqual(8 * 4, actualResult);
        }

        [TestMethod]
        public async Task NetworkSimulator_CorrectlyRecordsPulses_WhenButtonPressedMultipleTimes()
        {
            const int btnCnt = 2;

            var rawModuleConfig = "broadcaster -> a, b, c\r\n%a -> b\r\n%b -> c\r\n%c -> inv\r\n&inv -> a";

            var rawModules = rawModuleConfig.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var network = await Network.ParseAsync(rawModules.ToAsyncEnumerable());

            var simResult = network.Simulate(btnCnt);

            Assert.AreEqual(4 * btnCnt, simResult.Pulses.Where(kvp => kvp.Key).Sum(kvp => kvp.Value));
            Assert.AreEqual(8 * btnCnt, simResult.Pulses.Where(kvp => !kvp.Key).Sum(kvp => kvp.Value));

            var actualResult = simResult.Pulses.Values.Aggregate(1L, (product, cnt) => product * cnt);

            Assert.AreEqual(8 * 4 * Math.Pow(btnCnt, 2), actualResult);
        }

        [TestMethod]
        public async Task NetworkSimulator_CorrectlyRecordsPulses_WhenButtonPressedMultipleTimes_WithSteppedAssertions()
        {
            var rawModuleConfig = "broadcaster -> a\r\n%a -> inv, con\r\n&inv -> b\r\n%b -> con\r\n&con -> output";

            var rawModules = rawModuleConfig.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var network = await Network.ParseAsync(rawModules.ToAsyncEnumerable());

            var simResult = network.Simulate(1);

            Assert.AreEqual(4 * 1, simResult.Pulses.Where(kvp => kvp.Key).Sum(kvp => kvp.Value));
            Assert.AreEqual(4 * 1, simResult.Pulses.Where(kvp => !kvp.Key).Sum(kvp => kvp.Value));

            simResult = network.Simulate(1);

            Assert.AreEqual(2 * 1, simResult.Pulses.Where(kvp => kvp.Key).Sum(kvp => kvp.Value));
            Assert.AreEqual(4 * 1, simResult.Pulses.Where(kvp => !kvp.Key).Sum(kvp => kvp.Value));

            simResult = network.Simulate(1);

            Assert.AreEqual(3 * 1, simResult.Pulses.Where(kvp => kvp.Key).Sum(kvp => kvp.Value));
            Assert.AreEqual(5 * 1, simResult.Pulses.Where(kvp => !kvp.Key).Sum(kvp => kvp.Value));

            simResult = network.Simulate(1);

            Assert.AreEqual(2 * 1, simResult.Pulses.Where(kvp => kvp.Key).Sum(kvp => kvp.Value));
            Assert.AreEqual(4 * 1, simResult.Pulses.Where(kvp => !kvp.Key).Sum(kvp => kvp.Value));
        }

        [TestMethod]
        public async Task NetworkSimulator_CorrectlyRecordsPulses_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawModuleConfig = File.ReadLinesAsync("2023/Day20/Pulse-Propagation-File.txt", cts.Token);

            var network = await Network.ParseAsync(rawModuleConfig);

            var simResult = network.Simulate(1000);

            var actualResult = simResult.Pulses.Values.Aggregate(1L, (product, cnt) => product * cnt);

            Assert.AreEqual(800830848, actualResult);
        }

        [TestMethod]
        public async Task FewestNumberOfCycles_CalculatedCorrectly_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawModuleConfig = File.ReadLinesAsync("2023/Day20/Pulse-Propagation-File.txt", cts.Token);

            var network = await Network.ParseAsync(rawModuleConfig);

            var simResult = network.Simulate(25000);

            var rx = simResult.Rx;

            Assert.IsNotNull(rx);

            var cycles = rx.InboundModules.Select(m => rx.Counters[m.Name][^1] - rx.Counters[m.Name][^2]);

            var actualResult = Numerics.LowestCommonMultiple(cycles);

            Assert.AreEqual(244055946148853, actualResult);
        }
    }
}
