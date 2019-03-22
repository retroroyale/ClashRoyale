using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home
{
    public class Arena
    {
        [JsonIgnore]
        public Home Home { get; set; }

        public int CurrentArena { get; set; }

        public void AddTrophies(int trophies)
        {
            // TODO
        }
    }
}