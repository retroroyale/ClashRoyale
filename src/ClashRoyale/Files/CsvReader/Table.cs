using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

namespace ClashRoyale.Files.CsvReader
{
    public class Table
    {
        private readonly List<Column> _columns;
        private readonly List<string> _headers;
        private readonly List<Row> _rows;
        private readonly List<string> _types;

        public Table(string path)
        {
            _rows = new List<Row>();
            _headers = new List<string>();
            _types = new List<string>();
            _columns = new List<Column>();

            using (var reader = new TextFieldParser(path))
            {
                reader.SetDelimiters(",");

                var columns = reader.ReadFields();

                foreach (var column in columns)
                {
                    _headers.Add(column);
                    _columns.Add(new Column());
                }

                var types = reader.ReadFields();

                foreach (var type in types) _types.Add(type);

                while (!reader.EndOfData)
                {
                    var values = reader.ReadFields();

                    if (!string.IsNullOrEmpty(values[0])) AddRow(new Row(this));

                    for (var i = 0; i < _headers.Count; i++) _columns[i].Add(values[i]);
                }
            }
        }

        public void AddRow(Row row)
        {
            _rows.Add(row);
        }

        public int GetArraySizeAt(Row row, int columnIndex)
        {
            var index = _rows.IndexOf(row) + 1;
            if (index == -1) return 0;

            int nextOffset;
            if (index + 1 >= _rows.Count)
            {
                nextOffset = _columns[columnIndex].GetSize();
            }
            else
            {
                var nextRow = _rows[index + 1];
                nextOffset = nextRow.Offset;
            }

            return Column.GetArraySize(row.Offset, nextOffset);
        }

        public int GetColumnIndexByName(string name)
        {
            return _headers.IndexOf(name);
        }

        public string GetColumnName(int index)
        {
            return _headers[index];
        }

        public int GetColumnRowCount()
        {
            return _columns.Count > 0 ? _columns[0].GetSize() : 0;
        }

        public Row GetRowAt(int index)
        {
            return _rows[index];
        }

        public int GetRowCount()
        {
            return _rows.Count;
        }

        public string GetValue(string name, int level)
        {
            var index = _headers.IndexOf(name);
            return GetValueAt(index, level);
        }

        public string GetValueAt(int column, int row)
        {
            return _columns[column].Get(row);
        }
    }
}