namespace AdventOfCode.Tests
{
    [TestClass]
    public sealed class PathFinderTests
    {
        [DataTestMethod]
        [DataRow(5, 9, 9)]
        [DataRow(1, 7, 15)]
        [DataRow(3, 6, 17)]
        [DataRow(8, 9, 5)]
        public void ShortestPaths_CorrectlyDiscovered(int galaxyFrom, int galaxyTo, int expectedPathLength)
        {
            var rawObsAnalysis = new List<string>
            {
                "....1........",
                ".........2...",
                "3............",
                ".............",
                ".............",
                "........4....",
                ".5...........",
                "............6",
                ".............",
                ".............",
                ".........7...",
                "8....9......."
            };

            var yss = rawObsAnalysis.Select(s => s.ToCharArray()).ToArray();

            var expandedGalaxy = yss.ToMatrix();

            var edge = expandedGalaxy
                .Where(e => char.IsNumber(e.Value) && (int.Parse([e.Value]) == galaxyFrom || int.Parse([e.Value]) == galaxyTo))
                .Select(e => e.Point)
                .ToArray();

            Assert.AreEqual(2, edge.Length);

            var pathFinder = new AStarPathFinder<char>(expandedGalaxy);

            var actualPathLength = pathFinder.LocatePath(edge[0], edge[1], Compass.News).Length - 1;

            Assert.AreEqual(expectedPathLength, actualPathLength);
        }
    }
}
