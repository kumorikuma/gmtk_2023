using System.Collections.Generic;
using UnityEngine;

/*Simple player movement controller, based on character controller component,
with footstep system based on check the current texture of the component*/
public class PlayerController : MonoBehaviour {
    [NonNullField]
    public Animator Animator;

    [Header("Movement")]
    [SerializeField]
    private float HorizontalSpeed = 3.0f;
    [SerializeField]
    private float VerticalSpeed = 3.0f;

    //Private movement variables
    private Vector2 inputMoveVector;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(Vector2 moveVector) {
        inputMoveVector = moveVector;
    }

    public void OnJump() {
    }

    public void OnWalk(bool isWalking) {
    }

    private void Update() {
    }

    private void FixedUpdate() {
        Movement();
    }

    private void Movement()
    {
        float yMove = inputMoveVector.y * VerticalSpeed;
        float xMove = inputMoveVector.x * HorizontalSpeed;
        rb.velocity = new Vector3(xMove, yMove, 0);
    }
}