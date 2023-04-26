using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DialogueData {
    public string speaker;
    public string text;
}

public class DialogueSystem : MonoBehaviour {
    public event EventHandler<DialogueData> OnDialogueUpdate;

    // Start is called before the first frame update
    void Start() {
        DialogueData data = new DialogueData();
        data.speaker = "Farris";
        data.text = "Why did the tomato turn red? ... Because it saw the salad dressing!";
        OnDialogueUpdate(this, data);
    }
}