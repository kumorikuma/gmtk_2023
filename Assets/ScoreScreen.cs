using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScreen : MonoBehaviour
{
    [NonNullField]
    public TextMeshProUGUI LengthText;
    [NonNullField]
    public TextMeshProUGUI WeightText;
    [NonNullField]
    public TextMeshProUGUI CommentText;
    [NonNullField]
    public TextMeshProUGUI ContinueText;

    private static float bestLength = 0;
    private static float bestWeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(float lengthM, float weightKG, string comment) {
        string lengthNewRecord = lengthM > bestLength ? " (NEW RECORD!)" : "";
        string weightNewRecord = weightKG > bestWeight ? " (NEW RECORD!)" : "";
        LengthText.text = $"LENGTH: {lengthM.ToString("0.00")} M{lengthNewRecord}";
        WeightText.text = $"WEIGHT: {weightKG.ToString("0.00")} KG{weightNewRecord}";
        CommentText.text = comment;

        bestLength = Mathf.Max(lengthM, bestLength);
        bestWeight = Mathf.Max(weightKG, bestWeight);
    }

}
