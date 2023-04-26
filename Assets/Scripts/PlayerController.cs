using System.Collections.Generic;
using UnityEngine;

/*Simple player movement controller, based on character controller component,
with footstep system based on check the current texture of the component*/
public class PlayerController : MonoBehaviour {
    [NonNullField]
    public Animator Animator;

    //Variables for footstep system list
    [System.Serializable]
    public class GroundLayer {
        public string layerName;
        public Texture2D[] groundTextures;
        public AudioClip[] footstepSounds;
    }

    [Header("Movement")]
    [Tooltip("Walking controller speed")]
    [SerializeField]
    private float WalkSpeed = 1.0f;

    [Tooltip("Normal controller speed")]
    [SerializeField]
    private float RunSpeed = 3.0f;

    [Tooltip("Turning controller speed")]
    [SerializeField]
    private float TurnSpeed = 360.0f;

    [Tooltip("Force of the jump with which the controller rushes upwards")]
    [SerializeField]
    private float JumpForce = 1.0f;

    [Tooltip("Gravity, pushing down controller when it jumping")]
    [SerializeField]
    private float gravity = -9.81f;

    public GameObject PlayerModel;

    //Private movement variables
    private Vector3 inputMoveVector;
    private bool inputJumpOnNextFrame = false;
    private Vector3 _velocity; // Used for handling jumping
    private CharacterController characterController;
    private bool isWalkKeyHeld = false;
    Quaternion targetRotation;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _velocity.y = -2f;
    }

    public void OnMove(Vector2 moveVector) {
        inputMoveVector = new Vector3(moveVector.x, 0, moveVector.y);
    }

    public void OnJump() {
        inputJumpOnNextFrame = true;
    }

    // Using KBM controls, there's a specific button to walk.
    public void OnWalk(bool isWalking) {
        isWalkKeyHeld = isWalking;
    }

    private void Update() {
        Movement();
    }

    //Character controller movement
    private void Movement() {
        if (characterController.isGrounded && _velocity.y < 0) {
            // IsGrounded check becomes false if velocity is set to 0.
            // See: https://forum.unity.com/threads/charactercontroller-isgrounded-unreliable-or-bad-code.373492/
            _velocity.y = -.1f;
        }

        if (inputJumpOnNextFrame && characterController.isGrounded) {
            Animator.SetTrigger("Jump");
            _velocity.y = Mathf.Sqrt(JumpForce * -2f * gravity);
        }
        inputJumpOnNextFrame = false;

        // Using KBM controls, there's a specific button to walk.
        // Otherwise, use the amount that user is pushing stick.
        bool isWalking = isWalkKeyHeld || inputMoveVector.magnitude < 0.5f;
        float moveSpeed = isWalking ? WalkSpeed : RunSpeed;
        // Character should move in the direction of the camera
        Vector3 worldMoveDirection = PlayerManager.Instance.CameraController.Pivot.transform.rotation * inputMoveVector;
        // Y component should be 0
        worldMoveDirection.y = 0;
        worldMoveDirection = worldMoveDirection.normalized;
        Vector3 absoluteMoveVector = worldMoveDirection * moveSpeed * Time.deltaTime;
        bool isMoving = absoluteMoveVector.magnitude > 0;
        float forwardSpeed = isMoving ? moveSpeed : 0;
        Animator.SetBool("IsGrounded", characterController.isGrounded); // Set before character is moved
        characterController.Move(absoluteMoveVector);
        if (isMoving) {
            // Face the character in the direction of movement
            targetRotation = Quaternion.Euler(0, Mathf.Atan2(worldMoveDirection.x, worldMoveDirection.z) * Mathf.Rad2Deg, 0);
            // Adjust speed according to how fast character is moving
            Animator.SetBool("IsWalking", isWalking);
            Animator.SetBool("IsRunning", !isWalking);
            // Note: To fix sliding, animation speed needs to be adjusted based on character speed.
            // Running animation speed should be 30% of character speed.
        } else {
            Animator.SetBool("IsWalking", false);
            Animator.SetBool("IsRunning", false);
        }

        // Turn the player incrementally towards the direction of movement
        PlayerModel.transform.rotation = Quaternion.RotateTowards(PlayerModel.transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);

        Animator.applyRootMotion = false;

        // Handle jumping
        Animator.SetFloat("VerticalVelocity", _velocity.y);
        characterController.Move(_velocity * Time.deltaTime);
        _velocity.y += gravity * Time.deltaTime;
    }
}