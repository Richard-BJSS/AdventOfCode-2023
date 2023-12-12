﻿using AdventOfCode.Maths;
using System.Drawing;

namespace AdventOfCode.Maths
{
    public sealed class Matrix<T>(T[,] entries)
    {
        private readonly T[,] _entries = entries;
        private T[][] _jaggedEntries;

        public Matrix(T[][] entries) : this(entries.ToRectangularArray()) { }

        public static implicit operator T[,](Matrix<T> matrix) => matrix._entries;
        public static implicit operator T[][](Matrix<T> matrix) => matrix._entries.ToJaggedArray();
        public static implicit operator Matrix<T> (T[,] entries) => new (entries);
        public static implicit operator Matrix<T>(T[][] entries) => new (entries);

        public T? ValueAt(Point pt) => entries[pt.X, pt.Y];
        public T? ValueAt(int x, int y) => entries[x, y];

        public T[][] Rotate90CW() => Geometry.Rotate90CW(_entries);
        public T[][] Rotate90CCW() => Geometry.Rotate90CCW(_entries);

        public IEnumerable<Point> Cells(Predicate<T> predicate) => Cells((_, e) => predicate(e));
        public IEnumerable<Point> Cells(Func<Point, T, bool> predicate)
        {
            _jaggedEntries ??= _entries.ToJaggedArray();

            for (var y = 0; y < _jaggedEntries.Length; y++)
            {
                for (var x = 0; x < _jaggedEntries[0].Length; x++)
                {
                    var pt = new Point(x, y);

                    if (predicate(pt, _jaggedEntries[y][x])) yield return pt;
                }
            }
        }
    }
}

namespace AdventOfCode
{
    public static partial class Geometry
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
        
        public static T[][] ToJaggedArray<T>(this T[,] multiDim)
        {
            var mdw = multiDim.GetLength(0);
            var mdh = multiDim.GetLength(1);

            var r = new T[mdh][];

            for (var y = 0; y < mdh; y++)
            {
                r[y] = new T[mdw];

                for (var x = 0; x < mdw; x++)
                {
                    r[y][x] = multiDim[x, y];
                }
            }

            return r;
        }

        public static T[,] ToRectangularArray<T>(this T[][] jagged)
        {
            var r = new T[jagged[0].Length, jagged.Length];

            for (var ys = 0; ys < jagged.Length; ys++)
            {
                var xs = jagged[ys];

                for (var x = 0; x < xs.Length; x++)
                {
                    r[x, ys] = xs[x];
                }
            }

            return r;
        }

        public static T[][] Rotate90CW<T>(T[,] entries) => ArrayExtensions.Zip(entries.ToJaggedArray()).Select(m => m.Reverse().ToArray()).ToArray();
        public static T[][] Rotate90CCW<T>(T[,] entries) => ArrayExtensions.Zip(entries.ToJaggedArray()).Reverse().ToArray();
        public static T[][] Rotate90CW<T>(T[][] entries) => ArrayExtensions.Zip(entries).Select(m => m.Reverse().ToArray()).ToArray();
        public static T[][] Rotate90CCW<T>(T[][] entries) => ArrayExtensions.Zip(entries).Reverse().ToArray();


        public static int ManhattanDistance(Point a, Point b) => Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);

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
