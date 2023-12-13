﻿namespace AdventOfCode.Tests
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

            var compass = Geometry.Compass.North | Geometry.Compass.East | Geometry.Compass.South | Geometry.Compass.West;

            var edge = expandedGalaxy.Cells(c => char.IsNumber(c) && (int.Parse(c.ToString()) == galaxyFrom || int.Parse(c.ToString()) == galaxyTo)).ToArray();

            Assert.AreEqual(2, edge.Length);

            var actualPathLength = Geometry.PathFinder.LocateShortestPath<char>(expandedGalaxy, (edge[0], edge[1]), compass).Length - 1;

            Assert.AreEqual(expectedPathLength, actualPathLength);
        }

    }
}