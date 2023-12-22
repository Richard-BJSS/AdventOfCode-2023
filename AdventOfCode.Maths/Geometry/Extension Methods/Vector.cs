namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public static partial class ArrayExtensions
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

        public static string[] Rotate90CW(this string[] source) => source.Select(s => s.ToCharArray()).ToArray().Rotate90CW().Select(cs => new string(cs)).ToArray();
        
        public static string[] Rotate90CCW(this string[] source) => source.Select(s => s.ToCharArray()).ToArray().Rotate90CCW().Select(cs => new string(cs)).ToArray();

        public static T[][] Rotate90CW<T>(this T[,] source) => ArrayExtensions.Zip(source.ToJaggedArray()).Select(m => m.Reverse().ToArray()).ToArray();
        
        public static T[][] Rotate90CCW<T>(this T[,] source) => ArrayExtensions.Zip(source.ToJaggedArray()).Reverse().ToArray();

        public static T[][] Rotate90CW<T>(this T[][] source) => ArrayExtensions.Zip(source).Select(m => m.Reverse().ToArray()).ToArray();
        public static T[][] Rotate90CCW<T>(this T[][] source) => ArrayExtensions.Zip(source).Reverse().ToArray();

        public static string[] Rotate180(this string[] source) => source.FlipHorizontally().FlipVertically().ToArray();
    }
}
