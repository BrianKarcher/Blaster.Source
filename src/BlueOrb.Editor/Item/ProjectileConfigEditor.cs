using BlueOrb.Base.Item;
using UnityEditor;

namespace BlueOrb.Editor.Skill
{
    [CustomEditor(typeof(ProjectileConfig), true)]
    public class ProjectileConfigEditor : ConfigEditorBase<ProjectileConfig>
    {
        [MenuItem("Assets/Create/RQ/Item/Projectile Config")]
        public static void CreateNewAsset()
        {
            CreateAsset("NewProjectileConfig.asset");
        }
    }
}
