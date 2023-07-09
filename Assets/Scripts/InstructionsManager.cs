using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionsManager : Singleton<InstructionsManager>
{
    [NonNullField]
    public TextMeshProUGUI TitleText;
    [NonNullField]
    public TextMeshProUGUI StartText;
    [NonNullField]
    public TextMeshProUGUI MoveText;
    [NonNullField]
    public TextMeshProUGUI GrabText;
    [NonNullField]
    public TextMeshProUGUI PullText;
    [NonNullField]
    public TextMeshProUGUI CollectHumanText;
    [NonNullField]
    public TextMeshProUGUI MoveDownText;
    [NonNullField]
    public TextMeshProUGUI DeliverHereText;

    private float fadeTime = 1;
    public List<TextMeshProUGUI> currentlyShown = new List<TextMeshProUGUI>();

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void HideCurrent() {
        foreach (TextMeshProUGUI text in currentlyShown) {
            StartCoroutine(FadeOutText(fadeTime, text));
        }
        currentlyShown.Clear();
    }

    public void ShowTitle() {
        ShowText(TitleText);
        ShowText(StartText);
    }

    public void ShowMove() {
        ShowText(MoveText, true);
    }

    public void ShowGrab() {
        ShowText(GrabText, true);
    }

    public void ShowPull() {
        ShowText(PullText, true);
    }

    public void ShowCollectHuman() {
        ShowText(CollectHumanText);
    }

    public void ShowMoveDown() {
        ShowText(MoveDownText);
    }

    public void ShowDeliverHere() {
        if (DeliverHereText == null) {
            return;
        }
        ShowText(DeliverHereText);
    }

    private void ShowText(TextMeshProUGUI text, bool fade = false) {
        text.gameObject.SetActive(true);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        currentlyShown.Add(text);
        if (fade) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            StartCoroutine(FadeInText(fadeTime, text));
        }
    }

    private IEnumerator FadeInText(float timeSpeed, TextMeshProUGUI text)
    {
        // text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }

    private IEnumerator FadeOutText(float timeSpeed, TextMeshProUGUI text)
    {
        // text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }

}
