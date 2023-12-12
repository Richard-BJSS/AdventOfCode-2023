using AdventOfCode.Maths;

namespace AdventOfCode.Tests
{
    [TestClass]
    public sealed class MatrixTests
    {
        [TestMethod]
        public void RotateMatrix_CorrectlyRotatesBy90Clockwise()
        {
            var rawMap = new[] {
                ".1..6.....",
                "......3...",
                "9..2...7..",
                ".........4",
                "...5....8.",
            };

            var ms = rawMap.Select(s => s.ToCharArray()).ToArray();

            var matrix = new Matrix<char>(ms);

            var rotatedMatrix = matrix.Rotate90CW();

            var rawMatrix = rotatedMatrix.Select(cs => new string(cs)).ToArray();

            Assert.AreEqual(rawMap[0].Length, rawMatrix.Length);
            Assert.AreEqual(rawMap.Length, rawMatrix[0].Length);

            Assert.AreEqual("..9..", rawMatrix[0]);
            Assert.AreEqual("....1", rawMatrix[1]);
            Assert.AreEqual(".....", rawMatrix[2]);
            Assert.AreEqual("5.2..", rawMatrix[3]);
            Assert.AreEqual("....6", rawMatrix[4]);
            Assert.AreEqual(".....", rawMatrix[5]);
            Assert.AreEqual("...3.", rawMatrix[6]);
            Assert.AreEqual("..7..", rawMatrix[7]);
            Assert.AreEqual("8....", rawMatrix[8]);
            Assert.AreEqual(".4...", rawMatrix[9]);
        }
    }
}
