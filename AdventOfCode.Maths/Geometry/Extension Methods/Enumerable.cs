namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public static class EnumerableExtensions
    {
        public static Matrix<char> ToMatrix(this IEnumerable<string> source) => source.ToJaggedArray().ToMatrix();

        public static Matrix<T> ToMatrix<T>(this IEnumerable<string> source, Func<char, T> converter) => source.Select(s => s.Select(converter).ToArray()).ToArray().ToMatrix();

        public static IEnumerable<string> FlipVertically(this IEnumerable<string> source) => source.Reverse();

        public static IEnumerable<string> FlipHorizontally(this IEnumerable<string> source) => source.Select(s => new string(s.Reverse().ToArray()));

        public static IEnumerable<string> Rotate180(this IEnumerable<string> source) => source.FlipHorizontally().FlipVertically();

        public static IEnumerable<string> Rotate90CW(this IEnumerable<string> source) => source.Select(s => s.ToCharArray()).ToArray().Rotate90CW().Select(cs => new string(cs));

        public static IEnumerable<string> Rotate90CCW(this IEnumerable<string> source) => source.Select(s => s.ToCharArray()).ToArray().Rotate90CCW().Select(cs => new string(cs));

        public static char[,] ToRectangularArray(this IEnumerable<char[]> source) => source.ToArray().ToRectangularArray();

        public static char[][] ToJaggedArray(this IEnumerable<string> source) => source.Select(s => s.ToCharArray()).ToArray();
    }
}
