using System.Drawing;

namespace AdventOfCode._2023.Day10
{
    public sealed class PipeMap(char[,] ParsedMap)
    {
        private static class Compass
        {
            public const int North = 1;
            public const int NorthEast = 2;
            public const int NorthWest = 4;
            public const int South = 8;
            public const int SouthEast = 16;
            public const int SouthWest = 32;
            public const int East = 64;
            public const int West = 128;
        }

        public static (PipeMap, Point? Start) Parse(string[] rawMap)
        {
            var w = rawMap[0].Length;
            var h = rawMap.Length;

            var map = new char[w, h];

            Point? startLocation = default;

            for (var x = 0; x < w; x++)
            {
                for (var y = 0; y < h; y++)
                {
                    var c = rawMap[y][x];

                    if (c == 'S') startLocation = new(x, y);

                    map[x, y] = c;
                }
            }

            if (startLocation is null) return (new PipeMap(map), default);

            var sl = startLocation.Value;

            map[sl.X, sl.Y] = ComponentAt(map, sl);

            return (new PipeMap(map), startLocation);
        }

        public char? ValueAt(Point pt) => ParsedMap[pt.X, pt.Y];

        public PipeMapPath[] StartingFrom(Point pt) => PipeMapPath.PathsFromPoint(ParsedMap, pt);

        public static int ValidDirectionsOfTravel(char c, int? inboundDirection = default)
        {
            var directions = c switch
            {
                'F' => Compass.East | Compass.South,
                'J' => Compass.West | Compass.North,
                '7' => Compass.South | Compass.West,
                'L' => Compass.North | Compass.East,
                '-' => Compass.West | Compass.East,
                '|' => Compass.North | Compass.South,

                _ => 0
            };

            if (inboundDirection is null) return directions;

            return inboundDirection.Value switch
            {
                Compass.East => directions &= ~Compass.West,
                Compass.West => directions &= ~Compass.East,
                Compass.South => directions &= ~Compass.North,
                Compass.North => directions &= ~Compass.South,
                Compass.SouthEast => directions &= ~Compass.NorthWest,
                Compass.SouthWest => directions &= ~Compass.NorthEast,
                Compass.NorthEast => directions &= ~Compass.SouthWest,
                Compass.NorthWest => directions &= ~Compass.SouthEast,

                _ => directions
            };
        }

        private static char ComponentAt(char[,] map, Point pt)
        {
            if (map[pt.X, pt.Y] != 'S') return map[pt.X, pt.Y];

            var w = map.GetLength(0);
            var h = map.GetLength(1);

            var l = pt.X <= 0 ? 0 : "-LF".Contains(map[pt.X - 1, pt.Y]) ? 1 : 0;
            var r = pt.X >= w ? 0 : "-J7".Contains(map[pt.X + 1, pt.Y]) ? 2 : 0;
            var a = pt.Y <= 0 ? 0 : "|F7".Contains(map[pt.X, pt.Y - 1]) ? 4 : 0;
            var b = pt.Y >= h ? 0 : "|LJ".Contains(map[pt.X, pt.Y + 1]) ? 8 : 0;

            return (l | r | a | b) switch
            {
                8 + 4 => '|',
                8 + 2 => 'F',
                8 + 1 => '7',
                4 + 2 => 'L',
                4 + 1 => 'J',
                2 + 1 => '-',

                _ => throw new ApplicationException($"Unable to determine the type of pipe component at [{pt}]")
            };
        }

        public sealed class PipeMapPath
        {
            private readonly char[,] _map;
            private readonly Point _start;
            private int _currentDirection;
            private List<Point> _locationsVisited;

            public Point CurrentLocation { get; private set; }

            public Point[] LocationsVisited => _locationsVisited.ToArray();

            public long Ticks { get; private set; } = 0;

            public PipeMapPath(char[,] map, Point start, int intialDirectionOfTravel)
            {
                _map = map;
                _currentDirection = intialDirectionOfTravel;
                _start = start;

                _locationsVisited = new([start]);

                CurrentLocation = start;
            }

            public static PipeMapPath[] PathsFromPoint(char[,] map, Point pt)
            {
                var paths = new List<PipeMapPath>();

                var c = map[pt.X, pt.Y];

                var directions = ValidDirectionsOfTravel(c);

                if ((directions & Compass.North) == Compass.North) paths.Add(new(map, pt, Compass.North));
                if ((directions & Compass.South) == Compass.South) paths.Add(new(map, pt, Compass.South));
                if ((directions & Compass.East) == Compass.East) paths.Add(new(map, pt, Compass.East));
                if ((directions & Compass.West) == Compass.West) paths.Add(new(map, pt, Compass.West));
                if ((directions & Compass.SouthWest) == Compass.SouthWest) paths.Add(new(map, pt, Compass.SouthWest));
                if ((directions & Compass.SouthEast) == Compass.SouthEast) paths.Add(new(map, pt, Compass.SouthEast));
                if ((directions & Compass.NorthWest) == Compass.NorthWest) paths.Add(new(map, pt, Compass.NorthWest));
                if ((directions & Compass.NorthEast) == Compass.NorthEast) paths.Add(new(map, pt, Compass.NorthEast));

                return [.. paths];
            }

            public Point? ContinueUntil(Predicate<PipeMapPath> predicate)
            {
                var w = _map.GetLength(0);
                var h = _map.GetLength(1);

                while (!predicate(this))
                {
                    var l = CurrentLocation;

                    var (x, y) = _currentDirection switch
                    {
                        Compass.North => (l.X, l.Y - 1),
                        Compass.South => (l.X, l.Y + 1),
                        Compass.East => (l.X + 1, l.Y),
                        Compass.West => (l.X - 1, l.Y),
                        Compass.SouthEast => (l.X + 1, l.Y + 1),
                        Compass.SouthWest => (l.X - 1, l.Y + 1),
                        Compass.NorthEast => (l.X + 1, l.Y - 1),
                        Compass.NorthWest => (l.X - 1, l.Y - 1),

                        _ => throw new ApplicationException($"Direction not supported {_currentDirection}.  Ensure this is a uni directional path map")
                    };

                    if (x >= w || y >= h || x < 0 || y < 0) return CurrentLocation;

                    var c = _map[x, y];

                    CurrentLocation = new Point(x, y);

                    _locationsVisited.Add(CurrentLocation);

                    _currentDirection = PipeMap.ValidDirectionsOfTravel(c, _currentDirection);

                    Ticks++;
                }

                return CurrentLocation;
            }
        }
    }

}
