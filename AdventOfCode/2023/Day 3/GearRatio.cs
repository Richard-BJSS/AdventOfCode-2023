using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023.Day_3.GearRatio
{
    public sealed record Engine(IEnumerable<EnginePart> Parts)
    {
        private static readonly Regex _symbolsRegex = new("[^.0-9]", RegexOptions.Compiled);

        public static Engine FromSchematic(IEnumerable<string> rawEngineSchematic)
        {
            var matrix = rawEngineSchematic.Select(r => r.ToCharArray()).ToArray();

            var rows = matrix.Length;
            var cols = matrix[0].Length;

            var parts = new List<EnginePart>();
            var digits = new List<char>(cols);

            var discards = new List<string>();

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    var cellValue = matrix[y][x];
                    var isDigit = char.IsDigit(cellValue);

                    if (isDigit) digits.Add(cellValue);

                    if ((!isDigit || x == (cols - 1)) && digits.Count > 0)
                    {
                        var candidatePartNum = int.Parse(new string(digits.ToArray()));

                        var adjacentCellsInBounds = CellsAdjacentTo((x, y), digits.Count, cols).Where(c => IsInBounds(c, matrix));
                        
                        var adjacentCellsContainingSymbol = adjacentCellsInBounds.Where(c => IsValidSymbol(matrix[c.y][c.x]));

                        if (adjacentCellsContainingSymbol.Any()) parts.Add(candidatePartNum);
                        else discards.Add($"Discarding {candidatePartNum} on row {y + 1}");

                        digits.Clear();
                    }
                }
            }

            File.WriteAllLines("Discards.txt", discards.ToArray());

            return new (Parts: parts);
        }

        private static IEnumerable<(int x, int y)> CellsAdjacentTo((int x, int y) c, int lengthOfPartNumber, int numberOfColumns) =>
            Enumerable.Concat(
                AdjacentCellsOnRowAbove(c, lengthOfPartNumber),
                AdjacentCellsOnSameRow(c, lengthOfPartNumber, numberOfColumns)).Concat(
                AdjacentCellsOnRowBelow(c, lengthOfPartNumber));

        private static IEnumerable<(int x, int y)> AdjacentCellsOnRowAbove((int x, int y) c, int lengthOfPartNumber) => Enumerable.Range((c.x - (lengthOfPartNumber + 1)), lengthOfPartNumber + 2).Select(x => (x, y: c.y - 1));
        private static IEnumerable<(int x, int y)> AdjacentCellsOnSameRow((int x, int y) c, int lengthOfPartNumber, int numberOfColumns) => new[] { (x: c.x - (lengthOfPartNumber + (c.x + 1 == numberOfColumns ? 0 : 1)), y: c.y), c };
        private static IEnumerable<(int x, int y)> AdjacentCellsOnRowBelow((int x, int y) c, int lengthOfPartNumber) => Enumerable.Range((c.x - (lengthOfPartNumber + 1)), lengthOfPartNumber + 2).Select(x => (x, y: c.y + 1));

        private static bool IsInBounds((int x, int y) c, char[][] matrix) => c.x >= 0 && c.x < matrix[0].Length && c.y >= 0 && c.y < matrix.Length;
        private static bool IsValidSymbol(char c) => _symbolsRegex.IsMatch(new string(new[] { c }));
    }

    public sealed record EnginePart(int PartNumber)
    {
        public static implicit operator int(EnginePart enginePart) => enginePart.PartNumber;
        public static implicit operator EnginePart(int partNumber) => new(PartNumber: partNumber);
    }

}
