using BlueOrb.Controller.Enemy;
using UnityEditor;

namespace BlueOrb.Editor.Enemy
{
    [CustomEditor(typeof(EnemyGroupConfig), true)]
    public class EnemyGroupConfigEditor : ConfigEditorBase<EnemyGroupConfig>
    {
        [MenuItem("Assets/Create/Blue Orb/Enemy Group Config")]
        public static void CreateNewAsset()
        {
            var newAsset = CreateAsset("NewEnemyGroup.asset");
        }
    }
}