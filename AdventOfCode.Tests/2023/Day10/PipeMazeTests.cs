using AdventOfCode._2023.Day10;
using System.Drawing;

namespace AdventOfCode.Tests._2023.Day10
{
    [TestClass]
    public sealed class PipeMazeTests
    {
        [DataTestMethod]
        [DataRow(4, 1, 1, ".....", ".S-7.", ".|.|.", ".L-J.", ".....")]
        [DataRow(4, 1, 1, "-L|F7", "7S-7|", "L|7||", "-L-J|", "L|-JF")]
        [DataRow(8, 0, 2, "..F7.", ".FJ|.", "SJ.L7", "|F--J", "LJ...")]
        [DataRow(8, 0, 2, "7-F7-", ".FJ|7", "SJLL7", "|F--J", "LJ.LJ")]
        public void MapPath_CorrectlyFollowsLoop_UntilBackAtStart(int expectedTicks, int x, int y, params string[] rawMap)
        {
            var (map, start) = PipeMap.Parse(rawMap);

            Assert.IsNotNull(start);
            Assert.AreEqual(new Point(x, y), start);

            var paths = map.StartingFrom(start.Value);

            paths[0].ContinueUntil(p => p.Ticks > 0 && p.CurrentLocation == start);

            var actualValue = paths[0].Ticks / 2;

            Assert.AreEqual(expectedTicks, actualValue);
        }

        [TestMethod]
        public async Task MapPath_CorrectlyFollowsLoop_UntilBackAtStart_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawMap = await File.ReadAllLinesAsync("2023/Day10/Pipe-Maze-File.txt", cts.Token);

            var (map, start) = PipeMap.Parse(rawMap);

            Assert.IsNotNull(start);

            var paths = map.StartingFrom(start.Value);

            paths[0].ContinueUntil(p => p.Ticks > 0 && p.CurrentLocation == start);

            var actualValue = paths[0].Ticks / 2;
            
            Assert.AreEqual(6768, actualValue);
        }

        [TestMethod]
        public async Task MapPath_ConvertedToPolygon_ExtractsInternalPoints_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawMap = await File.ReadAllLinesAsync("2023/Day10/Pipe-Maze-File.txt", cts.Token);

            var (map, start) = PipeMap.Parse(rawMap);

            Assert.IsNotNull(start);

            var paths = map.StartingFrom(start.Value);

            paths[0].ContinueUntil(p => p.Ticks > 0 && p.CurrentLocation == start);

            var pointsAlongPath = paths[0].LocationsVisited;

            var polygon = new Geometry.Polygon([.. pointsAlongPath]);

            var w = rawMap[0].Length;
            var h = rawMap.Length;

            var internalPoints = 0;

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var c = rawMap[y][x];

                    var expectedIsInternal = (c == 'I');

                    if (Geometry.WindingNumberAlgorithm.IsPointLocatedInsidePolygon(new Point(x, y), polygon))
                    {
                        internalPoints++;
                    }
                }
            }

            Assert.AreEqual(351, internalPoints);
        }

        [DataTestMethod]
        [DataRow("F|L-J|7-F", ".....\r\n.S-7.\r\n.|.|.\r\n.L-J.\r\n.....")]
        [DataRow("F|L-J|7-F", "-L|F7\r\n7S-7|\r\nL|7||\r\n-L-J|\r\nL|-JF")]
        [DataRow("F|LJF--J7L|7FJFJF", "..F7.\r\n.FJ|.\r\nSJ.L7\r\n|F--J\r\nLJ...")]
        [DataRow("F|LJF--J7L|7FJFJF", "7-F7-\r\n.FJ|7\r\nSJLL7\r\n|F--J\r\nLJ.LJ")]
        [DataRow("F|||||L--J|7-L||F-----7||J-F|L--J|||||7-------F", "...........\r\n.S-------7.\r\n.|F-----7|.\r\n.||.....||.\r\n.||.....||.\r\n.|L-7.F-J|.\r\n.|..|.|..|.\r\n.L--J.L--J.\r\n...........")]
        [DataRow("FJFL7JF|JL|||7F|JL7FJFL-7J---LFJ|7LF-J7L7LFJ7--F|L7J--LFJ||F----7||LJ||F7||LJ||F7|||LJ|||F7||LJ||F-7JFL7L-7L7L7|JL7L7L7-FL7L7||JL|7F|JL||||7F", ".F----7F7F7F7F-7....\r\n.|F--7||||||||FJ....\r\n.||.FJ||||||||L7....\r\nFJL7L7LJLJ||LJ.L-7..\r\nL--J.L7...LJS7F-7L7.\r\n....F-J..F7FJ|L7L7L7\r\n....L7.F7||L7|.L7L7|\r\n.....|FJLJ|FJ|F7|.LJ\r\n....FJL-7.||.||||...\r\n....L---J.LJ.LJLJ...")]
        public void PathMap_CreatesCorrectPath(string expectedPath, string mapSchematic)
        {
            var rawMap = mapSchematic.Split(Environment.NewLine);

            var (map, start) = PipeMap.Parse(rawMap);

            Assert.IsNotNull(start);

            var path = map.StartingFrom(start.Value)[0];

            _ = path.ContinueUntil(p => p.Ticks > 0 && p.CurrentLocation == start);

            var pts = path.LocationsVisited;

            var p = new string(pts.Select(pt => map.ValueAt(pt) ?? default).ToArray());

            var pr = new string(p.Reverse().ToArray());

            Assert.IsTrue(expectedPath == p || expectedPath == pr);
        }

        [DataTestMethod]
        [DataRow("..........\r\n.S------7.\r\n.|F----7|.\r\n.||OOOO||.\r\n.||OOOO||.\r\n.|L-7F-J|.\r\n.|II||II|.\r\n.L--JL--J.\r\n..........")]
        [DataRow("OF----7F7F7F7F-7OOOO\r\nO|F--7||||||||FJOOOO\r\nO||OFJ||||||||L7OOOO\r\nFJL7L7LJLJ||LJIL-7OO\r\nL--JOL7IIILJS7F-7L7O\r\nOOOOF-JIIF7FJ|L7L7L7\r\nOOOOL7IF7||L7|IL7L7|\r\nOOOOO|FJLJ|FJ|F7|OLJ\r\nOOOOFJL-7O||O||||OOO\r\nOOOOL---JOLJOLJLJOOO")]
        [DataRow("FF7FSF7F7F7F7F7F---7\r\nL|LJ||||||||||||F--J\r\nFL-7LJLJ||||||LJL-77\r\nF--JF--7||LJLJIF7FJ-\r\nL---JF-JLJIIIIFJLJJ7\r\n|F|F-JF---7IIIL7L|7|\r\n|FFJF7L7F-JF7IIL---7\r\n7-L-JL7||F7|L7F-7F7|\r\nL.L7LFJ|||||FJL7||LJ\r\nL7JLJL-JLJLJL--JLJ.L")]
        public void WindingNumberAlgo_CorrectlyDeterminesWhenPointIsInsidePolygon(string rawSchematic)
        {
            var rawMap = rawSchematic.Split(Environment.NewLine);

            var (map, start) = PipeMap.Parse(rawMap);

            Assert.IsNotNull(start);

            var path = map.StartingFrom(start.Value)[0];
            
            _ = path.ContinueUntil(p => p.Ticks > 0 && p.CurrentLocation == start);

            var polygon = new Geometry.Polygon(path.LocationsVisited.ToArray());

            var w = rawMap[0].Length;
            var h = rawMap.Length;

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var c = rawMap[y][x];

                    var expectedIsInternal = (c == 'I');

                    var actualIsInternal = Geometry.WindingNumberAlgorithm.IsPointLocatedInsidePolygon(new Point(x, y), polygon);

                    Assert.AreEqual(expectedIsInternal, actualIsInternal);
                }
            }
        }

    }
}
