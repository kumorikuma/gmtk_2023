using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    public float Boyancy = 5f;
    public float Gravity = 10f;
    public float RandomMotionX = 1f;
    public float RandomMotionY = 5f;

    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        Movement();
    }

    private void Movement()
    {
        float forceX = Random.Range(-RandomMotionX, RandomMotionX);
        float forceY = Mathf.Sin(Time.time) * RandomMotionY;
        forceY -= Gravity;
        float depth = Mathf.Max(0, -transform.position.y);
        forceY += Boyancy * depth;

        rb.AddForce(new Vector3(forceX, forceY, 0));
    }

}
