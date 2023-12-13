using System.Text.RegularExpressions;
using static AdventOfCode._2023.Day12.HotSprings;

namespace AdventOfCode._2023.Day12
{
    public partial class HotSprings(SpringConditionRecord[] SpringConditionRecords)
    {
        const char SPRING_DAMAGED = '#';
        const char SPRING_OPERATIONAL = '.';
        const char SPRING_STATE_UNKNOWN = '?';

        [GeneratedRegex("#+",  RegexOptions.Compiled)] private static partial Regex AllPoundsRegex();
        [GeneratedRegex("\\?", RegexOptions.Compiled)] private static partial Regex FirstMatchRegex();

        private static readonly Regex _firstMatch = FirstMatchRegex();
        private static readonly Regex _allPounds = AllPoundsRegex();

        public static implicit operator HotSprings(SpringConditionRecord[] records) => new(records);

        public static HotSprings Parse(IEnumerable<string> rawSpringConditionRecords, int unfoldFactor = 1)
            => unfoldFactor switch {
                1 => rawSpringConditionRecords.Select(s => s.Split(' ')).Select(xs => new SpringConditionRecord(xs[0], xs[1].Split(',').Select(int.Parse).ToArray())).ToArray(),
                _ when unfoldFactor > 1 => rawSpringConditionRecords
                                                  .Select(s => s.Split(' '))
                                                  .Select(xs => new SpringConditionRecord(
                                                       new string(Enumerable.Repeat(string.Concat(xs[0], '?'), unfoldFactor).Aggregate(string.Empty, string.Concat)),
                                                       new string(Enumerable.Repeat(string.Concat(xs[1], ','), unfoldFactor).Aggregate(string.Empty, string.Concat)).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()
                                                   )).ToArray(),
                _ => []
            };

        public long CalculateTotalNumberOfPossibleArrangementsAcrossAllRecords() => 
            SpringConditionRecords.Select(SpringConditionRecord.CalculateNumberOfPossibleArrangements)
                                  .Sum();

        public sealed record SpringConditionRecord(string Pattern, int[] Counts)
        {
            private readonly static Dictionary<string, long>? _numberOfArrangementsCache = [];
            private readonly static Dictionary<(string, long), int[]>? _indexesOfPlacementsCache = [];

            public static SpringConditionRecord Parse(string rawRecord, int unfoldFactor = 1)
            {
                var xs = rawRecord.Split(' ');

                var pattern = xs[0];
                var counts = xs[1].Split(',').Select(int.Parse).ToArray();

                return unfoldFactor switch
                {
                    1 => new SpringConditionRecord(pattern, counts),

                    _ when unfoldFactor > 1 => 
                        new SpringConditionRecord(
                            new string(Enumerable.Repeat(string.Concat(xs[0], '?'), unfoldFactor).Aggregate(string.Empty, string.Concat)),
                            new string(Enumerable.Repeat(string.Concat(xs[1], ','), unfoldFactor).Aggregate(string.Empty, string.Concat)).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()
                            ),

                    _ => throw new ApplicationException($"Unable to parse the raw record [{rawRecord ?? "null"}]")
                };
            }
            
            public long CalculateNumberOfPossibleArrangements() => CalculateNumberOfPossibleArrangements(Pattern, Counts);

            public static long CalculateNumberOfPossibleArrangements(SpringConditionRecord record) => CalculateNumberOfPossibleArrangements(record.Pattern, record.Counts);

            private static long CalculateNumberOfPossibleArrangements(string pattern, int[] counts)
            {
                if (0 >= counts.Length) return pattern.Contains('#') ? 0 : 1;

                var cacheKey = $"{pattern} -> ({string.Join(',', counts)})"; 

                if (_numberOfArrangementsCache?.TryGetValue(cacheKey, out var possibilities) ?? false) return possibilities;

                var c = counts[0];

                possibilities =
                    CalculateOrderedIndexesOfPossiblePlacements(pattern, c)
                        .Sum(idx =>
                        {
                            var n = idx + c + 1;

                            var remainingPattern = n >= pattern.Length ? string.Empty : pattern[n..];

                            return CalculateNumberOfPossibleArrangements(remainingPattern, counts[1..]);
                        });

                _ = _numberOfArrangementsCache?.TryAdd(cacheKey, possibilities);

                return possibilities;
            }

            /// <summary>
            /// Tasked with calculating how many valid arrangements there are for the incoming pattern given the 
            /// number of broken springs
            /// </summary>
            /// <param name="pattern"></param>
            /// <param name="numContiguousDamagedSprings"></param>
            /// <returns>The index in the pattern where the group of damaged springs could be injected</returns>
            public static int[] CalculateOrderedIndexesOfPossiblePlacements(string pattern, int numContiguousDamagedSprings)
            {
                var cacheKey = (pattern, numContiguousDamagedSprings);

                if (_indexesOfPlacementsCache?.TryGetValue(cacheKey, out var indexes) ?? false) return indexes;

                // .            - matches any character other than a line terminator
                // !            - does not match
                // |            - or operator
                // $            - asserts it is the end of the string
                // (?=)         - positive lookahead search - aware of next token
                // (?<)         - negative lookbehind search - aware of previous token

                // !#           - does not match '#'
                // [?#]         - matches any single character from the list ... '#' or '?'
                // {n}          - matches the previous token exactly n-times (the count parameter)
                // [.?]|$       - matches a single character from the list [ '.' or '?' or {asserts end of line} ]

                // with +ve lookahead (?=
                //   (?<!#)     - with -ve lookbehind .. does not match the # character
                //   [?#]{n}    - matches '#' or '?' exactly n-times ... # or ?? or ### etc.
                //   (?=[.?]|$) - with +ve lookahead ... matches a single character from the list [ '.' or '?' or {asserts end of line} ] 
                // )

                // in other words ...
                // find me each index in the pattern where there are n contiguous '#' chars (already matches our pattern)
                // or n '?' and '#' characters grouped together (###, ???, ?##, ##?, #?#, ?#? when n = 3)
                // This gives us a list of potential matches that we then need to filter down to remove anything where there is a preceeding 
                // # character (a ? would become a .) anywhere in the pattern

                var p = string.Concat("(?=(?<!", SPRING_DAMAGED ,")[", SPRING_STATE_UNKNOWN, SPRING_DAMAGED, "]{", numContiguousDamagedSprings, "}(?=[", SPRING_OPERATIONAL, SPRING_STATE_UNKNOWN,"]|$)).");

                var regex = new Regex(p, RegexOptions.Compiled);

                // A ? character can be interpreted as either # or . so long as the pattern aligsn with the damaged spring grouping
                // If the damaged spring grouping (DSG) = (1,1,3) and we have the intial pattern ???.###
                // we would expect the result for #.#.### = (1, 1, 3) to be...
                // f("???.###", 1) => [0,1,2]
                // f("?.###", 1)   => []
                // f("###", 3)     => []

                var ms = regex.Matches(pattern);

                indexes = ms.Select(m => m.Index).Where(i => i == 0 || !pattern[..(i - 1)].Contains('#')).ToArray();

                _ = _indexesOfPlacementsCache?.TryAdd(cacheKey, indexes);

                return indexes;
            }
        }
    }
}
