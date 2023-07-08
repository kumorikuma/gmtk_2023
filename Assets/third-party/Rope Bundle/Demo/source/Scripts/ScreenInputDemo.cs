using UnityEngine;

namespace Kotorman
{
    namespace Rope
    {
        public class ScreenInputDemo : MonoBehaviour
        {
            [SerializeField] Camera cam;
            [SerializeField] Transform target;
            
            
            void Update()
            {
                if(Input.GetMouseButton(0))
                {
                    target.position = cam.ScreenToWorldPoint(Input.mousePosition);
                }
            }
        }
    }
}