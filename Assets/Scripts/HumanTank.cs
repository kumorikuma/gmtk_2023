using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTank : MonoBehaviour
{
    public int HumanCount = 0;

    private void Awake() {
    }

    private void FixedUpdate() {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Grabbable grabbable = other.gameObject.GetComponent<Grabbable>();
        if (grabbable != null && grabbable.gameObject.tag == "Human") {
            AddHuman();
            GameObject.Destroy(grabbable.gameObject);
        }
    }

    private void AddHuman() {
        HumanCount += 1;
        Debug.Log("Human added");
        GameManager.Instance.HumanDelivered();
    }

}
