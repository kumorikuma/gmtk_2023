using UnityEngine;

namespace Kotorman
{
    namespace Rope
    {
        public class RopeNode : MonoBehaviour
        {
            public SpriteRenderer spriteRenderer;

            [System.NonSerialized] public Vector3 PreviousPosition;
            public Transform pinnedTo;
            [System.NonSerialized] public int NodeIndex;
            public float Weight = 1.0f;

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
