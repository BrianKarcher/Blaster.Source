using System.Collections.Generic;

namespace BlueOrb.Controller.Persistence
{
    public class PersistUserScores
    {
        // Scene Unique Id : score
        public Dictionary<string, int> HighScores { get; set; }
    }
}