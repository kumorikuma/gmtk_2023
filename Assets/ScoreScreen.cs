using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScreen : Singleton<ScoreScreen>
{
    [NonNullField]
    public TextMeshProUGUI LengthText;
    [NonNullField]
    public TextMeshProUGUI WeightText;
    [NonNullField]
    public TextMeshProUGUI CommentText;
    [NonNullField]
    public TextMeshProUGUI ContinueText;
    [NonNullField]
    public GameObject Panel;

    private static float bestLength = 0;
    private static float bestWeight = 0;
    private static int commentNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show() {
        float lengthM = Random.Range(1.5f, 2.0f);
        float weightKG = Random.Range(140f, 250f);
        string comment = GenerateComment();
        
        string lengthNewRecord = lengthM > bestLength ? " (NEW RECORD!)" : "";
        string weightNewRecord = weightKG > bestWeight ? " (NEW RECORD!)" : "";
        LengthText.text = $"LENGTH: {lengthM.ToString("0.00")} M{lengthNewRecord}";
        WeightText.text = $"WEIGHT: {weightKG.ToString("0.00")} KG{weightNewRecord}";
        CommentText.text = comment;

        bestLength = Mathf.Max(lengthM, bestLength);
        bestWeight = Mathf.Max(weightKG, bestWeight);
        Panel.SetActive(true);
        Invoke("ShowContinue", 2);
    }

    public void Hide() {
        Panel.SetActive(false);
        ContinueText.gameObject.SetActive(false);
    }

    private void ShowContinue() {
        ContinueText.gameObject.SetActive(true);
        GameManager.Instance.StatScreenReadyToContinue();
    }

    private string GenerateComment() {
        commentNum += 1;
        switch (commentNum) {
            case 1:
            return "One's too lonely. I must collect more to keep it company.";
            case 2:
            return "Another amateur fisherman. Let's get a few more.";
            case 3:
            return "This one lost its land-walking device.";
            case 4:
            return "A strong one. Needed an extra kick.";
            case 5:
            return "I wonder how they breath out of the water.";
            case 6:
            return "Why do they still keep coming here?";
            default:
            return "Just a common fisherman. Nothing special.";
        }
    }
}
