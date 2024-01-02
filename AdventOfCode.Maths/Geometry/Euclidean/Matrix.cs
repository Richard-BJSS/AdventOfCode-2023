using System.Collections;
using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    // TODO: 
    // 1. Matrix is a specialised Directed Graph - add appropriate Base Class
    // 2. As a graph, a vector (array with 2 entries) can be used to describe each edge (path) between two nodes (cells)

    public sealed partial class Matrix<T>(T[,] entries)
        : IEnumerable<(Point Point, T Value)>
    {
        private readonly T[,] _entries = entries;
        private T[][]? _jaggedEntries;

        public Matrix(T[][] entries) : this(entries.ToRectangularArray()) { }

        public static implicit operator T[,](Matrix<T> matrix) => matrix._entries;
        public static implicit operator T[][](Matrix<T> matrix) => matrix._entries.ToJaggedArray();
        public static implicit operator Matrix<T>(T[,] entries) => new(entries);
        public static implicit operator Matrix<T>(T[][] entries) => new(entries);

        public T this[Point pt]
        {
            get => _entries[pt.X - Offset.X, pt.Y - Offset.Y];
            set => _entries[pt.X - Offset.X, pt.Y - Offset.Y] = value;
        }

        public T this[(int X, int Y) pt]
        {
            get => _entries[pt.X - Offset.X, pt.Y - Offset.Y];
            set => _entries[pt.X - Offset.X, pt.Y - Offset.Y] = value;
        }

        public Rectangle Rectangle => new(Offset, Size);

        public Size Size => new(_entries.GetLength(0), _entries.GetLength(1));

        public Point Offset { get; set; } = Point.Empty;

        public Point Centre => new((Size.Width / 2) + Offset.X, (Size.Height / 2) + Offset.Y);

        public T[] RowAt(int index) => (_jaggedEntries ??= _entries.ToJaggedArray())[index];
        public T[] ColAt(int index)
        {
            var r = new T[_entries.GetLength(0)];

            for (var y = 0; y < r.Length; y++)
            {
                r[y] = _entries[index, y];
            }

            return r;
        }

        public T? ValueAt(Point pt) => _entries[pt.X - Offset.X, pt.Y - Offset.Y];
        public T? ValueAt(int x, int y) => _entries[x - Offset.X, y - Offset.Y];

        public T[][] Rotate90CW() => _entries.Rotate90CW();
        public T[][] Rotate90CCW() => _entries.Rotate90CCW();

        IEnumerator<(Point Point, T Value)> IEnumerable<(Point Point, T Value)>.GetEnumerator() => new MatrixEnumerator(_entries, Offset);

        IEnumerator IEnumerable.GetEnumerator() => new MatrixEnumerator(_entries, Offset);
    }
}