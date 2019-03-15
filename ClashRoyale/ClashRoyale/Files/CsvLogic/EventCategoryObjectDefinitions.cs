using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class EventCategoryObjectDefinitions : Data
    {
        public EventCategoryObjectDefinitions(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string PropertyName { get; set; }

        public string PropertyType { get; set; }

        public bool IsRequired { get; set; }

        public string ObjectType { get; set; }

        public int DefaultInt { get; set; }

        public string DefaultString { get; set; }
    }
}