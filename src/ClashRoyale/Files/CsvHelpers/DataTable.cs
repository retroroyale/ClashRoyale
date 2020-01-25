using System.Collections.Generic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvHelpers
{
    public class DataTable
    {
        public List<Data> Datas;
        public Csv.Files Index;

        public DataTable()
        {
            Datas = new List<Data>();
        }

        public DataTable(Table table, Csv.Files index)
        {
            Index = index;
            Datas = new List<Data>();

            for (var i = 0; i < table.GetRowCount(); i += 2)
            {
                var row = table.GetRowAt(i);
                var data = Csv.Create(Index, row, this);
                Datas.Add(data);
            }
        }

        public int Count()
        {
            return Datas?.Count ?? 0;
        }

        public List<Data> GetDatas()
        {
            return Datas;
        }

        public Data GetDataWithId(int id)
        {
            return Datas[GlobalId.GetInstanceId(id)];
        }

        public T GetDataWithId<T>(int id) where T : Data
        {
            return Datas[GlobalId.GetInstanceId(id)] as T;
        }

        public T GetDataWithInstanceId<T>(int id) where T : Data
        {
            if (Datas.Count < id) return null;

            return Datas[id] as T;
        }

        public T GetData<T>(string name) where T : Data
        {
            return Datas.Find(data => data.GetName() == name) as T;
        }

        public int GetIndex()
        {
            return (int) Index;
        }
    }
}