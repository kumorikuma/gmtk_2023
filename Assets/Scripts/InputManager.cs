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
    }
    void OnMouseLook(InputValue value) {
    }
    void OnPause() {
        MenuSystem.Instance.PauseGame();
    }

    // [Menu]
    void OnUnpause() {
        MenuSystem.Instance.UnpauseGame();
    }
}