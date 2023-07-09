using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSprite : MonoBehaviour {
    public void EnablePhysics() {
        GetComponent<Animator>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<PolygonCollider2D>().enabled = true;
    }
}
