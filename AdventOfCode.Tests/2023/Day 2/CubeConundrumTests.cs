using AdventOfCode._2023.Day_2;

namespace AdventOfCode.Tests._2023.Day_2
{
    /// <summary>
    /// https://adventofcode.com/2023/day/2
    /// </summary>
    [TestClass]
    public sealed class CubeConundrumTests
    {
        [TestMethod]
        public async Task EliminateImpossibleGames_CorrectlyEliminatesGamesThatRevealTooManyCubes()
        {
            const int red = 12, green = 13, blue = 14;

            var bag = new CubeConundrum.Bag(red, green, blue);

            var games = new CubeConundrum.Game[] {
                new(1, new(Blue:  3, Red:   4),            new(Red:   1, Green: 2, Blue:  6),  new(Green: 2)),
                new(2, new(Blue:  1, Green: 2),            new(Green: 3, Blue:  4, Red:   1),  new(Green: 1, Blue: 1)),
                new(3, new(Green: 8, Blue:  6, Red:   20), new(Blue:  5, Red:   4, Green: 13), new(Green: 5, Red:  1)),
                new(4, new(Green: 1, Red:   6, Blue:  6),  new(Green: 3, Red:   6),            new(Green: 3, Blue: 15, Red: 14)),
                new(5, new(Red:   6, Blue:  1, Green: 3),  new(Blue:  2, Red:   1),            new(Green: 2))
            };

            var validGames = await bag.EliminateImpossibleGamesAsync(games.ToAsyncEnumerable()).ToArrayAsync();

            Assert.IsNotNull(validGames);

            Assert.AreEqual(3, validGames.Length);
            Assert.AreEqual(1, validGames[0].Id);
            Assert.AreEqual(2, validGames[1].Id);
            Assert.AreEqual(5, validGames[2].Id);

            var sumOfValidGamesIds = validGames.Sum(g => g.Id);

            Assert.AreEqual(8, sumOfValidGamesIds);
        }

        [TestMethod]
        public void Parse_CorrectlyParsesCubesRecord()
        {
            var cubes = new Dictionary<string, CubeConundrum.Cubes>
            {
                { "3 blue, 4 red", new(Red: 4, Green: 0, Blue: 3)},
                { "1 red, 2 green, 6 blue", new(Red: 1, Green: 2, Blue: 6)},
                { "2 green", new(Red: 0, Green: 2, Blue: 0)},
                { "1 blue, 2 green", new(Red: 0, Green: 2, Blue: 1)},
                { "3 green, 4 blue, 1 red", new(Red: 1, Green: 3, Blue: 4)},
                { "1 green", new(Red: 0, Green: 1, Blue: 0)},
                { "1 blue", new(Red: 0, Green: 0, Blue: 1)},
                { "8 green, 6 blue, 20 red", new(Red: 20, Green: 8, Blue: 6)},
                { "5 blue, 4 red, 13 green", new(Red: 4, Green: 13, Blue: 5)},
                { "5 green, 1 red", new(Red: 1, Green: 5, Blue: 0)},
                { "1 green, 3 red, 6 blue", new(Red: 3, Green: 1, Blue: 6)},
                { "3 green, 6 red", new(Red: 6, Green: 3, Blue: 0)},
                { "3 green, 15 blue, 14 red", new(Red: 14, Green: 3, Blue: 15)},
                { "6 red, 1 blue, 3 green", new(Red: 6, Green: 3, Blue: 1)},
                { "2 blue, 1 red, 2 green", new(Red: 1, Green: 2, Blue: 2)},
            };

            foreach (var kvp in cubes)
            {
                var actualResult = CubeConundrum.Cubes.Parse(kvp.Key);

                Assert.AreEqual(kvp.Value, actualResult);
            }
        }

        [TestMethod]
        public async Task ParseAsync_CorrectlyParsesGameRecord()
        {
            var gamesRecord = new Dictionary<string, CubeConundrum.Game>
            {
                { "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green", new(1, [new(Blue: 3, Red: 4), new(Red: 1, Green: 2, Blue: 6), new(Green: 2)]) },
                { "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue", new(2, [new(Blue: 1, Green: 2), new(Green: 3, Blue: 4, Red: 1), new(Green: 1, Blue: 1)]) },
                { "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red", new(3, [new(Green: 8, Blue: 6, Red: 20), new(Blue: 5, Red: 4, Green: 13), new(Green: 5, Red: 1)]) },
                { "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red", new(4, [new(Green: 1, Red: 3, Blue: 6), new(Green: 3, Red: 6),new(Green: 3, Blue: 15, Red: 14)]) },
                { "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green", new(5, [new(Red: 6, Blue: 1, Green: 3), new(Blue: 2, Red: 1, Green: 2)]) }
            };

            for (var n = 0; n < gamesRecord.Count; n++)
            {
                var kvp = gamesRecord.ElementAt(n);

                var actualResult = await CubeConundrum.Game.ParseAsync(kvp.Key);

                Assert.AreEqual(n + 1, actualResult.Id);
                Assert.IsTrue(Enumerable.SequenceEqual(kvp.Value.CubesRevealed, actualResult.CubesRevealed));
            }
        }

        [TestMethod]
        public async Task EliminateImpossibleGames_CanBeUsedToTotaliseDocument()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawGameHistory = File.ReadLinesAsync("2023/Day 2/Game-History-File.txt", cts.Token);

            const int red = 12, green = 13, blue = 14;

            var bag = new CubeConundrum.Bag(red, green, blue);

            var games = rawGameHistory.SelectAwait(CubeConundrum.Game.ParseAsync);

            var validGames = bag.EliminateImpossibleGamesAsync(games);

            var sumOfValidGamesIds = await validGames.SumAsync(g => g.Id);

            Assert.AreEqual(2348, sumOfValidGamesIds);
        }

        [TestMethod]
        public async Task CalculateMinimumCubesRequiredForGame_CorrectlyDeterminesMinimumBagContent()
        {
            var gameRecords = new Dictionary<string, CubeConundrum.Cubes>
            {
                { "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green", new(Red: 4, Green: 2, Blue: 6) },
                { "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue", new(Red: 1, Green: 3, Blue: 4) },
                { "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red", new(Red: 20, Green: 13, Blue: 6) },
                { "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red", new(Red: 14, Green: 3, Blue: 15) },
                { "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green", new(Red: 6, Green: 3, Blue: 2) },
            };

            foreach (var kvp in gameRecords) 
            {
                var game = await CubeConundrum.Game.ParseAsync(kvp.Key);

                var minCubesRequiredForGame = await CubeConundrum.Game.CalculateMinimumCubesRequiredForGameAsync(game);

                Assert.AreEqual(kvp.Value, minCubesRequiredForGame);
            }
        }

        [TestMethod]
        public async Task Power_CalculatesCorrectValue()
        {
            var gameRecords = new Dictionary<string, int>
            {
                { "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green", 48 },
                { "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue", 12 },
                { "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red", 1560 },
                { "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red", 630 },
                { "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green", 36 },
            };

            foreach (var kvp in gameRecords)
            {
                var game = await CubeConundrum.Game.ParseAsync(kvp.Key);

                var power = (await CubeConundrum.Game.CalculateMinimumCubesRequiredForGameAsync(game)).Power;

                Assert.AreEqual(kvp.Value, power);
            }
        }

        [TestMethod]
        public async Task CalculateThePowerOfTheMinimumNecessaryCubesInABagAndTotaliseAllGames()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawGameHistory = File.ReadLinesAsync("2023/Day 2/Game-History-File.txt", cts.Token);

            var games = rawGameHistory.SelectAwait(CubeConundrum.Game.ParseAsync);

            var minCubeSetPerGame = games.SelectAwait(CubeConundrum.Game.CalculateMinimumCubesRequiredForGameAsync);

            var sumOfPower = await minCubeSetPerGame.SumAsync(c => c.Power);

            Assert.AreEqual(76008, sumOfPower);
        }
    }
}
