using Kotorman.Rope;
using UnityEngine;

namespace Kotorman
{
    namespace Rope
    {
        public class WeightObj : MonoBehaviour
        {
            [SerializeField] Rope rope;

            private void Start()
            {
                rope.OnRopeNodesCountChanged += OnRopeChanged; //subscribe to event
                OnRopeChanged(); //...and make an init call
            }


            private void OnRopeChanged()
            {
                RopeNode n = rope.GetLastNode();
                this.transform.parent = n.transform;
                transform.localPosition = Vector2.zero;
                transform.rotation = n.transform.rotation;
            }
        }
    }
}
