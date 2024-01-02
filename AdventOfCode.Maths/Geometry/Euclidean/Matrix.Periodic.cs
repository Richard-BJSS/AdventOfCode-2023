using System.Drawing;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    /// <summary>
    /// Represent a Matrix pinned to finite space that is continuously repeated in every direction and 
    /// thus logically exists in infinite space (is unbounded)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="matrix">The bound matrix that repeats</param>
    public sealed partial class PeriodicMatrix<T>(Matrix<T> matrix)
    {
        internal Matrix<T> Matrix { get; } = matrix;

        public T this[Point pt]
        {
            get => Matrix[ToLocal(pt, Matrix)];
            set => Matrix[ToLocal(pt, Matrix)] = value;
        }

        private static Point ToLocal(Point pt, Matrix<T> matrix) =>
            new(x: (pt.X - matrix.Offset.X).Modulo(matrix.Size.Width),
                y: (pt.Y - matrix.Offset.Y).Modulo(matrix.Size.Height));

    }
}
