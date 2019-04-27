namespace ClashRoyale.Files.CsvReader
{
    public class Row
    {
        private readonly int _rowStart;
        private readonly Table _table;

        public Row(Table table)
        {
            _table = table;
            _rowStart = _table.GetColumnRowCount();

            _table.AddRow(this);
        }

        public int GetArraySize(string name)
        {
            var index = _table.GetColumnIndexByName(name);
            return index != -1 ? _table.GetArraySizeAt(this, index) : 0;
        }

        public string GetName()
        {
            return _table.GetValueAt(0, _rowStart);
        }

        public int GetRowOffset()
        {
            return _rowStart;
        }

        public string GetValue(string name, int level)
        {
            return _table.GetValue(name, level + _rowStart);
        }
    }
}