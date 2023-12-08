namespace AdventOfCode._2023.Day_7
{
    public sealed class CamelCards
    {
        public static ValueTask<(Hand Hand, int Bid)> ParseAsync(string rawExtendedHand) => ValueTask.FromResult(Parse(rawExtendedHand));

        public static (Hand Hand, int Bid) Parse(string rawExtendedHand)
        {
            var extHand = rawExtendedHand.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return (Hand.Parse(extHand[0]), int.Parse(extHand[1]));
        }
    }

    public sealed record Hand
        : IComparable<Hand>
    {
        private readonly Card[] _cards;
        private readonly Card[] _cardsOrderedByStrength;

        public Hand(Card[] cards) : base()
        {
            _cards = cards;
            _cardsOrderedByStrength = [.. _cards.Order()];

            var q = from c in _cardsOrderedByStrength
                    group c by c.face into g
                    let gc = g.Count()
                    orderby gc descending
                    select gc;

            Type = q.ToList() switch
            {
                [5] => HandType.FiveOfAKind,
                [4, 1] => HandType.FourOfAKind,
                [3, 2] => HandType.FullHouse,
                [3, 1, 1] => HandType.ThreeOfAKind,
                [2, 2, 1] => HandType.TwoPair,
                [2, 1, 1, 1] => HandType.Pair,
                [1, 1, 1, 1, 1] => HandType.HighCard,
                _ => throw new ApplicationException("Unable to determine hand type from cards")
            };
        }

        public static Hand Parse(string s) => new([.. s.ToCharArray().Select(c => Card.Parse(c))]);

        public HandType Type { get; init; }

        public int CompareTo(Hand? other)
        {
            if (other is null) return -1;

            if (Type > other.Type) return -1;

            if (Type == other.Type)
            {
                for (var n = 0; n < _cards.Length; n++)
                {
                    var c1 = _cards[n];
                    var c2 = other._cards[n];

                    if (c1.Value == c2.Value) continue;

                    return Math.Sign(c2.Value - c1.Value);
                }

                return 0;
            }

            return 1;
        }

        public override string ToString() =>
            _cards.Aggregate(string.Empty, (a, c) => a + c.ToString()) + "=>" +
            _cardsOrderedByStrength.Aggregate(string.Empty, (a, c) => a + c.ToString()) + "=>" +
            Type.ToString();
    }

    public enum HandType
    {
        HighCard = 0,
        Pair = 1,
        TwoPair = 2,
        ThreeOfAKind = 3,
        FullHouse = 4,
        FourOfAKind = 5,
        FiveOfAKind = 6
    }

    public sealed record Card(char face)
        : IComparable<Card>
    {
        private static Dictionary<char, int> relativeValuesMap = new() {
                { '_', 0 },
                { '1', 1 },
                { '2', 2 },
                { '3', 3 },
                { '4', 4 },
                { '5', 5 },
                { '6', 6 },
                { '7', 7 },
                { '8', 8 },
                { '9', 9 },
                { 'T', 10 },
                { 'J', 11 },
                { 'Q', 12 },
                { 'K', 13 },
                { 'A', 14 },
            };

        public static Card Parse(char face) => new(face);

        public int Value => relativeValuesMap[face];

        public int CompareTo(Card? other) => Value.CompareTo(other?.Value ?? 0);

        public override string ToString() => new(new[] { face });
    };

    public sealed class HandStrengthComparer
        : IComparer<(Hand hand, int bid)>
    {
        public readonly static HandStrengthComparer StrongestFirst = new();

        public int Compare((Hand hand, int bid) x, (Hand hand, int bid) y) => x.hand.CompareTo(y.hand);
    }

}
