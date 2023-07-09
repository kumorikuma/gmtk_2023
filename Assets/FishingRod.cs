using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kotorman.Rope;

public class FishingRod : MonoBehaviour {
    [NonNullField]
    public GameObject HookObject = null;
    [NonNullField]
    public Rope RopeScript = null;

    public float HookWeight = 10.0f;
    private bool isHookPinned = false;

    public void PinHook() {
        RopeScript.PinNodeTo(RopeScript.GetLastNode(), HookObject.transform);
        isHookPinned = true;
    }

    public void UnpinHook() {
        RopeScript.UnpinNode(RopeScript.GetLastNode());
        isHookPinned = false;
    }

    public void DisableHook() {
        HookObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    void FixedUpdate() {
        RopeNode hookNode = RopeScript.GetLastNode();
        hookNode.Weight = HookWeight;

        if (!isHookPinned) {
            HookObject.transform.position = hookNode.transform.position;
            HookObject.transform.rotation = hookNode.transform.rotation;
        }
    }
}
