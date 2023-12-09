namespace AdventOfCode._2023.Day_7
{
    public sealed class CamelCards
    {
        public static ValueTask<(Hand Hand, int Bid)> ParseAsync(string rawExtendedHand) => ValueTask.FromResult(Parse(rawExtendedHand));

        public static ValueTask<(Hand Hand, int Bid)> ParseWithWildcardsAsync(string rawExtendedHand) => ValueTask.FromResult(ParseWithWildcards(rawExtendedHand)); 

        public static (Hand Hand, int Bid) Parse(string rawExtendedHand)
        {
            var extHand = rawExtendedHand.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return (Hand.Parse(extHand[0]), int.Parse(extHand[1]));
        }

        public static (Hand Hand, int Bid) ParseWithWildcards(string rawExtendedHand)
        {
            var extHand = rawExtendedHand.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return (Hand.Parse(extHand[0], playingWildcards: true), int.Parse(extHand[1]));
        }

    }

    public sealed record Hand
        : IComparable<Hand>
    {
        private readonly Card[] _cards;
        private readonly bool _playingWildcards;

        public Hand(Card[] cards, bool playingWildcards) : base()
        {
            _playingWildcards = playingWildcards;

            _cards = _playingWildcards ? cards.Select(c => c == Card.Joker ? Card.Wildcard : c).ToArray() : cards;

            IEnumerable<Card> cardsOrderedByValueAsc = !_playingWildcards ? cards.Order() : ReplaceWildcards(_cards).Order();

            var typePattern = 
                from c in cardsOrderedByValueAsc
                group c by c into g
                let gc = g.Count()
                orderby gc descending
                select gc;

            Type = typePattern.ToList() switch
            {
                [5] => 6,               // Five Of A Kind
                [4, 1] => 5,            // Four Of A Kind
                [3, 2] => 4,            // Full House
                [3, 1, 1] => 3,         // Three Of A Kind
                [2, 2, 1] => 2,         // Two Pair
                [2, 1, 1, 1] => 1,      // Pair
                [1, 1, 1, 1, 1] => 0,   // High Card

                _ => throw new ApplicationException("Unable to determine hand type from cards")
            };
        }

        public static Hand Parse(string s, bool playingWildcards = false) => new([.. s.ToCharArray().Select(c => Card.Parse(c))], playingWildcards);
 
        public byte Type { get; init; }

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

                    var comparison = c1.CompareTo(c2);

                    if (0 == comparison) continue;

                    return comparison * -1;
                }

                return 0;
            }

            return 1;
        }

        private static Card[] ReplaceWildcards(Card[] cards)
        {
            if (!cards.Any(c => c == Card.Wildcard)) return cards;

            var noWildcards = cards.Where(c => c != Card.Wildcard);

            var q = from c in noWildcards
                    group c by c into g
                    orderby g.Count() descending
                    select g.Key;

            var mostFrequentCard = q.FirstOrDefault();

            if (mostFrequentCard is null) return Parse("AAAAA")._cards;

            var xs = new Card[cards.Length];

            for (var n = 0; n < cards.Length; n++)
            {
                xs[n] = Card.Wildcard == cards[n]
                    ? mostFrequentCard
                    : cards[n];
            }

            return [.. xs];
        }
    }

    public sealed record Card(char Face)
        : IComparable<Card>
    {
        public readonly static Card Wildcard = new('*');
        public readonly static Card Joker = new('J');

        private static readonly Dictionary<char, int> relativeValuesMap = new() {
            { '*', 0 },
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

        public int Value => relativeValuesMap[Face];

        public int CompareTo(Card? other) => Value.CompareTo(other?.Value ?? 0);
    };

    public sealed class HandStrengthComparer
        : IComparer<(Hand hand, int bid)>
    {
        public readonly static HandStrengthComparer StrongestFirst = new();

        public int Compare((Hand hand, int bid) x, (Hand hand, int bid) y) => x.hand.CompareTo(y.hand);
    }

}
