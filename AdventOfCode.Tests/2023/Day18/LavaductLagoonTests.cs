using AdventOfCode._2023.Day18;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode.Geometry;

namespace AdventOfCode.Tests._2023.Day18
{
    [TestClass]
    public sealed class LavaductLagoonTests
    {
        [DataTestMethod]
        [DataRow(false, 62, "R 6 (#70c710)\r\nD 5 (#0dc571)\r\nL 2 (#5713f0)\r\nD 2 (#d2c081)\r\nR 2 (#59c680)\r\nD 2 (#411b91)\r\nL 5 (#8ceee2)\r\nU 2 (#caa173)\r\nL 1 (#1b58a2)\r\nU 2 (#caa171)\r\nR 2 (#7807d2)\r\nU 3 (#a77fa3)\r\nL 2 (#015232)\r\nU 2 (#7a21e3)")]
        [DataRow(true, 952408144115, "R 6 (#70c710)\r\nD 5 (#0dc571)\r\nL 2 (#5713f0)\r\nD 2 (#d2c081)\r\nR 2 (#59c680)\r\nD 2 (#411b91)\r\nL 5 (#8ceee2)\r\nU 2 (#caa173)\r\nL 1 (#1b58a2)\r\nU 2 (#caa171)\r\nR 2 (#7807d2)\r\nU 3 (#a77fa3)\r\nL 2 (#015232)\r\nU 2 (#7a21e3)")]
        public void VolumeOfHole_CorrectlyCalculated(bool hexParse, long expectedVolumeOfHole, string rawDigPlan)
        {
            var lavaductLagoon = hexParse 
                                 ? LavaductLagoon.ParseUsingHex(rawDigPlan)
                                 : LavaductLagoon.Parse(rawDigPlan);

            var actualVolumeOfHole = lavaductLagoon.CalculateVolumeOfHole(); 

            Assert.AreEqual(expectedVolumeOfHole, actualVolumeOfHole);
        }

        [TestMethod]
        public async Task VolumeOfHole_CorrectlyCalculated_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawDigPlan = await File.ReadAllTextAsync("2023/Day18/Lavaduct-Lagoon-File.txt", cts.Token);

            var lavaductLagoon = LavaductLagoon.Parse(rawDigPlan);

            var actualVolumeOfHole = lavaductLagoon.CalculateVolumeOfHole();

            Assert.AreEqual(42317, actualVolumeOfHole);
        }

        [TestMethod]
        public async Task VolumeOfHole_CorrectlyCalculated_UsingHex_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawDigPlan = await File.ReadAllTextAsync("2023/Day18/Lavaduct-Lagoon-File.txt", cts.Token);

            var lavaductLagoon = LavaductLagoon.ParseUsingHex(rawDigPlan);

            var actualVolumeOfHole = lavaductLagoon.CalculateVolumeOfHole();

            Assert.AreEqual(83605563360288, actualVolumeOfHole);
        }
    }
}
