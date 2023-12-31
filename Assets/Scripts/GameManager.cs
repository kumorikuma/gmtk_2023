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
    public Minigame minigame;

    [NonNullField]
    public GameObject BoatPrefab;

    private GameState currentState;

    private bool tutorialComplete = false;
    private bool boatSpawned = false;
    private int boatsSpawned = 0;

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
            CameraController.Instance.AllowGoingDown = false;
            playerController.AllowGoingDown = false;
            // Human Tank stuff
            switch (HumanTank.Instance.HumanCount) {
                case 1:
                    HumanTank.Instance.NewGuyObj.SetActive(false);
                    HumanTank.Instance.OldManObj.SetActive(true);
                    break;
                case 2:
                    HumanTank.Instance.NewGuyObj.SetActive(false);
                    HumanTank.Instance.TVGuyObj.SetActive(true);
                    break;
                default:
                    break;
            }
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

        int boatBehavior;
        int numBehaviors = 3;
        if (boatsSpawned < numBehaviors) {
            boatBehavior = boatsSpawned;
        } else {
            boatBehavior = UnityEngine.Random.Range(0, numBehaviors);
        }

        switch (boatBehavior) {
            case 0:
                minigame.fishermanBarSize = 2.0f;
                minigame.followDelay = 0.325f;
                minigame.FishermanStaminaDecay = 0.2f;
                minigame.FishermanStaminaRecovery = 0.2f;
                break;
            case 1:
                minigame.fishermanBarSize = 0.7f;
                minigame.followDelay = 0.01f;
                minigame.FishermanStaminaDecay = 0.2f;
                minigame.FishermanStaminaRecovery = 0.5f;
                break;
            case 2:
                minigame.fishermanBarSize = 3.0f;
                minigame.followDelay = 0.45f;
                minigame.FishermanStaminaDecay = 0.2f;
                minigame.FishermanStaminaRecovery = 0.2f;
                break;
        }

        boatsSpawned += 1;
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
                playerController.AllowGoingDown = true;
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
            InstructionsManager.Instance.ShowDeliverHere();
            break;
            case GameState.StatScreen:
            InstructionsManager.Instance.HideCurrent();
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
