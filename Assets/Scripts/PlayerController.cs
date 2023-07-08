using System.Collections.Generic;
using UnityEngine;

/*Simple player movement controller, based on character controller component,
with footstep system based on check the current texture of the component*/
public class PlayerController : MonoBehaviour {
    [NonNullField]
    public Animator Animator;

    [Header("Movement")]
    [SerializeField]
    private float Speed = 1.0f;
    [SerializeField]
    private float DashSpeed = 3.0f;
    [SerializeField]
    private float DashBurstSpeed = 5.0f;



    //Private movement variables
    private Vector2 inputMoveVector;
    private Rigidbody2D rb;
    private bool isDashing;
    private bool dashBurst;
    private bool isBiting;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(Vector2 moveVector) {
        inputMoveVector = moveVector;
    }

    public void OnDash(bool isPressed) {
        if (isPressed && !isDashing) {
            dashBurst = true;
        }
        isDashing = isPressed;
    }

    public void OnBite(bool isPressed) {
        isBiting = isPressed;
    }

    private void Update() {
    }

    private void FixedUpdate() {
        Movement();
    }

    private void Movement()
    {
        float xAcceleration = inputMoveVector.x;
        float yAcceleration = inputMoveVector.y;
        if (isDashing) {
            xAcceleration *= DashSpeed;
            yAcceleration *= DashSpeed;
            if (dashBurst) {
                dashBurst = false;
                xAcceleration *= DashBurstSpeed;
                yAcceleration *= DashBurstSpeed;
            }
        } else {
            xAcceleration *= Speed;
            yAcceleration *= Speed;
        }
        rb.velocity = new Vector3(rb.velocity.x + xAcceleration, rb.velocity.y + yAcceleration, 0);
    }
}