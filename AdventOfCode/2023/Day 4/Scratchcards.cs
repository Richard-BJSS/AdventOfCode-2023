namespace AdventOfCode._2023.Day_4.Scratchcards
{
    public sealed record Scratchcard(int CardIdx, int[] WinningNumbers, int[] CardNumbers)
    {
        public int[] MatchingNumbers = WinningNumbers.Intersect(CardNumbers).ToArray();

        public int Points => 
            MatchingNumbers.Length switch
            {
                0 => 0,
                1 => 1,
                _ => MatchingNumbers.Aggregate(1, (a, _) => a + a) / 2
            };

        public static ValueTask<Scratchcard> ParseAsync(string s)
        {
            var idxOfColon = s.IndexOf(':');
            var idxOfPipe = s.IndexOf('|');

            var cardIdx = int.Parse(s[5..idxOfColon++]);

            var wns = s[idxOfColon..idxOfPipe];
            var cns = s[(idxOfPipe + 1)..];

            var winningNumbers = wns.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray();
            var cardNumbers = cns.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray();

            return ValueTask.FromResult(new Scratchcard(cardIdx, winningNumbers, cardNumbers));
        }

        public IEnumerable<Scratchcard> ExpandToIncludeExtraCardsWon(Dictionary<int, Scratchcard> scratchcards)
        {
            yield return this;

            var mn = MatchingNumbers;

            if (0 < mn.Length)
            {
                var cardsWon = from cardIdx in Enumerable.Range(CardIdx + 1, mn.Length)
                               where scratchcards.ContainsKey(cardIdx)
                               select scratchcards[cardIdx];

                foreach (var card in cardsWon)
                {
                    var extraCardsWon = card.ExpandToIncludeExtraCardsWon(scratchcards);

                    foreach (var extraCard in extraCardsWon) yield return extraCard;
                }
            }
        }

    }
}
