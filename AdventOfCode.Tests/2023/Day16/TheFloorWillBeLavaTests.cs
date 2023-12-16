using AdventOfCode._2023.Day16;

namespace AdventOfCode.Tests._2023.Day16
{
    [TestClass]
    public sealed class TheFloorWillBeLavaTests
    {
        [TestMethod]
        public void NumberOfEnergisedTiles_CalculatedCorrectly()
        {
            var rawContraptionLayout = ".|...\\....\r\n|.-.\\.....\r\n.....|-...\r\n........|.\r\n..........\r\n.........\\\r\n..../.\\\\..\r\n.-.-/..|..\r\n.|....-|.\\\r\n..//.|....";

            var contraptionLayout = rawContraptionLayout.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var entryPoint = (x: 0, y: 0, dx: 1, dy: 0);

            var energisedTileCount = TheFloorWillBeLava.FindEnergisedTilesFromPointOfEntry(contraptionLayout, entryPoint).Count();

            Assert.AreEqual(46, energisedTileCount);
        }

        [TestMethod]
        public async Task NumberOfEnergisedTiles_CalculatedCorrectly_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawContraptionLayout = await File.ReadAllTextAsync("2023/Day16/Lava-Floor-File.txt", cts.Token);

            var contraptionLayout = rawContraptionLayout.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var entryPoint = (x: 0, y: 0, dx: 1, dy: 0);

            var energisedTileCount = TheFloorWillBeLava.FindEnergisedTilesFromPointOfEntry(contraptionLayout, entryPoint).Count();

            Assert.AreEqual(7496, energisedTileCount);
        }

        [TestMethod]
        public void MaximumEnergisedTileCount_CalculatedCorrectly()
        {
            var rawContraptionLayout = ".|...\\....\r\n|.-.\\.....\r\n.....|-...\r\n........|.\r\n..........\r\n.........\\\r\n..../.\\\\..\r\n.-.-/..|..\r\n.|....-|.\\\r\n..//.|....";

            var contraptionLayout = rawContraptionLayout.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var maxEnergisedTileCount = TheFloorWillBeLava.CalculateMaximumPossibleNumberOfEnergisedTiles(contraptionLayout);

            Assert.AreEqual(51, maxEnergisedTileCount);
        }

        [TestMethod]
        public async Task MaximumEnergisedTileCount_CalculatedCorrectly_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawContraptionLayout = await File.ReadAllTextAsync("2023/Day16/Lava-Floor-File.txt", cts.Token);

            var contraptionLayout = rawContraptionLayout.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var maxEnergisedTileCount = TheFloorWillBeLava.CalculateMaximumPossibleNumberOfEnergisedTiles(contraptionLayout);

            Assert.AreEqual(7932, maxEnergisedTileCount);
        }
    }
}
