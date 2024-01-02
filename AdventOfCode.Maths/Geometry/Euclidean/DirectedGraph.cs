using AdventOfCode.Utility;

namespace AdventOfCode.Maths.Geometry.Euclidean
{
    public sealed class DirectedGraph<T> 
        where T : IEquatable<T>
    {
        public readonly record struct Edge(T StartsFrom, T EndsAt);

        private IDictionary<T, HashSet<T>> _endings = new LazyDictionary<T, HashSet<T>>(_ => []);
        private IDictionary<T, HashSet<T>> _beginnings { get; } = new LazyDictionary<T, HashSet<T>>(_ => []);

        public IReadOnlyDictionary<T, HashSet<T>> EdgeEndPoints => _endings.AsReadOnly();
        public IReadOnlyDictionary<T, HashSet<T>> EdgeStartPoint => _beginnings.AsReadOnly();

        public void Add(Edge edge) => Add(edge.StartsFrom, edge.EndsAt);

        public void Add(T startsFrom, T endsAt) { _endings[endsAt].Add(startsFrom); _beginnings[startsFrom].Add(endsAt); }

        public void Remove(Edge edge) => Remove(edge.StartsFrom, edge.EndsAt);

        public void Remove(T startsFrom, T endsAt) { _endings[endsAt].Remove(startsFrom); _beginnings[startsFrom].Remove(endsAt); }
    }
}
