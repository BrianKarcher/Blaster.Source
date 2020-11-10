using BlueOrb.Common.Components;
using UnityEngine;

namespace BlueOrb.Controller.Block
{
    [AddComponentMenu("RQ/Components/Block")]
    public class BlockComponent : ComponentBase<BlockComponent>
    {
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("Block Component is awake!");
        }
    }
}
