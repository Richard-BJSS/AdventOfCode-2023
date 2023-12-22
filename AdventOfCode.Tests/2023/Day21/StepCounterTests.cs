using AdventOfCode._2023.Day21;

namespace AdventOfCode.Tests._2023.Day21
{
    [TestClass]
    public sealed class StepCounterTests
    {
        [TestMethod]
        public void AccessibleGardenPlots_LocatedCorrectly_UsingStepCountConstraint()
        {
            var rawMapOfGardenPlots = "...........\r\n.....###.#.\r\n.###.##..#.\r\n..#.#...#..\r\n....#.#....\r\n.##..S####.\r\n.##..#...#.\r\n.......##..\r\n.##.#.####.\r\n.##..##.##.\r\n...........".Split(Environment.NewLine);

            var mapOfGardenPlots = rawMapOfGardenPlots.ToMatrix();

            var startAt = mapOfGardenPlots.Centre;

            Assert.AreEqual('S', mapOfGardenPlots[startAt]);

            mapOfGardenPlots[startAt] = '.';

            var getAdjGardenPlots = StepCounter.GetAdjacentGardenPlots(mapOfGardenPlots);

            var plots = StepCounter.LocateAccessibleGardenPlotsAtDistances(startAt, getAdjGardenPlots, 6).Take(1).ToArray();

            Assert.AreEqual(16, plots[0]);
        }

        [TestMethod]
        public async Task AccessibleGardenPlots_LocatedCorrectly_UsingStepCountConstraint_AndInfiniteSizedGarden_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawMapOfGardenPlots = await File.ReadAllLinesAsync("2023/Day21/Step-Counter-File.txt", cts.Token);

            var mapOfGardenPlots = rawMapOfGardenPlots.ToMatrix();

            // I suspect there must be a more generic way to solve this puzzle but I don't know what it is.
            //
            // TODO: Research this one because we might be able to extract common abstraction into the PeriodicMatrix class
            //       I am trying to create (and retrofit the Point of Incidence puzzle from Day 13 of 2023
            //       Might have something to do with the Vandermonde Matrix?
            //
            // Based on a few observations of the source data (that feels like a very dirty way to do this)
            // we can note:-

            // 1. We are starting our journey from the exact centre of an infinite, square universe

            var startAt = mapOfGardenPlots.Centre;

            Assert.AreEqual('S', mapOfGardenPlots[startAt]);

            mapOfGardenPlots[startAt] = '.';

            var getAdjGardenPlots = StepCounter.GetAdjacentGardenPlots(mapOfGardenPlots);

            // 2. We are working with a Square Matrix so the centre point splits the matrix into two equal size rectangular matrices

            var size = mapOfGardenPlots.Size;

            Assert.AreEqual(size.Width, size.Height);

            Assert.AreEqual(1, size.Width % 2);

            // 3. The centre vertices only contain empty space, thus in (size / 2) steps we will reach each side of the bounded matrix
            //    if we navigate these paths

            var centreRow = mapOfGardenPlots.RowAt(startAt.Y);
            var centreCol = mapOfGardenPlots.ColAt(startAt.X);

            Assert.IsTrue(centreRow.All(e => e == '.'));
            Assert.IsTrue(centreCol.All(e => e == '.'));

            // 4. This means we can we can replicate (up to the remaining number of steps), by calculating the 
            //    quadratic coefficients (a,b,c) when solving a system of equations

            var takeSampleWhenTheseDistancesTravelled = new[] {
                startAt.X,                          // Walked the entire matrix (i.e. reached the boundaries)
                startAt.X + size.Width,            
                startAt.X + (2 * size.Width)        
            };

            var plots = StepCounter.LocateAccessibleGardenPlotsAtDistances(startAt, getAdjGardenPlots, takeSampleWhenTheseDistancesTravelled).ToArray();

            // At this point we have found all garden plots in a universe the size of 16 replicas of our matrix (4 x 4)

            const int NUMBER_OF_STEPS_REQUIRED = 26501365;

            // A little algebra reminder for my old brain ... quadratic curves:
            //
            // A quadratic equation is any polynomial equation of the second degree with the form ax² + bx + c = 0
            // When x is unknown we can calculate it using..
            //
            // x = -b ± √(b² + 4ac)
            //     ----------------     
            //           2a
            //
            // Similar problem we had on Day 6 of 2023 (Wait for It)
            //
            // plots[0] = c                     
            // plots[1] = a + b + c             
            // plots[2] = 4a + b + c            

            var c = plots[0];                                                      var b = ((4 * plots[1]) - plots[2] - (3 * plots[0])) / 2;  
            var a = plots[1] - plots[0] - b;                            

            var x = (NUMBER_OF_STEPS_REQUIRED - startAt.X) / size.Width;

            var solvedEquation =  (a * x * x) + (b * x) + c;  // ax² + bx + c

            Assert.AreEqual(625587097150084, solvedEquation);
        }

        [TestMethod]
        public async Task AccessibleGardenPlots_LocatedCorrectly_UsingStepCountConstraint_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawMapOfGardenPlots = await File.ReadAllLinesAsync("2023/Day21/Step-Counter-File.txt", cts.Token);

            var mapOfGardenPlots = rawMapOfGardenPlots.ToMatrix();

            var startAt = mapOfGardenPlots.Centre;

            Assert.AreEqual('S', mapOfGardenPlots[startAt]);

            mapOfGardenPlots[startAt] = '.';

            var getAdjGardenPlots = StepCounter.GetAdjacentGardenPlots(mapOfGardenPlots);

            var plots = StepCounter.LocateAccessibleGardenPlotsAtDistances(startAt, getAdjGardenPlots, 64).Take(1).ToArray();

            var actualResult = plots[0];

            Assert.AreEqual(3776, actualResult);
        }
    }
}
