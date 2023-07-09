using System.Collections.Generic;
using UnityEngine;
using Kotorman.Rope;

/*Simple player movement controller, based on character controller component,
with footstep system based on check the current texture of the component*/
public class PlayerController : Singleton<PlayerController> {
    [SerializeField]
    private GameObject Sprite;
    [NonNullField]
    public GrabController GrabPoint;

    [Header("Movement")]
    [SerializeField]
    private float Speed = 1.0f;
    [SerializeField]
    private float DashSpeed = 3.0f;
    [SerializeField]
    private float DashBurstSpeed = 5.0f;
    [SerializeField]
    private float Gravity = 1.0f;

    public bool AllowGoingDown = false;
    public float LowestYAllowedOnTopScreen = -12;
    public float LowestYAllowedOnBottomScreen = -24;
    public float XBound = 18;

    //Private movement variables
    private Vector2 inputMoveVector;
    private Rigidbody2D rb;
    private bool isDashing;
    private bool dashBurst;
    private bool isBiting;
    public bool IsBiting {
        get => isBiting;
    }
    private bool isHoldingRope;
    Grabbable heldObject = null;



    // Movement animation
    float lastRotationY = 180f;
    float rotationSmooth = 5.0f;
    float tiltAngle = 30.0f;

    // Fishing rod stuff
    [NonNullField]
    public Rope rope = null;
    public RopeNode pinnedNode = null;
    [NonNullField]
    public FishermanAlert fishermanAlert = null;

    [NonNullField]
    public Minigame FishingMinigame = null;
    private bool isInFishingMinigame = false;
    [NonNullField]
    public Boat BoatController = null;

    // Instructions
    bool movedUp = false;
    bool movedDown = false;
    bool movedRight = false;
    bool movedLeft = false;

    // Animate entry
    bool isAnimatingEntry = false;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();

        // Don't want to have Raycast starting from this to hit itself.
        Physics2D.queriesStartInColliders = false;
    }

    public void OnMove(Vector2 moveVector) {
        inputMoveVector = moveVector;
        movedUp = movedUp || moveVector.y > 0;
        movedDown = movedDown || moveVector.y < 0;
        movedRight = movedRight || moveVector.x > 0;
        movedLeft = movedLeft || moveVector.x < 0;
        if (movedUp && movedDown && movedRight && movedLeft) {
            GameManager.Instance.MoveTutorialComplete();
        }
    }

    public void OnDash(bool isPressed) {
        if (isPressed && !isDashing) {
            dashBurst = true;
            GameManager.Instance.PullTutorialComplete();
        }
        isDashing = isPressed;
    }

    private void ResetConfusedToIdle() {
        if (BoatController.CurrentAction == FishermanAction.Confused) {
            BoatController.SwitchSprites(FishermanAction.Idle);
        }
    }

    private void FadeoutRope() {
        rope.Fadeout(5.0f);
    }

    private void EndFishingMinigame(bool success) {
        isInFishingMinigame = false;

        // Hide minigame UI
        fishermanAlert.ShowAlert(false);
        FishingMinigame.ShowMinigame(false);

        if (success) {
            BoatController.SwitchSprites(FishermanAction.Fall);
            isBiting = false;
            // Unpin the rope and let it free fall for a bit, then make it disappear.
            rope.UnpinNode(rope.GetFirstNode());
            Invoke("FadeoutRope", 5.0f);
        } else {
            BoatController.SwitchSprites(FishermanAction.Confused);
            Invoke("ResetConfusedToIdle", 5.0f);
        }
    }

    public void OnBite(bool isPressed) {
        if (!isPressed) {
            return;
        }

        isBiting = !isBiting;

        // Pin the rope to the player
        if (heldObject == null && rope != null) {
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

        // Alert the fisherman
        // TODO: Only do this if the gameplay state permits it
        if (isBiting && pinnedNode != null && !isInFishingMinigame) {
            fishermanAlert.ShowAlert(true);
            FishingMinigame.ShowMinigame(true);
            fishermanAlert.SetAlertness(0);
        } else if (!isBiting) {
            if (isInFishingMinigame) {
                EndFishingMinigame(false);
            } else {
                // Didn't really succeed or fail at the minigame
                fishermanAlert.ShowAlert(false);
                FishingMinigame.ShowMinigame(false);
                BoatController.SwitchSprites(FishermanAction.Idle);
            }
        }

        // If not interacting with rope, try grabbing/releasing item
        if (heldObject == null) {
            heldObject = GrabPoint.TryGrab();
        } else {
            GrabPoint.TryRelease();
            heldObject = null;
        }

        // Tutorial 
        if (pinnedNode != null) {
            GameManager.Instance.GrabTutorialComplete();
        }
        if (heldObject != null && heldObject.gameObject.tag == "Human") {
            GameManager.Instance.HumanGrabbed();
        }

        // Failed to bite something
        if (isBiting && pinnedNode == null) {
            isBiting = false;
        }
    }

    private void Update() {
        if (!isAnimatingEntry) {
            UpdateRotations();
        }
    }

    private void FixedUpdate() {
        if (isAnimatingEntry) {
            AnimateMovement();
            return;
        }
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

            float ropeMaxStretchForMinigame = 3.0f;
            float fishIconPosition = Mathf.Clamp((ropeStretch - 1.0f) / (ropeMaxStretchForMinigame - 1.0f), 0, 1);

            float ropeMaxSize = ropeMaxStretch * ropeNormalSize;
            Vector2 ropeAnchorPos2d = new Vector2(rope.GetFirstNode().transform.position.x, rope.GetFirstNode().transform.position.y);
            Vector2 ropeAnchorToPlayer = rb.position - ropeAnchorPos2d;
            float distanceToRopeAnchor = ropeAnchorToPlayer.magnitude;

            fishermanAlert.SetAlertness(pctStretched);
            FishingMinigame.SetFishPosition(fishIconPosition);

            // Trigger the minigame
            if (pctStretched >= 1.0f && !isInFishingMinigame) {
                isInFishingMinigame = true;

                fishermanAlert.ShowAlert(false);
                FishingMinigame.SetFishermanFollow(true);
                BoatController.SwitchSprites(FishermanAction.Pull);
                // BoatController.SwitchSprites(FishermanAction.Fall); // TODO: Debug

                // Let go of the line at the right time after the fisherman is tired to win!

                // Dashing / pulling more will make the fish icon go higher
                // Letting go will make the fish icon go lower
                // Staying inside will make a red bar go up, if this goes up too high you let go automatically?

                // The second fisherman will be better at the minigame.
                // The third fisherman will be OP at the minigame, and force you to think of another way. Use the boot!

                // Press left/right button repeatedly 
            }

            if (isInFishingMinigame) {
                FishingMinigame.UpdateStaminaBar();
                if (FishingMinigame.FishermanStamina <= 0.0f) {
                    EndFishingMinigame(true);
                }
            }

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
                Vector3 maxCounterAcceleration = -ropeAnchorToPlayer.normalized * 1.0f;
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

        // Prevent going down before beating minigame
        if (!AllowGoingDown && rb.position.y < LowestYAllowedOnTopScreen) {
            newVelocity.y = Mathf.Max(0, newVelocity.y);
        } else if (rb.position.y < LowestYAllowedOnBottomScreen) {
            newVelocity.y = Mathf.Max(0, newVelocity.y);
        }
        // Left/right boundaries
        if (rb.position.x < -XBound) {
            newVelocity.x = Mathf.Max(0, newVelocity.x);
        } else if (rb.position.x > XBound) {
            newVelocity.x = Mathf.Min(0, newVelocity.x);
        }
        rb.velocity = newVelocity;
    }

    private void AnimateMovement() {
        // During entry animation, just move left until a spot
        rb.velocity = rb.velocity + new Vector2(-Speed, 0f);
        if (rb.position.x < 9) {
            isAnimatingEntry = false;
        }
    }

    public void AnimateEntry() {
        // Face left
        Sprite.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        isAnimatingEntry = true;
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