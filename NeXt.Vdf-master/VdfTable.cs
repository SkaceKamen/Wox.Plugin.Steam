using System.Collections.Generic;
using System;

namespace NeXt.Vdf
{
    /// <summary>
    /// A VdfValue that represents a table containing other VdfValues
    /// </summary>
    public sealed class VdfTable : VdfValue, IList<VdfValue>
    {
        public VdfTable(string name) : base(name)
        {
        }

        private readonly List<VdfValue> _values = new List<VdfValue>();
        private readonly Dictionary<string, VdfValue> _valuelookup = new Dictionary<string, VdfValue>();

        public int IndexOf(VdfValue item)
        {
            return _values.IndexOf(item);
        }

        public void Insert(int index, VdfValue item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (index < 0 || index >= _values.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                throw new ArgumentException("item name cannot be empty or null");
            }

            if (TryGetValue(item.Name, out _))
            {
                throw new ArgumentException($"a value with name {item.Name} already exists in the table");
            }


            item.Parent = this;

            _values.Insert(index, item);
            _valuelookup.Add(item.Name, item);
        }

        public void RemoveAt(int index)
        {
            var val = _values[index];
            _values.RemoveAt(index);
            _valuelookup.Remove(val.Name);
        }

        public VdfValue this[int index]
        {
            get => _values[index];
            set
            {
                if (_values[index].Name != value.Name)
                {
                    _valuelookup.Remove(_values[index].Name);
                    _valuelookup.Add(value.Name, value);
                }
                else
                {
                    _valuelookup[value.Name] = value;
                }

                _values[index] = value;
            }
        }

        public void Add(VdfValue item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                throw new ArgumentException("item name cannot be empty or null");
            }

            if (TryGetValue(item.Name, out _))
            {
                throw new ArgumentException($"a value with name {item.Name} already exists in the table");
            }

            item.Parent = this;

            _values.Add(item);
            _valuelookup.Add(item.Name, item);
        }

        public void Clear()
        {
            _values.Clear();
            _valuelookup.Clear();
        }

        public bool Contains(VdfValue item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                throw new ArgumentException("item name cannot be empty or null");
            }

            return _valuelookup.TryGetValue(item.Name, out var value) && value == item;
        }

        public bool TryGetValue(string key, out VdfValue value)
        {
            return _valuelookup.TryGetValue(key, out value);
        }

        public VdfValue GetByName(string name)
        {
            return _valuelookup.TryGetValue(name, out var value) ? value : null;
        }

        public void CopyTo(VdfValue[] array, int arrayIndex)
        {
            _values.CopyTo(array, arrayIndex);
        }

        public int Count => _values.Count;

        public bool Remove(VdfValue item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                throw new ArgumentException("item name cannot be empty or null");
            }

            if (!Contains(item))
            {
                return false;
            }

            _valuelookup.Remove(item.Name);
            _values.Remove(item);
            return true;
        }

        public IEnumerator<VdfValue> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        bool ICollection<VdfValue>.IsReadOnly => false;

        public override VdfValueType Type => VdfValueType.Table;
    }
}