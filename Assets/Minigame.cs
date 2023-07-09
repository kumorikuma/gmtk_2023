using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour {
    public GameObject FishIcon;
    public GameObject FishermanBar;
    public Transform TopOfBar;

    private bool fishermanFollow = false;
    private float followSpeed = 0.1f;
    private float followDelay = 0.325f; // Roughly human reaction time
    private float fishermanBarSize = 1.0f;

    private struct TimePosition {
        public float time;
        public Vector3 position;

        public TimePosition(float time, Vector3 position) {
            this.time = time;
            this.position = position;
        }
    }

    private Queue<TimePosition> timePositions = new Queue<TimePosition>();

    // Start is called before the first frame update
    void Start() {
        ShowMinigame(false);
    }

    // Update is called once per frame
    void Update() {
        if (!fishermanFollow) {
            return;
        }

        // Oldest items in queue will also be oldest in time. They won't be consumed until they are at least older than the delay.
        timePositions.Enqueue(new TimePosition(Time.time, FishIcon.transform.position));

        // Remove elements from the queue until the front element's time is within the last 0.2 seconds
        TimePosition timePosFromPast = new TimePosition(0, Vector3.zero);
        while ((Time.time - timePositions.Peek().time) >= followDelay) {
            timePosFromPast = timePositions.Dequeue();
        }

        if (timePosFromPast.time == 0) {
            return;
        }

        float targetYPos = Mathf.Clamp(timePosFromPast.position.y, transform.position.y + fishermanBarSize / 2.0f, TopOfBar.transform.position.y - fishermanBarSize / 2.0f);
        Vector3 targetPosition = new Vector3(timePosFromPast.position.x, targetYPos, timePosFromPast.position.z);
        FishermanBar.transform.position = Vector3.Lerp(FishermanBar.transform.position, targetPosition, followSpeed);
    }

    public void ShowMinigame(bool isVisible) {
        this.gameObject.SetActive(isVisible);
        if (!isVisible) {
            SetFishermanFollow(false);
        }
        // Reset the initial position
        FishermanBar.transform.position = Vector3.Lerp(transform.position, TopOfBar.transform.position, 0.6f);
    }

    public void SetFishermanFollow(bool shouldFollow) {
        FishermanBar.SetActive(shouldFollow);
        fishermanFollow = shouldFollow;
    }

    // public void SetFishermanPosition(float t) {
    //     t = Mathf.Clamp(t, 0, 1);
    //     FishermanBar.transform.position = Vector3.Lerp(transform.position, TopOfBar.transform.position, t);
    // }

    public void SetFishPosition(float t) {
        t = Mathf.Clamp(t, 0, 1);
        FishIcon.transform.position = Vector3.Lerp(transform.position, TopOfBar.transform.position, t);
    }
}
