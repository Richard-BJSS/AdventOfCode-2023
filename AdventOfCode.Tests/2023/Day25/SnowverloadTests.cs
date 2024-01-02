using AdventOfCode._2023.Day25;

namespace AdventOfCode.Tests._2023.Day25
{
    [TestClass]
    public sealed class SnowverloadTests
    {
        [TestMethod]
        public void CorrectWiresDisconnected_ToCreateTwoSeparateComponentGroups()
        {
            var rawWiringDiagram = "jqt: rhn xhk nvd\r\nrsh: frs pzl lsr\r\nxhk: hfx\r\ncmg: qnr nvd lhk bvb\r\nrhn: xhk bvb hfx\r\nbvb: xhk hfx\r\npzl: lsr hfx nvd\r\nqnr: nvd\r\nntq: jqt hfx bvb xhk\r\nnvd: lhk\r\nlsr: lhk\r\nrzs: qnr cmg lsr rsh\r\nfrs: qnr lhk lsr".Split(Environment.NewLine);

            var wiringDiagram = WiringDiagram.Parse(rawWiringDiagram);

            var cuts = wiringDiagram.FindMinimumCut();

            Assert.AreEqual(2, cuts.Count);

            var actualResult = cuts.Aggregate(1, (p, s) => p * s.Count);

            Assert.AreEqual(54, actualResult);
        }

        [TestMethod]
        public async Task CorrectWiresDisconnected_ToCreateTwoSeparateComponentGroups_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawWiringDiagram = await File.ReadAllLinesAsync("2023/Day25/Snowverload-File.txt", cts.Token);

            var wiringDiagram = WiringDiagram.Parse(rawWiringDiagram);

            var cuts = wiringDiagram.FindMinimumCut();

            Assert.AreEqual(2, cuts.Count);

            var actualResult = cuts.Aggregate(1, (p, s) => p * s.Count);

            Assert.AreEqual(554064, actualResult);
        }
    }
}