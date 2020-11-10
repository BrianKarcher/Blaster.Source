using BlueOrb.Base.Skill;
using UnityEditor;

namespace BlueOrb.Editor.Skill
{
    [CustomEditor(typeof(ShardConfig), true)]
    public class ShardConfigEditor : ConfigEditorBase<ShardConfig>
    {
        [MenuItem("Assets/Create/RQ/Skill/Shard Config")]
        public static void CreateNewAsset()
        {
            CreateAsset("NewShardConfig.asset");
        }
    }
}
