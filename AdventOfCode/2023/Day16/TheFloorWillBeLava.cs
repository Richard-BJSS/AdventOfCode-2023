using System.Drawing;

namespace AdventOfCode._2023.Day16
{
    public static class TheFloorWillBeLava
    {
        public static IEnumerable<Point> FindEnergisedTilesFromPointOfEntry(string[] contraptionLayout, (int x, int y, int dx, int dy) entryPoint)
        {
            var q = new Queue<(int x, int y, int dx, int dy)>();

            var v = new List<(int x, int y, int dx, int dy)>();

            var sz = new Size(contraptionLayout[0].Length, contraptionLayout.Length);

            q.Enqueue(entryPoint);

            while (0 < q.Count)
            {
                var l = q.Dequeue();

                if (l.x < 0 || l.y < 0 || l.x >= sz.Width || l.y >= sz.Height || v.Contains(l)) continue;

                v.Add(l);

                (int x, int y, int dx, int dy)[] nls = contraptionLayout[l.y][l.x] switch
                {
                    '-' when l.dx == 0                => [(l.x - 1, l.y, -1, 0), (l.x + 1, l.y, 1, 0)],
                    '-'                               => [(l.x + l.dx, l.y, l.dx, 0)                      ],

                    '|' when l.dy == 0                => [(l.x, l.y - 1, 0, -1), (l.x, l.y + 1, 0, 1)],
                    '|'                               => [(l.x, l.y + l.dy, 0, l.dy)],

                    '/' when l.dx == 0 && l.dy == 1   => [(l.x - 1, l.y, -1, 0)],
                    '/' when l.dx == 0 && l.dy == -1  => [(l.x + 1, l.y, 1, 0)],
                    '/' when l.dx == 1                => [(l.x, l.y - 1, 0, -1)],
                    '/'                               => [(l.x, l.y + 1, 0, 1)],

                    '\\' when l.dx == 0 && l.dy == 1  => [(l.x + 1, l.y, 1, 0)],
                    '\\' when l.dx == 0 && l.dy == -1 => [(l.x - 1, l.y, -1, 0)],
                    '\\' when l.dx == 1               => [(l.x, l.y + 1, 0, 1)],
                    '\\'                              => [(l.x, l.y - 1, 0, -1)],

                    _ => [(l.x + l.dx, l.y + l.dy, l.dx, l.dy)]
                };
                 
                foreach (var nl in nls) q.Enqueue(nl);
            }

            return v.Select(t => new Point(t.x, t.y)).Distinct();
        }

        public static int CalculateMaximumPossibleNumberOfEnergisedTiles(string[] contraptionLayout)
        {
            var w = contraptionLayout[0].Length;
            var h = contraptionLayout.Length;

            var entryPoints = new List<(int, int, int, int)>();

            entryPoints.AddRange(Enumerable.Range(0, w - 1).Select(n => (n, 0, 0, -1)));
            entryPoints.AddRange(Enumerable.Range(0, w - 1).Select(n => (n, 0, 0, +1)));

            entryPoints.AddRange(Enumerable.Range(0, h - 1).Select(n => (0, n, -1, 0)));
            entryPoints.AddRange(Enumerable.Range(0, h - 1).Select(n => (0, n, +1, 0)));

            return entryPoints.Max(ep => FindEnergisedTilesFromPointOfEntry(contraptionLayout, ep).Count());
        }
    }
}
