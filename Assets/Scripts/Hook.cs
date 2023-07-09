using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kotorman.Rope;

public class Hook : MonoBehaviour
{
    [NonNullField]
    public Transform hookPoint;
    [NonNullField]
    public Rope rope;

    private Grabbable currentlyHooked;

    private void Awake() {
    }

    private void FixedUpdate() {
        if (currentlyHooked != null) {
            this.transform.position = currentlyHooked.gameObject.transform.position; 
        }
    }

    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter2D(Collider2D other)
    {
        // When a Grabbable collides with hook, hook it.
        Grabbable grabbable = other.gameObject.GetComponent<Grabbable>();
        if (grabbable != null)
        {
            grabbable.Grab(this.hookPoint);
            currentlyHooked = grabbable;
            // RopeNode node = rope.GetLastNode();
            // rope.PinNodeTo(node, grabbable.gameObject.transform);
        }
    }

}
