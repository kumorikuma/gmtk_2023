using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : Singleton<PlayerManager> {
    [NonNullField]
    public PlayerController PlayerController;

    public void SwitchActionMaps(string actionMapName) {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap(actionMapName);
    }
}