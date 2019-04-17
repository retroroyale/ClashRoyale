using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class EventOutput : Data
    {
        public EventOutput(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }
        public int Id { get; set; }
        public int Channels { get; set; }
        public int DurationMillis { get; set; }
    }
}