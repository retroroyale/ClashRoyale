using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvClient
{
    public class Sounds : Data
    {
        public Sounds(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }

        public string FileNames { get; set; }

        public int MinVolume { get; set; }

        public int MaxVolume { get; set; }

        public int MinPitch { get; set; }

        public int MaxPitch { get; set; }

        public int Priority { get; set; }

        public int MaximumByType { get; set; }

        public int MaxRepeatMs { get; set; }

        public bool Loop { get; set; }

        public bool PlayVariationsInSequence { get; set; }

        public bool PlayVariationsInSequenceManualReset { get; set; }

        public int StartDelayMinMs { get; set; }

        public int StartDelayMaxMs { get; set; }

        public bool PlayOnlyWhenInView { get; set; }

        public int MaxVolumeScaleLimit { get; set; }

        public int NoSoundScaleLimit { get; set; }

        public int PadEmpyToEndMs { get; set; }
    }
}