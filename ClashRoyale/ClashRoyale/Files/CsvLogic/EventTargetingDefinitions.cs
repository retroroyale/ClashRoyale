using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class EventTargetingDefinitions : Data
    {
        public EventTargetingDefinitions(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string MetadataType { get; set; }

        public string MetadataPath { get; set; }

        public string EvaluationLocation { get; set; }

        public string ParameterName { get; set; }

        public string ParameterType { get; set; }

        public bool IsRequired { get; set; }

        public string ObjectType { get; set; }

        public string MatchingRuleType { get; set; }
    }
}