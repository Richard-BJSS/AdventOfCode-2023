using System.Text.RegularExpressions;

namespace AdventOfCode._2023.Day_1
{
    public static class Trebuchet
    {
        public static IAsyncEnumerable<int?> CalibrateUsingDigitsAsync(IAsyncEnumerable<string> toBeCalibrated) => CalibrateAsync(toBeCalibrated, Token.DIGITS_DEFINITIONS);

        public static IAsyncEnumerable<int?> CalibrateUsingDigitsAndWordsAsync(IAsyncEnumerable<string> toBeCalibrated) => CalibrateAsync(toBeCalibrated, Token.DIGITS_AND_WORDS_DEFINITIONS);



        private static async IAsyncEnumerable<int?> CalibrateAsync(
            IAsyncEnumerable<string> toBeCalibrated,
            IEnumerable<TokenDefinition> tokenDefinitions
            )
        {
            var tokenizer = new Tokenizer(tokenDefinitions);

            await foreach (var valueToBeCalibrated in toBeCalibrated)
            {
                var tokens = tokenizer.Tokenize(valueToBeCalibrated).ToArray();

                if (2 > tokens.Length)
                {
                    yield return default;
                    continue;
                };

                var fst = tokens[0];
                var lst = tokens[^1];

                yield return (fst * 10) + lst;
            }
        }

        private sealed class Tokenizer(IEnumerable<TokenDefinition> definitions)
        {
            public IEnumerable<Token> Tokenize(string toBeParsed)
            {
                if (string.IsNullOrWhiteSpace(toBeParsed)) return Enumerable.Empty<Token>();

                return from definition in definitions ?? Enumerable.Empty<TokenDefinition>()
                       from token in definition.FindTokens(toBeParsed)
                       orderby token.Index ascending
                       select token;
            }
        }

        public sealed record Token(string Value, string Name, int Index)
        {
            public readonly static IEnumerable<TokenDefinition> DIGITS_DEFINITIONS = new TokenDefinition[] { "[0-9]" };
            public readonly static IEnumerable<TokenDefinition> DIGITS_AND_WORDS_DEFINITIONS = new TokenDefinition[] { "[0-9]|zero|one|two|three|four|five|six|seven|eight|nine" };

            public static implicit operator int?(Token token) 
            {
                var v = token.Value;

                if (v.Length == 1)
                {
                    if (char.IsNumber(token.Value, 0)) return (int)Math.Round(char.GetNumericValue(v, 0), 0);

                    throw new ApplicationException($"None supported token value : {v}");
                }
                else return v switch {
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
                    _ => throw new ApplicationException($"None supported token value : {v}"),
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
