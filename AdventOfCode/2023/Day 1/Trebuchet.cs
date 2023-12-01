using System.Text.RegularExpressions;

namespace AdventOfCode._2023.Day_1
{
    public sealed class Trebuchet
    {
        public async Task<int?> CalibrateAndSumAllAsync(
            IAsyncEnumerable<string> valuesToBeCalibratedAndTotalised,
            IEnumerable<TokenDefinition> tokenDefinitions
            )
        {
            var t = default(int?);

            var parser = new Parser(tokenDefinitions);

            await foreach (var valueToBeCalibrated in valuesToBeCalibratedAndTotalised)
            {
                var calibratedValue = parser.Parse(valueToBeCalibrated);

                if (calibratedValue.HasValue)
                {
                    if (t.HasValue) t += calibratedValue.Value;
                    else t = calibratedValue;
                }
            }

            return t;
        }

        private sealed class Parser(IEnumerable<TokenDefinition> definitions)
        {
            public int? Parse(string toBeParsed)
            {
                if (string.IsNullOrWhiteSpace(toBeParsed)) return default;

                var q = from definition in definitions ?? Enumerable.Empty<TokenDefinition>()
                        from token in definition.FindTokens(toBeParsed)
                        orderby token.Index ascending
                        select token;

                var xs = q.ToArray();

                var fst = xs.Take(1).SingleOrDefault()?.AsNumber();
                var lst = xs.TakeLast(1).SingleOrDefault()?.AsNumber();

                if (fst.HasValue && lst.HasValue) return (fst.Value * 10) + lst.Value;

                return default;
            }
        }

        public sealed record Token(string Value, string Name, int Index)
        {
            public readonly static IEnumerable<TokenDefinition> JUST_SINGLE_DIGITS = new TokenDefinition[] { "[0-9]" };
            public readonly static IEnumerable<TokenDefinition> DIGITS_AND_WORDS = new TokenDefinition[] { "[0-9]|zero|one|two|three|four|five|six|seven|eight|nine" };

            public int AsNumber()
            {
                if (Value.Length == 1)
                {
                    if (char.IsNumber(Value, 0)) return (int)Math.Round(char.GetNumericValue(Value, 0), 0);

                    throw new ApplicationException($"None supported token value : {Value}");
                }
                else return Value switch {
                    "zero" => 0,
                    "one" => 1,
                    "two" => 2,
                    "three" => 3,
                    "four" => 4,
                    "five" => 5,
                    "six" => 6,
                    "seven" => 7,
                    "eight" => 8,
                    "nine" => 9,
                    _ => throw new ApplicationException($"None supported token value : {Value}"),
                };
            }
        }

        public sealed class TokenDefinition(string pattern)
        {
            private readonly Regex _L2Rregex = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            private readonly Regex _R2Lregex = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.RightToLeft);

            public IEnumerable<Token> FindTokens(string input)
            {
                var l2rMatch = _L2Rregex.Matches(input).FirstOrDefault();
                var r2lMatch = _R2Lregex.Matches(input).FirstOrDefault();

                if (l2rMatch is null || r2lMatch is null) yield break;

                yield return new Token(l2rMatch.Value, l2rMatch.Name, l2rMatch.Index);
                yield return new Token(r2lMatch.Value, r2lMatch.Name, r2lMatch.Index);
            }

            public static implicit operator TokenDefinition (string pattern) => new(pattern);            
        }
    }
}
