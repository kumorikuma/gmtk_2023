using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    enum GameState : int
    {
        Title = 0,
        MoveTutorial,
        GrabTutorial,
        PullTutorial,
        Game
    }

    [NonNullField]
    public GameObject TitleCard;

    [NonNullField]
    public PlayerController playerController;

    private GameState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.Title;
        InstructionsManager.Instance.ShowTitle();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AnyKeyPressed()
    {
        // Any key to start game
        if (currentState == GameState.Title) {
            AdvanceGameState();
        }
    }

    public void MoveTutorialComplete()
    {
        if (currentState == GameState.MoveTutorial) {
            AdvanceGameState();
        }
    }

    public void GrabTutorialComplete()
    {
        if (currentState == GameState.GrabTutorial) {
            AdvanceGameState();
        }
    }

    public void PullTutorialComplete()
    {
        if (currentState == GameState.PullTutorial) {
            AdvanceGameState();
        }
    }

    private void AdvanceGameState() {
        currentState += 1;
        Debug.Log("AdvanceGameState");
        switch (currentState) {
            case GameState.Title:
            case GameState.MoveTutorial:
            playerController.gameObject.SetActive(true);
            InstructionsManager.Instance.HideCurrent();
            InstructionsManager.Instance.ShowMove();
            break;
            case GameState.GrabTutorial:
            InstructionsManager.Instance.HideCurrent();
            InstructionsManager.Instance.ShowGrab();
            break;
            case GameState.PullTutorial:
            InstructionsManager.Instance.HideCurrent();
            InstructionsManager.Instance.ShowPull();
            break;
            case GameState.Game:
            InstructionsManager.Instance.HideCurrent();
            break;
            default:
            Debug.LogError($"Reached state {currentState}");
            break;
        }
    }
}
