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

    void OnDash(InputValue value) {
        PlayerManager.Instance.PlayerController.OnDash(value.isPressed);
    }

    void OnBite(InputValue value) {
        PlayerManager.Instance.PlayerController.OnBite(value.isPressed);
    }

    void OnAnyKey() {
        GameManager.Instance.AnyKeyPressed();
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