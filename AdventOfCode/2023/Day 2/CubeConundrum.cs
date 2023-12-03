namespace AdventOfCode._2023.Day_2.CubeConundrum
{
    public sealed record Cubes(int Red = 0, int Green = 0, int Blue = 0)
    {
        public int Count => Red + Green + Blue;

        public int Power => Red * Green * Blue;

        public static Cubes Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ApplicationException($"Unable to parse {s}");

            var rawReveals = s.Split(',');

            int r = 0, g = 0, b = 0;

            foreach (var rawReveal in rawReveals)
            {
                if (rawReveal.Contains("red")) r = int.Parse(rawReveal[..rawReveal.IndexOf('r')]);
                if (rawReveal.Contains("green")) g = int.Parse(rawReveal[..rawReveal.IndexOf('g')]);
                if (rawReveal.Contains("blue")) b = int.Parse(rawReveal[..rawReveal.IndexOf('b')]);
            }

            return new(Red: r, Green: g, Blue: b);
        }
    }

    public sealed record Bag(Cubes Cubes)
    {
        public Bag(int red = 0, int green = 0, int blue = 0) : this(new Cubes(red, green, blue)) { }

        public async IAsyncEnumerable<Game> EliminateImpossibleGamesAsync(IAsyncEnumerable<Game> gamesToValidate)
        {
            if (gamesToValidate is null) yield break;

            var numberOfCubesInThisBag = Cubes.Count;

            await foreach (var game in gamesToValidate)
            {
                var validReveals = 0;

                foreach (var reveal in game.CubesRevealed)
                {
                    if (numberOfCubesInThisBag < reveal.Count) break;

                    if (reveal.Red > Cubes.Red) break;
                    if (reveal.Green > Cubes.Green) break;
                    if (reveal.Blue > Cubes.Blue) break;

                    validReveals++;
                }

                if (validReveals == game.CubesRevealed.Length) yield return game;
            }
        }
    }

    public sealed record Game(int Id, params Cubes[] CubesRevealed)
    {
        public static ValueTask<Game> ParseAsync(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ApplicationException($"Unable to parse {s}");

            var idxOfColon = s.IndexOf(':');

            var rawGameId = s["Game ".Length..idxOfColon];

            var rawReveals = s.Substring(idxOfColon + 2).Split(';');

            var reveals = rawReveals.Select(Cubes.Parse).ToArray();

            var gameId = int.Parse(rawGameId);

            return new(new Game(Id: gameId, CubesRevealed: reveals));
        }

        public static ValueTask<Cubes> CalculateMinimumCubesRequiredForGameAsync(Game game)
        {
            var r = game.CubesRevealed.Max(c => c.Red);
            var g = game.CubesRevealed.Max(c => c.Green);
            var b = game.CubesRevealed.Max(c => c.Blue);

            return ValueTask.FromResult(new Cubes(Red: r, Green: g, Blue: b));
        }
    }
}
