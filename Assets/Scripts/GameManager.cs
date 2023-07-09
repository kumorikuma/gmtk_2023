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
        Game,
        CollectReward,
        MoveDownTutorial,
        DropOffTutorial,
        StatScreen,
        StatScreenContinue
    }

    [NonNullField]
    public GameObject TitleCard;

    [NonNullField]
    public PlayerController playerController;

    [NonNullField]
    public GameObject BoatPrefab;

    private GameState currentState;

    private bool tutorialComplete = false;
    private bool boatSpawned = false;

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
            GotoGameState(GameState.MoveTutorial);
        } else if (currentState == GameState.StatScreenContinue) {
            ScoreScreen.Instance.Hide();
            GotoGameState(GameState.Game);
        }
    }

    public void MoveTutorialComplete()
    {
        if (currentState == GameState.MoveTutorial) {
            GotoGameState(GameState.GrabTutorial);
        }
    }

    public void GrabTutorialComplete()
    {
        if (currentState == GameState.GrabTutorial) {
            GotoGameState(GameState.PullTutorial);
        }
    }

    public void PullTutorialComplete()
    {
        if (currentState == GameState.PullTutorial) {
            GotoGameState(GameState.Game);
        }
    }

    public void GameWon() {
        GotoGameState(GameState.CollectReward);
    }

    public void HumanGrabbed() {
        // Only show rest of tutorial if it's not already complete
        if (!tutorialComplete) {
            GotoGameState(GameState.MoveDownTutorial);
        }
    }

    public void MovedDown() {
        if (currentState == GameState.MoveDownTutorial) {
            GotoGameState(GameState.DropOffTutorial);
        }
    }

    public void MovedUp() {
        if (currentState == GameState.Game) {
            // Spawn a new boat and make sure all the references are hooked up properly.
            SpawnNewBoat();
        }
    }

    public void HumanDelivered() {
        GotoGameState(GameState.StatScreen);
    }

    public void StatScreenReadyToContinue() {
        GotoGameState(GameState.StatScreenContinue);
    } 

    public GameObject SpawnNewBoat() {
        GameObject newBoatObj = Instantiate(BoatPrefab, new Vector3(20, 0.75f, 0), Quaternion.identity);
        Boat boatController = newBoatObj.GetComponent<Boat>();
        PlayerManager.Instance.PlayerController.pinnedNode = null;
        PlayerManager.Instance.PlayerController.BoatController = boatController;
        boatController.StartEntryAnimation();
        return newBoatObj;
    }

    private void GotoGameState(GameState state) {
        Debug.Log($"Go to game state {state}");
        switch (state) {
            case GameState.Title:
            case GameState.MoveTutorial:
            playerController.AnimateEntry();
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
            case GameState.CollectReward:
            InstructionsManager.Instance.HideCurrent();
            InstructionsManager.Instance.ShowCollectHuman();
            if (tutorialComplete) {
                CameraController.Instance.AllowGoingDown = true;
                PlayerController.Instance.AllowGoingDown = true;
            }
            break;
            case GameState.MoveDownTutorial:
            InstructionsManager.Instance.HideCurrent();
            InstructionsManager.Instance.ShowMoveDown();
            CameraController.Instance.AllowGoingDown = true;
            playerController.AllowGoingDown = true;
            break;
            case GameState.DropOffTutorial:
            InstructionsManager.Instance.HideCurrent();
            // InstructionsManager.Instance.();
            break;
            case GameState.StatScreen:
            ScoreScreen.Instance.Show();
            break;
            case GameState.StatScreenContinue:
            // No change, but ready to move on
            break;
            default:
            Debug.LogError($"Reached state {state}");
            break;
        }
        currentState = state;
    }


}
