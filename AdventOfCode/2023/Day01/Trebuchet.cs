using System.Text.RegularExpressions;

namespace AdventOfCode._2023.Day01
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

                if (2 == tokens.Length)
                {
                    var fst = tokens[0];
                    var lst = tokens[^1];

                    yield return (fst * 10) + lst;
                }
                else yield return default;
            }
        }

        private sealed class Tokenizer(IEnumerable<TokenDefinition> definitions)
        {
            public IEnumerable<Token> Tokenize(string toBeTokenized) =>
                from definition in definitions ?? Enumerable.Empty<TokenDefinition>()
                from token in definition.FindTokens(toBeTokenized)
                orderby token.Index ascending
                select token;
        }   

        private sealed record Token(string Value, string Name, int Index)
        {
            public readonly static IEnumerable<TokenDefinition> DIGITS_DEFINITIONS = new TokenDefinition[] { "[0-9]" };
            public readonly static IEnumerable<TokenDefinition> DIGITS_AND_WORDS_DEFINITIONS = new TokenDefinition[] { "[0-9]|zero|one|two|three|four|five|six|seven|eight|nine" };

            public static implicit operator int?(Token token) 
            {
                var v = token.Value;

                if (v.Length == 1)
                {
                    if (char.IsNumber(token.Value, 0)) return (int)(char.GetNumericValue(v, 0));

                    throw new ApplicationException($"Not supported token value : {v}");
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
                    _ => throw new ApplicationException($"Not supported token value : {v}"),
                };
            }
        }

        private sealed class TokenDefinition(string pattern)
        {
            private readonly Regex _positiveLookaheadRegex = new ($"(?=({pattern}))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            public IEnumerable<Token> FindTokens(string input)
            {
                var matches = _positiveLookaheadRegex.Matches(input);

                var fst = matches.FirstOrDefault()?.Groups[1];
                var lst = matches.LastOrDefault()?.Groups[1];

                if (fst is not null) yield return new(fst.Value, fst.Name, fst.Index);
                if (lst is not null) yield return new(lst.Value, lst.Name, lst.Index);
            }

            public static implicit operator TokenDefinition (string pattern) => new(pattern);            
        }
    }
}
