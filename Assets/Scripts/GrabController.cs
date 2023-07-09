using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public Transform grabDetect;
    public float rayDist;
    public Grabbable currentlyGrabbed;

    // Update is called once per frame
    void Update() {
    }

    public Grabbable TryGrab() {
        Vector3 facing = transform.rotation * Vector3.right;
        RaycastHit2D hitCheck = Physics2D.Raycast(grabDetect.position, new Vector2(facing.x, facing.y), rayDist);
        Grabbable grabbable = hitCheck.transform ? hitCheck.transform.gameObject.GetComponent<Grabbable>() : null;
        if (grabbable != null)
        {
            grabbable.Grab(this.transform);
            currentlyGrabbed = grabbable;
            return currentlyGrabbed;
        }
        return null;
    }

    public Grabbable TryRelease() {
        if (currentlyGrabbed != null) {
            Grabbable result = currentlyGrabbed;
            currentlyGrabbed.Release();
            currentlyGrabbed = null;
            return result;
        }
        return null;
    }
}
