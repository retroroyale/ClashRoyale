using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class EventCategoryEnums : Data
    {
        public EventCategoryEnums(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }
        public string Option { get; set; }
    }
}