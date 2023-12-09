using AdventOfCode._2023.Day_8.HauntedWasteland;

namespace AdventOfCode._2023.Day_8.HauntedWasteland
{
    public sealed class Network(Dictionary<Location, (Location Left, Location Right)> journeyMap)
    {
        public static Network Parse(IEnumerable<string> raw)
        {
            var q = from r in raw
                    let key = new Location(r[..3])
                    let val = (new Location(r.Substring(7, 3)), new Location(r.Substring(12, 3)))
                    select KeyValuePair.Create(key, val);

            var network = new Dictionary<Location, (Location, Location)>(q);

            return new(network);
        }

        public (Location Left, Location Right) this[string identifier] => journeyMap[identifier];

        public IEnumerable<Location> Locations => journeyMap.Keys;

        public Journey StartJourney(StepPattern stepPattern, Location startingLocation) => new(this) { StepPattern = stepPattern, StartingLocation = startingLocation };
    }

    public sealed class Journey(Network Network)
    {
        private Location? _current;

        public StepPattern StepPattern = StepPattern.Default;
        public Location StartingLocation = Location.Default;
        public long Tick = 0;

        public Task<Journey> EndWhenAsync(Predicate<Location> predicate)
        {
            _current ??= StartingLocation;

            var stepPattern = StepPattern.Pattern;

            do
            {
                for (var n = 0; n < stepPattern.Length; n++)
                {
                    Tick++;

                    var (left, right) = Network[_current];

                    _current = stepPattern[n] == 'L' ? left : right;

                    if (predicate(_current)) return Task.FromResult(this);
                }
            }
            while (true);
        }
    }

    public sealed record Location
    {
        public readonly static Location Default = new("AAA");

        public Location(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; init; }

        public static implicit operator Location(string identifier) => new(identifier);
        public static implicit operator string(Location location) => location.Identifier;
    }

    public sealed record StepPattern
    {
        public static readonly StepPattern Default = new("RL");

        public StepPattern(string pattern)
        {
            Pattern = pattern;
        }

        public string Pattern { get; init; }

        public static implicit operator StepPattern(string pattern) => new(pattern);
        public static implicit operator string(StepPattern stepPattern) => stepPattern.Pattern;
    }
}
