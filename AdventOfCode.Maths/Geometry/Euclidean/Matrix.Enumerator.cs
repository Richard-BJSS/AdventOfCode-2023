using System.Collections;
using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public partial class Matrix<T>
    {
        private sealed class MatrixEnumerator(T[,] entries, Point origin)
            : IEnumerator<(Point, T)>
        {
            private Size _size = new(entries.GetLength(0), entries.GetLength(1));
            private Point _current;

            public (Point, T) Current => (_current, entries[_current.X, _current.Y]);

            object IEnumerator.Current => (_current, entries[_current.X, _current.Y]);

            public void Dispose() { }

            public void Reset() => _current = Point.Empty;

            public bool MoveNext()
            {
                var current = _current == Point.Empty ? origin : _current;

                var x = current.X - origin.X + 1;
                var y = current.Y - origin.Y;

                if (x >= _size.Width) { x = 0; y += 1; };

                if (y >= _size.Height) return false;

                _current = new(x + origin.X, y + origin.Y);

                return true;
            }
        }
    }
}
