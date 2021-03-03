using UnityEngine;

namespace BlueOrb.Controller.Transforms
{
    [AddComponentMenu("BlueOrb/Components/Attach Joint On Start")]
    public class AttachJointOnStart : MonoBehaviour
    {
        [SerializeField]
        private Joint _otherJoint;

        private void Start()
        {
            Debug.Log($"Reparenting to {name}");
            _otherJoint.connectedBody = GetComponent<Rigidbody>();
            //_child.transform.parent = transform;
        }

        //private void OnDisable()
        //{
        //    //_child.transform.parent = null;
        //    _child.transform.SetParent(null);
        //}
    }
}
