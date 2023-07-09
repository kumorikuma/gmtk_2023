using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTank : Singleton<HumanTank>
{
    [NonNullField]
    public GameObject OldManObj;
    [NonNullField]
    public GameObject TVGuyObj;
    [NonNullField]
    public GameObject NewGuyObj;

    public int HumanCount = 0;
    public GameObject humanSpritePrefab;

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

        if (HumanCount < 4) {
            // For the first 3 humans, drop in the middle
            NewGuyObj.SetActive(true);
        } else {
            // Pick a random spawn point
            // TODO: Have a list of spawn points to pick from
            Vector3 offset = new Vector3(Random.Range(0f, 6.5f), Random.Range(0f, 0.2f), 0f);
            GameObject go = Instantiate(humanSpritePrefab, this.transform.position + offset, Quaternion.identity);
            go.transform.parent = this.transform;
        }
    }

}
