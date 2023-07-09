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
                // Prevent the player from grabbing the first node
                if (node == rope.GetFirstNode()) {
                    node = rope.GetNodeAt(1);
                }
                // Only allow the player to grab the rope if they're within a certain distance
                float distance = (node.transform.position - transform.position).magnitude;
                if (distance < 0.5f) {
                    rope.PinNodeTo(node, transform);
                    pinnedNode = node;
                }
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

        float newXVelocity = rb.velocity.x + xAcceleration;
        float newYVelocity = rb.velocity.y + yAcceleration;
        Vector3 newVelocity = new Vector3(newXVelocity, newYVelocity, 0);

        // Movement is constrained by rope
        if (IsBiting && pinnedNode != null) {
            float ropeStretch = rope.GetRopeStretchFromStartTo(pinnedNode);
            bool isRopeStretched = ropeStretch > 1.0f;

            float ropeNormalSize = rope.GetRopeNormalSizeFromStartTo(pinnedNode);
            float ropeMaxStretch = 2.0f;
            float pctStretched = Mathf.Clamp((ropeStretch - 1.0f) / (ropeMaxStretch - 1.0f), 0, 1);
            float ropeMaxSize = ropeMaxStretch * ropeNormalSize;
            Vector2 ropeAnchorPos2d = new Vector2(rope.GetFirstNode().transform.position.x, rope.GetFirstNode().transform.position.y);
            Vector2 ropeAnchorToPlayer = rb.position - ropeAnchorPos2d;
            float distanceToRopeAnchor = ropeAnchorToPlayer.magnitude;

            // Hard stop the distance
            // if (isRopeStretched && distanceToRopeAnchor > ropeMaxSize) {
            //     Vector2 futurePosition = new Vector2(newXVelocity, newYVelocity) * Time.fixedDeltaTime + rb.position;
            //     Vector2 ropeAnchorToFuturePosition = futurePosition - ropeAnchorPos2d;
            //     float futureDistanceToRopeAnchor = ropeAnchorToFuturePosition.magnitude;
            //     if (futureDistanceToRopeAnchor > ropeMaxSize) {
            //         // Adjust the future position
            //         futurePosition = ropeAnchorToFuturePosition.normalized * ropeMaxSize + ropeAnchorPos2d;
            //         // Recompute velocity to get us there
            //         newVelocity = (futurePosition - rb.position) / Time.fixedDeltaTime;
            //     }
            // }

            // Gradually slow down the player and pull them back if the rope is stretched too far
            if (isRopeStretched && distanceToRopeAnchor > ropeNormalSize) {
                // This stretches the max to a maximum of about 2x
                Vector3 maxCounterAcceleration = -ropeAnchorToPlayer.normalized * 1.5f;
                Vector3 counterAcceleration = Vector3.Slerp(Vector3.zero, maxCounterAcceleration, pctStretched);
                newVelocity += counterAcceleration;
                // Debug.Log("ropeStretch: " + ropeStretch);
                // Vector2 futurePosition = new Vector2(newXVelocity, newYVelocity) + rb.position;
                // Vector2 ropeAnchorToFuturePosition = futurePosition - ropeAnchorPos2d;
                // float futureDistanceToRopeAnchor = ropeAnchorToFuturePosition.magnitude;
                // if (inputMoveVector.magnitude < 0) {
                //     Debug.Log("Counter force!");
                //     // Only apply a counter force if the player is not moving
                //     Vector3 maxCounterAcceleration = -ropeAnchorToPlayer.normalized * 1.0f;
                //     Vector3 counterAcceleration = Vector3.Slerp(Vector3.zero, maxCounterAcceleration, pctStretched);
                //     newVelocity += counterAcceleration;
                // } else if (futureDistanceToRopeAnchor > distanceToRopeAnchor) {
                //     // Only apply the slowdown if the player is making the rope more stretched
                //     newVelocity = Vector3.Slerp(Vector3.zero, newVelocity, 1.0f - pctStretched);
                // }
            }
        }

        rb.velocity = newVelocity;
    }

    void LateUpdate() {

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