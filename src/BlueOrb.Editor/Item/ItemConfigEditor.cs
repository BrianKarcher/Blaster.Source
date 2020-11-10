using BlueOrb.Base.Item;
using UnityEditor;

namespace BlueOrb.Editor.Skill
{
    [CustomEditor(typeof(ItemConfig), true)]
    public class ItemConfigEditor : ConfigEditorBase<ItemConfig>
    {
        [MenuItem("Assets/Create/RQ/Item/Item Config")]
        public static void CreateNewAsset()
        {
            CreateAsset("NewItemConfig.asset");
        }
    }
}
