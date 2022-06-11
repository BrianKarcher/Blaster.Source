using System.Collections.Generic;

namespace BlueOrb.Controller.Persistence
{
    public class PersistData
    {
        public string LootLockerDeviceId { get; set; }
        public string LootLockerMemberId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        // Scene Unique Id : score
        public Dictionary<string, int> HighScores { get; set; }
    }
}