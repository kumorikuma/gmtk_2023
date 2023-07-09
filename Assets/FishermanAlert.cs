using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishermanAlert : MonoBehaviour {
    [NonNullField] public SpriteRenderer alertRenderer;

    public void ShowAlert(bool isVisible) {
        alertRenderer.enabled = isVisible;
    }

    // t should be a value between 0 and 1
    public void SetAlertness(float t) {
        t = Mathf.Clamp(t, 0, 1);
        Vector3 yellowHsv = new Vector3(58, 50, 100);
        Vector3 redHsv = new Vector3(8, 81, 100);
        Vector3 hsvColor = Vector3.Lerp(yellowHsv, redHsv, t);
        alertRenderer.color = Color.HSVToRGB(hsvColor.x / 360.0f, hsvColor.y / 100.0f, hsvColor.z / 100.0f);
    }
}
