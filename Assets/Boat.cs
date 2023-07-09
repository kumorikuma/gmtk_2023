using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FishermanAction {
    Idle,
    Pull,
    Confused,
    Fall,
}

public class Boat : MonoBehaviour {
    [NonNullField]
    public GameObject BoatSprite;
    [NonNullField]
    public GameObject FishermanIdle;
    [NonNullField]
    public GameObject FishermanPull;
    [NonNullField]
    public GameObject FishermanConfused;
    [NonNullField]
    public GameObject FishermanFall;
    [NonNullField]
    public GameObject RodIdle;
    [NonNullField]
    public GameObject RodPull;
    [NonNullField]
    public GameObject RodFall;

    public FishermanAction CurrentAction = FishermanAction.Idle;

    public void SwitchSprites(FishermanAction action) {
        CurrentAction = action;
        FishermanIdle.SetActive(false);
        FishermanPull.SetActive(false);
        FishermanConfused.SetActive(false);
        FishermanFall.SetActive(false);
        RodIdle.SetActive(false);
        RodPull.SetActive(false);
        RodFall.SetActive(false);

        switch (action) {
            case FishermanAction.Idle:
                FishermanIdle.SetActive(true);
                RodIdle.SetActive(true);
                break;
            case FishermanAction.Pull:
                FishermanPull.SetActive(true);
                RodPull.SetActive(true);
                break;
            case FishermanAction.Confused:
                FishermanConfused.SetActive(true);
                RodIdle.SetActive(true);
                break;
            case FishermanAction.Fall:
                FishermanFall.SetActive(true);
                RodFall.SetActive(true);
                BoatSprite.GetComponent<Animator>().enabled = true;
                break;
        }
    }
}
