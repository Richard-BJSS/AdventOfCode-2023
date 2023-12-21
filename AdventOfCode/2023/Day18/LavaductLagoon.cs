using AdventOfCode.Maths.Geometry.Euclidean;
using System.Drawing;

namespace AdventOfCode._2023.Day18
{
    public sealed class LavaductLagoon((char Command, int Length)[] instructions)
    {

        public static LavaductLagoon Parse(string rawDigPlan)
        {
            var instructions = rawDigPlan.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(i => i.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                         .Select(a => (Command: char.Parse(a[0]), Length: int.Parse(a[1]))).ToArray();

            return new(instructions);
        }

        public static LavaductLagoon ParseUsingHex(string rawDigPlan)
        {
            var instructions = rawDigPlan.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(i => i.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                         .Select(a => (
                                            Command: a[2][^2] switch { 
                                                '0' => 'R',
                                                '1' => 'D',
                                                '2' => 'L',
                                                '3' => 'U',
                                                _ => throw new ApplicationException($"Unexpected code {a[2][..^1]}")
                                            }, 
                                            Length: Convert.ToInt32(a[2][2..^2], fromBase: 16)))
                                         .ToArray();

            return new(instructions);
        }

        public long CalculateVolumeOfHole()
        {
            var points = new List<Point>([Point.Empty]);
            var pt = points[0];
            var perimeter = 0L;

            for (var n = 0; n < instructions.Length; n++)
            {
                var (command, length) = instructions[n];

                pt = command switch
                {
                    'R' => new Point(pt.X + length, pt.Y),
                    'L' => new Point(pt.X - length, pt.Y),
                    'U' => new Point(pt.X, pt.Y - length),
                    'D' => new Point(pt.X, pt.Y + length),

                    _ => throw new ApplicationException($"Unknown command : {command}")
                };

                points.Add(pt);

                perimeter += length;
            }

            var trench = new Polygon([.. points]);

            var internalArea = Polygon.PicksTheorum.InternalAreaOfSimplePolygon(trench, perimeter);

            return internalArea + perimeter;
        }
    }
}
