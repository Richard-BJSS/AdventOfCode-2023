using System.Collections;
using System.Numerics;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public readonly struct Range<T>(T min, T max) 
        : IEnumerable<T>, 
          IEquatable<Range<T>>
        where T : IBinaryNumber<T>
    {
        public T Min { get; } = min;
        public T Max { get; } = max;
        public T Length => Max - Min + T.One;

        public static bool Intersects(Range<T> a, Range<T> b) => a.Max >= b.Min && a.Min <= b.Max;

        public static Range<T> Intersection(Range<T> a, Range<T> b)
        {
            if (!Intersects(a, b)) throw new ApplicationException("The two ranges do not intersect");

            var limits = new[] { a.Min, a.Max, b.Min, b.Max }.Order().ToArray();
            
            return new(min: limits[1], max: limits[2]);
        }

        public bool Contains(Range<T> b) => Contains(b.Min) && Contains(b.Max);

        public bool Contains(T value) =>  value >= Min && value <= Max;

        public override string ToString() => $"[{Min} to {Max}]";

        public IEnumerator<T> GetEnumerator()
        {
            for (var n = Min; n <= Max; n++)
            {
                yield return n;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Range<T> other) => Min == other.Min && Max == other.Max;
       
        public override bool Equals(object? obj) => obj is Range<T> other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Min, Max);

        public static bool operator ==(Range<T> left, Range<T> right) => left.Equals(right);
        
        public static bool operator !=(Range<T> left, Range<T> right) => !left.Equals(right);

        public static Range<T> AsSingle(T value) => new(min: value, max: value);
    }
}
