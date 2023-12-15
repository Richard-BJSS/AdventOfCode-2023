using AdventOfCode._2023.Day15;

namespace AdventOfCode.Tests._2023.Day15
{
    [TestClass]
    public sealed class LensLibraryTests
    {
        [TestMethod]
        public void ComputeHash_CalculatesCorrectValue()
        {
            var source = "HASH";

            var hash = LensLibrary.ComputeHash(source);

            Assert.AreEqual(52, hash);
        }

        [TestMethod]
        public void FocusingPower_CalculatedCorrectly()
        {
            var initSeq = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";

            var lensLibrary = LensLibrary.Parse(initSeq);

            var actualFocusingPower = lensLibrary.ComputeFocusingPower();

            Assert.AreEqual(145, actualFocusingPower);
        }

        [TestMethod]
        public async Task ComputeHash_CalculatesCorrectValue_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var source = await File.ReadAllTextAsync("2023/Day15/Lens-Library-File.txt", cts.Token);

            var hash = source.Split(',', StringSplitOptions.RemoveEmptyEntries).Sum(LensLibrary.ComputeHash);

            Assert.AreEqual(512797, hash);
        }

        [TestMethod]
        public async Task FocusingPower_CalculatesCorrectValue_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var initSeq = await File.ReadAllTextAsync("2023/Day15/Lens-Library-File.txt", cts.Token);

            var lensLibrary = LensLibrary.Parse(initSeq);

            var actualFocusingPower = lensLibrary.ComputeFocusingPower();

            Assert.AreEqual(262454, actualFocusingPower);
        }
    }
}
