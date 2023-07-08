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

    void Start() {
        // RopeScript.PinNodeTo(RopeScript.GetLastNode(), HookObject.transform);
    }

    void FixedUpdate() {
        // TODO: This is really hacky and the hook logic should be added as physics onto the Rope but generalizing it is a lot of work.
        RopeNode hookNode = RopeScript.GetLastNode();
        hookNode.Weight = HookWeight;
        // PlayerController player = PlayerManager.Instance.PlayerController;
        // if (player.IsBiting) {
        //     // Hook is pulled taut by player
        //     float stretch = player.rope.GetRopeStretchToEndFrom(player.pinnedNode);
        //     if (stretch >= 1.0f) {
        //         RopeScript.UnpinNode(hookNode);
        //         HookObject.transform.position = hookNode.transform.position;
        //     }
        // } else {
        //     // Hook sinks to gravity, the rope sinks with the hook.
        //     RopeScript.PinNodeTo(hookNode, HookObject.transform);
        //     float ropeStretch = RopeScript.GetRopeStretchToEndFrom(RopeScript.GetFirstNode());
        //     Debug.Log(ropeStretch);
        //     // if (ropeStretch < 1.1f) {
        //     {
        //         // HookObject.transform.position = hookNode.transform.position;
        //         Vector3 velocity = Vector3.zero;
        //         velocity.y = HookGravity * Time.fixedDeltaTime;
        //         // HookObject.transform.position += velocity;
        //         hookNode.transform.position += velocity;
        //     }

        //     {
        //         Vector3 velocity = Vector3.zero;
        //         if (hookNode.transform.position.x < RopeScript.GetFirstNode().transform.position.x) {
        //             velocity.x = 0.2f * Time.fixedDeltaTime;
        //         } else if (hookNode.transform.position.x > RopeScript.GetFirstNode().transform.position.x) {
        //             velocity.x = -0.2f * Time.fixedDeltaTime;
        //         }
        //         hookNode.transform.position += velocity;
        //     }
        // }

        HookObject.transform.position = hookNode.transform.position;
        HookObject.transform.rotation = hookNode.transform.rotation;
    }
}
