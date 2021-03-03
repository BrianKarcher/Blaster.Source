using UnityEngine;

namespace BlueOrb.Controller.Transforms
{
    [AddComponentMenu("BlueOrb/Components/Add Child On Start")]
    public class AddChildOnStart : MonoBehaviour
    {
        [SerializeField]
        private Transform _child;

        private void Start()
        {
            Debug.Log($"Reparenting to {name}");
            //_child.transform.parent = transform;
            _child.SetParent(transform, false);
            //_child.localRotation = Quaternion.Euler(0, 0, 0);
        }

        //private void OnDisable()
        //{
        //    //_child.transform.parent = null;
        //    _child.transform.SetParent(null);
        //}
    }
}
