using BlueOrb.Base.Config;
using UnityEditor;

namespace BlueOrb.Editor.Skill
{
    [CustomEditor(typeof(GameSettingsConfig), true)]
    public class GameSettingsConfigEditor : ConfigEditorBase<GameSettingsConfig>
    {
        [MenuItem("Assets/Create/Blue Orb/Game Settings Config")]
        public static void CreateNewAsset()
        {
            CreateAsset("NewGameSettingsConfig.asset");
        }
    }
}
