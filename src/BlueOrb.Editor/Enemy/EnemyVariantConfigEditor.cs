using BlueOrb.Controller.Enemy;
using UnityEditor;

namespace BlueOrb.Editor.Enemy
{
    [CustomEditor(typeof(EnemyVariantConfig), true)]
    public class EnemyVariantConfigEditor : ConfigEditorBase<EnemyVariantConfig>
    {
        [MenuItem("Assets/Create/Blue Orb/Enemy Variant Config")]
        public static void CreateNewAsset()
        {
            var newAsset = CreateAsset("NewEnemyVariant.asset");
        }
    }
}