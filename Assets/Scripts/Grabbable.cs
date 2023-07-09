using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    // What's holding this
    private Transform holder;
    private Rigidbody2D rb;
    private Collider2D collider;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (holder != null) {
            this.transform.position = holder.transform.position;
        }
    }

    public void Grab(Transform grabber) {
        holder = grabber;
        collider.enabled = false;
    }

    public void Release() {
        holder = null;
        collider.enabled = true;
    }
}
