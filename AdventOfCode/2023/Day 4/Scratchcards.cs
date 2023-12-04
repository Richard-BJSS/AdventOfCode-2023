using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2023.Day_4.Scratchcards
{
    public sealed record Scratchcard(int CardIdx, int[] WinningNumbers, int[] CardNumbers, int Points)
    {
        public static ValueTask<Scratchcard> ParseAsync(string s)
        {
            var idxOfColon = s.IndexOf(':');
            var idxOfPipe = s.IndexOf('|');

            var cardIdx = int.Parse(s[5..idxOfColon++]);

            var wns = s[idxOfColon..idxOfPipe];
            var cns = s[(idxOfPipe + 1)..];

            var winningNumbers = wns.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray();
            var cardNumbers = cns.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray();

            var intersect = cardNumbers.Intersect(winningNumbers).ToArray();

            var points = intersect.Length switch {
                0 => 0,
                1 => 1,
                _ => intersect.Aggregate(1, (a, _) => a + a) / 2
            };

            return ValueTask.FromResult(new Scratchcard(cardIdx, winningNumbers, cardNumbers, points));
        }
    }
}
