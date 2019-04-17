using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class EventCategoryEnums : Data
    {
        public EventCategoryEnums(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 76);
        }

        public string Name { get; set; }
        public string Option { get; set; }
    }
}