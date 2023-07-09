using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [NonNullField]
    public GameObject TitleCard;

    [NonNullField]
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        playerController.gameObject.SetActive(true);
    }

    private void ShowTitle() {
        TitleCard.SetActive(false);
    }
}
