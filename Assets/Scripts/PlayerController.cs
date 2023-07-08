using System.Collections.Generic;
using UnityEngine;
using Kotorman.Rope;

/*Simple player movement controller, based on character controller component,
with footstep system based on check the current texture of the component*/
public class PlayerController : MonoBehaviour {
    [SerializeField]
    private GameObject Sprite;

    [Header("Movement")]
    [SerializeField]
    private float Speed = 1.0f;
    [SerializeField]
    private float DashSpeed = 3.0f;
    [SerializeField]
    private float DashBurstSpeed = 5.0f;
    [SerializeField]
    private float Gravity = 1.0f;

    //Private movement variables
    private Vector2 inputMoveVector;
    private Rigidbody2D rb;
    private bool isDashing;
    private bool dashBurst;
    private bool isBiting;
    public bool IsBiting {
        get => isBiting;
    }

    // Movement animation
    float lastRotationY = 0f;
    float rotationSmooth = 5.0f;
    float tiltAngle = 30.0f;

    // Fishing rod stuff
    [NonNullField]
    public Rope rope = null;
    public RopeNode pinnedNode = null;

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

        // Pin the rope to the player
        if (rope != null) {
            if (pinnedNode == null) {
                RopeNode node = rope.GetClosestNode(transform.position);
                rope.PinNodeTo(node, transform);
                pinnedNode = node;
            } else {
                rope.UnpinNode(pinnedNode);
                pinnedNode = null;
            }
        }
    }

    private void Update() {
        UpdateRotations();
    }

    private void FixedUpdate() {
        Movement();

        // If rope is stretched too far, trigger event
        if (IsBiting) {
            // float ropeStretch = rope.GetRopeStretchToEndFrom()
        }
    }

    private void Movement()
    {
        float xAcceleration = inputMoveVector.x;
        float yAcceleration = inputMoveVector.y;
        if (rb.position.y > 0) {
            // Fish out of the water
            yAcceleration = -Gravity;
        } else if (isDashing) {
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

    private void UpdateRotations() {
        float rotationY = lastRotationY;
        float rotationZ = 0f;
        // Flip the player
        if (inputMoveVector.x > 0.01) {
            rotationY = 0f;
        } else if (inputMoveVector.x < -0.01) {
            rotationY = 180f;
        }
        // Keep the same facing if no button is pressed
        lastRotationY = rotationY;

        if (inputMoveVector.y > 0.01) {
            rotationZ = tiltAngle;
        } else if (inputMoveVector.y < -0.01) {
            rotationZ = -tiltAngle;
        }
        Quaternion target = Quaternion.Euler(0f, rotationY, rotationZ);
        Sprite.transform.rotation = Quaternion.Slerp(Sprite.transform.rotation, target,  Time.deltaTime * rotationSmooth);
    }
}