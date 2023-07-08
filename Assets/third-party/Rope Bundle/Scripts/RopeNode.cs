using UnityEngine;

namespace Kotorman
{
    namespace Rope
    {
        public class RopeNode : MonoBehaviour
        {
            public SpriteRenderer spriteRenderer;
            
            [System.NonSerialized] public Vector3 PreviousPosition;
            [System.NonSerialized] public Transform pinnedTo;

            public bool IsPinned() {
                return pinnedTo != null;
            }

            public void MaybeUpdatePositionToPinnedTransform() {
                if (pinnedTo != null) {
                    transform.position = pinnedTo.transform.position;
                }
            }
        }
    }
}
