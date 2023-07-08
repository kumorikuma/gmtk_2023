using UnityEditor;
using UnityEngine;

namespace Kotorman
{
    namespace Rope
    {
        #if UNITY_EDITOR
        public class MenuItems : MonoBehaviour
        {
            [MenuItem("GameObject/Kotorman/Rope")]
            public static void CreateRope()
            {
                GameObject holder = new GameObject("Rope");

                GameObject pivot = new GameObject("pivot");
                pivot.transform.parent = holder.transform;

                GameObject body = new GameObject("body");
                body.transform.parent = holder.transform;

                //
                Rope r = body.AddComponent<Rope>();
                r.AttachTo(pivot.transform);

            }
        }
        #endif
    }
}