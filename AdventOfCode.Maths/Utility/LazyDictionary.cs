using System.Collections;

namespace AdventOfCode.Utility
{
    public sealed class LazyDictionary<K, V>(Func<K, V> factory) 
        : IDictionary<K, V> where K : notnull
    {
        private readonly Dictionary<K, V> _dictionary = [];

        public bool IsReadOnly => false;

        public IReadOnlyDictionary<K, V> AsReadOnly() => _dictionary.AsReadOnly();

        public int Count => _dictionary.Count;

        public ICollection<K> Keys => _dictionary.Keys;

        public ICollection<V> Values => _dictionary.Values;

        public V this[K key]
        {
            get => _dictionary.TryGetValue(key, out var value) ? value :_dictionary[key] = factory(key);
            set => _dictionary[key] = value;
        }

        public void Add(K key, V value) => _dictionary.Add(key, value);
        public void Add(KeyValuePair<K, V> kvp) => _dictionary[kvp.Key] = kvp.Value;

        public bool Remove(K key) => _dictionary.Remove(key);
        public bool Remove(KeyValuePair<K, V> kvp) => _dictionary.Remove(kvp.Key);

        public bool ContainsKey(K key) => _dictionary.ContainsKey(key);
        public bool Contains(KeyValuePair<K, V> value) => _dictionary.Contains(value);
        
        public bool TryGetValue(K key, out V value)
        {
            value = this[key];

            return true;
        }

        public void Clear() => _dictionary.Clear();

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) => ((ICollection<KeyValuePair<K, V>>)_dictionary).CopyTo(array, arrayIndex);
        
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => _dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();       
    }
}
