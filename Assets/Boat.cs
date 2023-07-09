using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FishermanAction {
    Idle,
    Pull,
}

public class Boat : MonoBehaviour {
    [NonNullField]
    public GameObject FishermanIdle;
    [NonNullField]
    public GameObject FishermanPull;
    [NonNullField]
    public GameObject RodIdle;
    [NonNullField]
    public GameObject RodPull;

    public void SwitchSprites(FishermanAction action) {
        FishermanIdle.SetActive(false);
        FishermanPull.SetActive(false);
        RodIdle.SetActive(false);
        RodPull.SetActive(false);

        switch (action) {
            case FishermanAction.Idle:
                FishermanIdle.SetActive(true);
                RodIdle.SetActive(true);
                break;
            case FishermanAction.Pull:
                FishermanPull.SetActive(true);
                RodPull.SetActive(true);
                break;
        }
    }
}
