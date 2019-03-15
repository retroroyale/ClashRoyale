using System.Collections.Generic;
using System.Linq;

namespace ClashRoyale.Files.CsvReader
{
    public class Column
    {
        private readonly List<string> _values;

        public Column()
        {
            _values = new List<string>();
        }

        public static int GetArraySize(int offset, int nOffset)
        {
            return nOffset - offset;
        }

        public void Add(string value)
        {
            if (value == null)
                _values.Add(_values.Count > 0 ? _values.Last() : string.Empty);
            else
                _values.Add(value);
        }

        public string Get(int row)
        {
            return _values[row];
        }

        public int GetSize()
        {
            return _values.Count;
        }
    }
}