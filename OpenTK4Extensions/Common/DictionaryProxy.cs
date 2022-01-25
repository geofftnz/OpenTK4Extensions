using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Common
{
    public class DictionaryProxy<TKey, TToValue, TFromValue> : IReadOnlyDictionary<TKey, TToValue> where TFromValue : TToValue
    {
        private Dictionary<TKey, TFromValue> _baseDictionary;

        public DictionaryProxy(Dictionary<TKey, TFromValue> baseDictionary)
        {
            _baseDictionary = baseDictionary;
        }

        public TToValue this[TKey key] => _baseDictionary[key];

        public IEnumerable<TKey> Keys => _baseDictionary.Keys;

        public IEnumerable<TToValue> Values => _baseDictionary.Values.Select(v => (TToValue)v);

        public int Count => _baseDictionary.Count;

        public bool ContainsKey(TKey key) => _baseDictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<TKey, TToValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TToValue value)
        {
            if (_baseDictionary.TryGetValue(key, out TFromValue innerValue))
            {
                value = innerValue;
                return true;
            }
            else
            {
                value = default;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
