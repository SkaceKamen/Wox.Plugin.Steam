using System.Collections.Generic;

namespace WoxSteam.BinaryVdf
{
    public class BinaryVdfItem
    {
        private readonly Dictionary<string, BinaryVdfItem> _items = new Dictionary<string, BinaryVdfItem>();
        private readonly string _value;

        public BinaryVdfItem()
        {
        }

        public BinaryVdfItem(string value)
        {
            _value = value;
        }

        public BinaryVdfItem this[string index]
        {
            get => _items.TryGetValue(index, out var value) ? value : new BinaryVdfItem();
            set => _items[index] = value;
        }

        public string GetString(string index)
        {
            return this[index]._value;
        }
    }
}