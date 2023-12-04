using AdventOfCode._2023.Day_4.Scratchcards;

namespace AdventOfCode.Tests._2023.Day_4
{
    [TestClass]
    public sealed class ScratchcardsTests
    {
        private static readonly string[] _rawScratchcards =
            [
                "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
                "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
                "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
                "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
                "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
                "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11",
            ];

        [TestMethod]
        public async Task Scratchcard_CorrectlyCalculatesThePointsEarnedByEachCard()
        {
            var rawScratchcards = _rawScratchcards.ToAsyncEnumerable();

            var scratchcards = await rawScratchcards.SelectAwait(Scratchcard.ParseAsync).ToArrayAsync();

            Assert.AreEqual(8, scratchcards[0].Points);
            Assert.AreEqual(2, scratchcards[1].Points);
            Assert.AreEqual(2, scratchcards[2].Points);
            Assert.AreEqual(1, scratchcards[3].Points);
            Assert.AreEqual(0, scratchcards[4].Points);
            Assert.AreEqual(0, scratchcards[5].Points);
        }

        [TestMethod]
        public async Task Scratchcard_CorrectlyCalculatesThePointsEarnedByEachCardInFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawScratchcards = File.ReadLinesAsync("2023/Day 4/Scratchcard-File.txt", cts.Token);

            var scratchcards = rawScratchcards.SelectAwait(Scratchcard.ParseAsync);

            var sumOfPoints = await scratchcards.SumAsync(c => c.Points);

            Assert.AreEqual(21558, sumOfPoints);
        }

        [TestMethod]
        public async Task ScratchCard_RollsUpWinningCards()
        {
            var rawScratchcards = _rawScratchcards.ToAsyncEnumerable();

            var scratchcards = await rawScratchcards.SelectAwait(Scratchcard.ParseAsync).ToDictionaryAsync(s => s.CardIdx, s => s);

            var expandedCards = scratchcards.Values.SelectMany(sc => sc.ExpandToIncludeExtraCardsWon(scratchcards));

            var expandedCardCount = expandedCards.Count();

            Assert.AreEqual(30, expandedCardCount);
        }

        [TestMethod]
        public async Task ScratchCard_RollsUpWinningCardsInFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawScratchcards = File.ReadLinesAsync("2023/Day 4/Scratchcard-File.txt", cts.Token);

            var scratchcards = await rawScratchcards.SelectAwait(Scratchcard.ParseAsync).ToDictionaryAsync(s => s.CardIdx, s => s);

            var expandedCards = scratchcards.Values.SelectMany(sc => sc.ExpandToIncludeExtraCardsWon(scratchcards));

            var expandedCardCount = expandedCards.Count();

            Assert.AreEqual(10425665, expandedCardCount);
        }
    }
}
