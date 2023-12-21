using AdventOfCode.Maths.Geometry.Euclidean;
using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    // TODO: 
    // 1. Matrix is a specialised Directed Graph - add appropriate Base Class
    // 2. As a graph, a vector can be used to describe each edge (path) between two nodes (cells)

    public sealed class Matrix<T>(T[,] entries)
    {
        private readonly T[,] _entries = entries;
        private T[][]? _jaggedEntries;

        public Matrix(T[][] entries) : this(entries.ToRectangularArray()) { }

        public static implicit operator T[,](Matrix<T> matrix) => matrix._entries;
        public static implicit operator T[][](Matrix<T> matrix) => matrix._entries.ToJaggedArray();
        public static implicit operator Matrix<T>(T[,] entries) => new(entries);
        public static implicit operator Matrix<T>(T[][] entries) => new(entries);

        public Rectangle Rectangle => new(Origin, Size);

        public Size Size => new(_entries.GetLength(0), _entries.GetLength(1));

        public Point Origin { get; set; } = Point.Empty;

        public T[] RowAt(int index) => (_jaggedEntries ??= _entries.ToJaggedArray())[index];

        public T? ValueAt(Point pt) => _entries[pt.X + Origin.X, pt.Y + Origin.Y];
        public T? ValueAt(int x, int y) => _entries[x + Origin.X, y + Origin.Y];

        public T[][] Rotate90CW() => _entries.Rotate90CW();
        public T[][] Rotate90CCW() => _entries.Rotate90CCW();

        public IEnumerable<Point> Entries(Predicate<Point> predicate)
        {
            for (var x = 0; x < _entries.GetLength(0); x++)
            {
                for (var y = 0; y < _entries.GetLength(1); y++)
                {
                    var pt = new Point(x, y);

                    if (predicate(pt)) yield return pt;
                }
            }
        }
    }
}

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public static partial class GeometryExtensions
    {
        public static Matrix<T> ToMatrix<T>(this T[][] source)
        {
            var xs = source[0].Length;
            var ys = source.Length;

            var m = new T[xs, ys];

            for (var x = 0; x < xs; x++)
                for (var y = 0; y < ys; y++)
                    m[x, y] = source[y][x];

            return new Matrix<T>(m);
        }


        public static Matrix<char> ToMatrix(this IEnumerable<string> source) => source.ToJaggedArray().ToMatrix();
        public static Matrix<T> ToMatrix<T>(this IEnumerable<string> source, Func<char, T> converter) => source.Select(s => s.Select(converter).ToArray()).ToArray().ToMatrix();

        public static T[][] ToJaggedArray<T>(this T[,] source)
        {
            var mdw = source.GetLength(0);
            var mdh = source.GetLength(1);

            var r = new T[mdh][];

            for (var y = 0; y < mdh; y++)
            {
                r[y] = new T[mdw];

                for (var x = 0; x < mdw; x++)
                {
                    r[y][x] = source[x, y];
                }
            }

            return r;
        }
        public static char[][] ToJaggedArray(this IEnumerable<string> source) => source.Select(s => s.ToCharArray()).ToArray();

        public static T[,] ToRectangularArray<T>(this T[][] source)
        {
            var r = new T[source[0].Length, source.Length];

            for (var ys = 0; ys < source.Length; ys++)
            {
                var xs = source[ys];

                for (var x = 0; x < xs.Length; x++)
                {
                    r[x, ys] = xs[x];
                }
            }

            return r;
        }
        public static char[,] ToRectangularArray(this string[] source) => source.Select(s => s.ToCharArray()).ToRectangularArray();
        public static char[,] ToRectangularArray(this IEnumerable<char[]> source) => source.ToArray().ToRectangularArray();

        public static IEnumerable<string> Rotate90CW(this IEnumerable<string> source) => source.Select(s => s.ToCharArray()).ToArray().Rotate90CW().Select(cs => new string(cs));
        public static IEnumerable<string> Rotate90CCW(this IEnumerable<string> source) => source.Select(s => s.ToCharArray()).ToArray().Rotate90CCW().Select(cs => new string(cs));

        public static string[] Rotate90CW(this string[] source) => source.Select(s => s.ToCharArray()).ToArray().Rotate90CW().Select(cs => new string(cs)).ToArray();
        public static string[] Rotate90CCW(this string[] source) => source.Select(s => s.ToCharArray()).ToArray().Rotate90CCW().Select(cs => new string(cs)).ToArray();

        public static T[][] Rotate90CW<T>(this T[,] source) => ArrayExtensions.Zip(source.ToJaggedArray()).Select(m => m.Reverse().ToArray()).ToArray();
        public static T[][] Rotate90CCW<T>(this T[,] source) => ArrayExtensions.Zip(source.ToJaggedArray()).Reverse().ToArray();

        public static T[][] Rotate90CW<T>(this T[][] source) => ArrayExtensions.Zip(source).Select(m => m.Reverse().ToArray()).ToArray();
        public static T[][] Rotate90CCW<T>(this T[][] source) => ArrayExtensions.Zip(source).Reverse().ToArray();

        public static IEnumerable<string> Rotate180(this IEnumerable<string> source) => source.FlipHorizontally().FlipVertically();
        public static string[] Rotate180(this string[] source) => source.FlipHorizontally().FlipVertically().ToArray();

        public static IEnumerable<string> FlipVertically(this IEnumerable<string> source) => source.Reverse();
        public static IEnumerable<string> FlipHorizontally(this IEnumerable<string> source) => source.Select(s => new string(s.Reverse().ToArray()));

        public static int ManhattanDistance(Point a, Point b) => Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
        public static int ChebyshevDistance(Point a, Point b) => Math.Max(Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y));

    }

    public static partial class ArrayExtensions
    {
        public static T[][] Zip<T>(T[][] source)
        {
            var r = new T[source[0].Length][];

            for (var x = 0; x < source[0].Length; x++)
            {
                r[x] = new T[source.Length];

                for (var y = 0; y < source.Length; y++)
                {
                    r[x][y] = source[y][x];
                }
            }

            return r;
        }
    }
}
