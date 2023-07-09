using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {
    public float speed = -0.1f;
    // Update is called once per frame
    void FixedUpdate() {
        // -18 and +18
        float newXPos = transform.position.x + speed;
        if (newXPos < -20.0f) {
            newXPos = 20.0f;
        }
        transform.position = new Vector3(newXPos, transform.position.y, transform.position.z);
    }
}
