using System;
using System.Collections.Generic;
using ClashRoyale.Files.CsvHelpers;

namespace ClashRoyale.Files.CsvReader
{
    public class Gamefiles : IDisposable
    {
        private readonly List<DataTable> _dataTables = new List<DataTable>();

        public Gamefiles()
        {
            if (Csv.Gamefiles.Count > 0)
                for (var i = 0; i < Csv.Gamefiles.Count; i++)
                    _dataTables.Add(new DataTable());
        }

        public void Dispose()
        {
            _dataTables.Clear();
        }

        public DataTable Get(Csv.Types index)
        {
            return _dataTables[(int) index - 1];
        }

        public DataTable Get(int index)
        {
            return _dataTables[index - 1];
        }

        public void Initialize(Table table, Csv.Types index)
        {
            _dataTables[(int)index - 1] = new DataTable(table, index);
        }
    }
}