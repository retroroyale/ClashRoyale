using System.Collections.Generic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvHelpers
{
    public class DataTable
    {
        public List<Data> Data;
        public Csv.Files Index;

        public DataTable()
        {
            Data = new List<Data>();
        }

        public DataTable(Table table, Csv.Files index)
        {
            Index = index;
            Data = new List<Data>();

            for (var i = 0; i < table.GetRowCount(); i++)
            {
                var row = table.GetRowAt(i);
                var data = Csv.Create(Index, row, this);

                Data.Add(data);
            }
        }

        public int Count()
        {
            return Data?.Count ?? 0;
        }

        public List<Data> GetDatas()
        {
            return Data;
        }

        public Data GetDataWithId(int id)
        {
            return Data[GlobalId.GetInstanceId(id)];
        }

        public T GetDataWithInstanceId<T>(int id) where T : Data
        {
            if (Data.Count < id) return null;

            return Data[id] as T;
        }

        public T GetData<T>(string name) where T : Data
        {
            return Data.Find(data => data.GetName() == name) as T;
        }

        public int GetIndex()
        {
            return (int) Index;
        }
    }
}