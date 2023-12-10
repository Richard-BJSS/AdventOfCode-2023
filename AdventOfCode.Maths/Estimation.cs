namespace AdventOfCode
{
    public static partial class LinearInterpolation
    {
        public static long Predict(long[] source)
        {
            var ds = source.ToSequencesOfDifferences();
            var xs = ds.Select(xs => xs[^1]);

            var nextVal = 0L;
            var prevVal = 0L;

            foreach (var x in xs) prevVal = nextVal = prevVal + x;

            return nextVal + source[^1];
        }

        public static ValueTask<long> PredictAsync(long[] source) => ValueTask.FromResult(Predict(source));

        public static IEnumerable<long> ToSequenceOfDifferences(this long[] source)
            => source?.Zip(source.Skip(1)).Select(p => p.Second - p.First)
               ?? Enumerable.Empty<long>() ;

        public static IEnumerable<long[]> ToSequencesOfDifferences(this long[] source)
        {
            var ds = source.ToSequenceOfDifferences().ToArray();

            if (ds.All(d => 0 == d)) yield break;

            yield return ds;

            foreach (var seq in ds.ToSequencesOfDifferences()) yield return seq;
        }
    }
}
