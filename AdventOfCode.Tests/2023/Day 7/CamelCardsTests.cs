using AdventOfCode._2023.Day_7;

namespace AdventOfCode.Tests._2023.Day_7
{
    [TestClass]
    public sealed class CamelCardsTests
    {
        [TestMethod]
        public void TotalWinnings_AreCalculatedCorrectly()
        {
            var rawGame = new[] {
                "32T3K 765", 
                "T55J5 684", 
                "KK677 28", 
                "KTJJT 220", 
                "QQQJA 483",
            };

            var hands = rawGame.Select(CamelCards.Parse);

            var handsOrderedByStrength = hands.Order(HandStrengthComparer.StrongestFirst).OrderByDescending(x => x);

            var winnings = handsOrderedByStrength.Select((h, n) => (n + 1) * h.Item2);
            
            var totalWinnings = winnings.Sum();

            Assert.AreEqual(6440, totalWinnings);
        }

        [TestMethod]
        public void Hands_AreOrderedByStrongestFirst()
        {
            var rawGame = new[] {
                "99999  1", // 5x9
                "11111  2", // 5x1
                "99991  3", // 4x9 - 9
                "19999  4", // 4x9 - 1
                "33322  5", // FH - 3332
                "33311  6", // FH - 3331
                "33324  7", // 3x3 - 3332
                "33312  8", // 3x3 - 3331
                "21333  9", // 3x3 - 2
                "12333 10", // 3x3 - 1
                "12644 11", // Pair 4 - 126
                "12544 12", // Pair 4 - 125
                "25Q73 13", // High Card - 25Q
                "2567Q 14", // High Card - 256
                "2468T 15", // High Card - 246
                "23456 16", // High Card - 234
                "12345 17", // High Card - 123
            };

            var hands = rawGame.Select(CamelCards.Parse);

            var handsOrderedByStrength = hands.OrderBy(_ => Guid.NewGuid()).Order(HandStrengthComparer.StrongestFirst).ToArray();

            for (var n = 0; n < handsOrderedByStrength.Length; n++)
            {
                var x = CamelCards.Parse(rawGame[n]);
                var y = handsOrderedByStrength[n];

                Assert.AreEqual(0, x.CompareTo(y));
                Assert.AreEqual(n + 1, (x.Bid + y.Item2) / 2);
            }

            handsOrderedByStrength = hands.OrderBy(_ => Guid.NewGuid()).OrderDescending(HandStrengthComparer.StrongestFirst).ToArray();

            for (var n = 0; n < handsOrderedByStrength.Length; n++)
            {
                var x = CamelCards.Parse(rawGame[n]);
                var y = handsOrderedByStrength[handsOrderedByStrength.Length - n - 1];

                Assert.AreEqual(0, x.CompareTo(y));
                Assert.AreEqual(n + 1, (x.Bid + y.Item2) / 2);
            }
        }

        [TestMethod]
        public void Hands_AreOrderedByWeakestFirst()
        {
            var rawGame = new[] {
                "99999  1", // 5x9
                "11111  2", // 5x1
                "99991  3", // 4x9 - 9
                "19999  4", // 4x9 - 1
                "33322  5", // FH - 3332
                "33311  6", // FH - 3331
                "33324  7", // 3x3 - 3332
                "33312  8", // 3x3 - 3331
                "21333  9", // 3x3 - 2
                "12333 10", // 3x3 - 1
                "12644 11", // Pair 4 - 126
                "12544 12", // Pair 4 - 125
                "25Q73 13", // High Card - 25Q
                "2567Q 14", // High Card - 256
                "2468T 15", // High Card - 246
                "23456 16", // High Card - 234
                "12345 17", // High Card - 123
            };

            var hands = rawGame.Select(CamelCards.Parse);

            var handsOrderedByStrength = hands.OrderBy(_ => Guid.NewGuid()).OrderDescending(HandStrengthComparer.StrongestFirst).ToArray();

            for (var n = 0; n < handsOrderedByStrength.Length; n++)
            {
                var x = CamelCards.Parse(rawGame[n]);
                var y = handsOrderedByStrength[handsOrderedByStrength.Length - n - 1];

                Assert.AreEqual(0, x.CompareTo(y));
                Assert.AreEqual(n + 1, (x.Bid + y.Item2) / 2);
            }
        }

        [TestMethod]
        public async Task TotalWinnings_AreCalculatedCorrectlyFromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawHands = File.ReadLinesAsync("2023/Day 7/CamelCards-File.txt", cts.Token);

            var handsWithBids = rawHands.SelectAwait(CamelCards.ParseAsync);

            var handsOrderedByStrength = handsWithBids.OrderByDescendingAwait(x => ValueTask.FromResult(x), HandStrengthComparer.StrongestFirst);

            var winnings = handsOrderedByStrength.Select((h, n) => (n + 1) * h.Bid);

            var totalWinnings = await winnings.SumAsync();

            Assert.AreEqual(253638586, totalWinnings);
        }

        [DataTestMethod]
        [DataRow("12345", "12345", 0)]
        [DataRow("22145", "12245", -1)]
        [DataRow("12234", "22341", 1)]
        [DataRow("JQQQA", "KQQQA", 1)]
        [DataRow("KQQQA", "JQQQA", -1)]
        [DataRow("33332", "2AAAA", -1)]
        [DataRow("77888", "77788", -1)]
        public void HandStrengths_AreComparedCorrectly(string hand1, string hand2, int expectedResult)
        {
            var h1 = Hand.Parse(hand1);
            var h2 = Hand.Parse(hand2);

            var actualResult = h1.CompareTo(h2);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void WithWildcard_TotalWinnings_AreCalculatedCorrectly()
        {
            var rawGame = new[] {
                "32T3K 765",    // 5. Pair 3
                "T55J5 684",    // 3. Four of a Kind - 5
                "KK677 28",     // 4. Two Pair - K7
                "KTJJT 220",    // 2. Four of a Kind - 10
                "QQQJA 483",    // 1. Four of a Kind - Q
            };

            var hands = rawGame.Select(CamelCards.ParseWithWildcards);

            var handsOrderedByStrength = hands.Order(HandStrengthComparer.StrongestFirst).OrderByDescending(x => x);

            var winnings = handsOrderedByStrength.Select((h, n) => (n + 1) * h.Item2);

            var totalWinnings = winnings.Sum();

            Assert.AreEqual(5905, totalWinnings);
        }

        [TestMethod]
        public async Task WithWildcard_TotalWinnings_AreCalculatedCorrectlyFromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawHands = File.ReadLinesAsync("2023/Day 7/CamelCards-File.txt", cts.Token);

            var handsWithBids = rawHands.SelectAwait(CamelCards.ParseWithWildcardsAsync);

            var handsOrderedByStrength = handsWithBids.OrderByDescendingAwait(x => ValueTask.FromResult(x), HandStrengthComparer.StrongestFirst);

            var winnings = handsOrderedByStrength.Select((h, n) => (n + 1) * h.Bid);

            var totalWinnings = await winnings.SumAsync();

            Assert.AreEqual(253253225, totalWinnings); 
        }
    }
}
