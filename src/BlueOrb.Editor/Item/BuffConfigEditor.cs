using BlueOrb.Controller.Buff;
using UnityEditor;

namespace BlueOrb.Editor.Skill
{
    [CustomEditor(typeof(BuffConfig), true)]
    public class BuffConfigEditor : ConfigEditorBase<BuffConfig>
    {
        [MenuItem("Assets/Create/Blue Orb/Buff/Buff Config")]
        public static void CreateNewAsset()
        {
            CreateAsset("New Buff Config.asset");
        }
    }
}