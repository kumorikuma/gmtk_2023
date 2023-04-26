using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public float MouseLookSensitivity = 0.5f;
    public float JoystickLookSensitivity = 2.0f;

    // [Gameplay]
    void OnMove(InputValue value) {
        PlayerManager.Instance.PlayerController.OnMove(value.Get<Vector2>());
    }
    void OnJump() {
        PlayerManager.Instance.PlayerController.OnJump();
    }
    void OnWalk(InputValue value) {
        PlayerManager.Instance.PlayerController.OnWalk(value.isPressed);
    }

    void OnJoystickLook(InputValue value) {
        PlayerManager.Instance.CameraController.OnLook(value.Get<Vector2>() * JoystickLookSensitivity);
    }
    void OnMouseLook(InputValue value) {
        // Inputs come in as Delta, like: <0, -1>, <12, 0>
        // Main difference with gamepads is that diagonal movement is difficult to capture,
        // since they can be a series of horizontal/vertical deltas.
        // There can also be a movement with a really large delta, whereas gamepad is capped out.
        PlayerManager.Instance.CameraController.OnLook(value.Get<Vector2>() * MouseLookSensitivity);
    }
    void OnPause() {
        MenuSystem.Instance.PauseGame();
    }

    // [Menu]
    void OnUnpause() {
        MenuSystem.Instance.UnpauseGame();
    }
}