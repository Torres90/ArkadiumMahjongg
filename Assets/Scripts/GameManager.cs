using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Events
    public static EventHandler<(Tile, Tile)> PlayerFoundTilePair;
    public static EventHandler<int> TimeLeftChanged;
    public static EventHandler<string> SwitchCanvas;
    public static EventHandler<int> ScoreChanged;
    public static EventHandler<int> MultiplierChanged;
    public static EventHandler TimeOutGameOver;

    //[Header("UI Variables")]
    //[SerializeField] private Canvas gameplayCanvas;
    //[SerializeField] private Canvas gamePausedCanvas;
    //[SerializeField] private Canvas gameOverCanvas;
    //[SerializeField] private Canvas gameWonCanvas;

    [Header("Gameplay Variables")]
    [SerializeField] private int gameDurationInSeconds;
    [Tooltip("Number of seconds until combo multiplier is reset.")]
    [SerializeField] private int multiplierSecondsLimit;

    private int timeLeft;
    private int score;
    private int multiplier;
    private readonly int scoreAwardedPerTilePair = 100;
    private float timeOfLastTileMatch;
    private Tile lastSelectedTile;
    private IEnumerator countdownTimer;

    private void OnEnable()
    {
        //Subscribing to Events
        Board.ClickedOnAFreeTile += OnClickedOnFreeTile;
        Board.BoardCleared += OnBoardCleared;
        Board.BoardInitialized += OnBoardInitialized;
        PauseButton.PauseButtonWasPressed += OnPause;
        ResumeButton.ResumeButtonWasPressed += OnResume;
        RestartGameButton.RestartGameButtonPressed += RestartGame;
    }
    private void OnDisable()
    {
        //Unsubscribing to Events
        Board.ClickedOnAFreeTile -= OnClickedOnFreeTile;
        Board.BoardCleared -= OnBoardCleared;
        Board.BoardInitialized += OnBoardInitialized;
        PauseButton.PauseButtonWasPressed -= OnPause;
        ResumeButton.ResumeButtonWasPressed -= OnResume;
        RestartGameButton.RestartGameButtonPressed -= RestartGame;
    }

    private void OnBoardInitialized(object sender, EventArgs e)
    {
        StartGameTimer();
    }

    private void Start()
    {
        SetupGame();
    }

    private void RestartGame(object sender, EventArgs e) => SetupGame();

    private void SetupGame()
    {
        ////Turning on Main Canvas
        //gameplayCanvas.enabled = true;
        ////Making sure the other ones are off.
        //gameOverCanvas.enabled = false;
        //gamePausedCanvas.enabled = false;
        //gameWonCanvas.enabled = false;
        SwitchCanvas?.Invoke(this, "Canvas - In Game");

        score = 0;
        multiplier = 1;
        ScoreChanged?.Invoke(this, score);
        MultiplierChanged?.Invoke(this, multiplier);

        lastSelectedTile = null;
        timeLeft = gameDurationInSeconds;
        TimeLeftChanged?.Invoke(this, timeLeft);        
    }

    private void StartGameTimer()
    {
        if (countdownTimer != null)
        {
            StopCoroutine(countdownTimer);
        }

        countdownTimer = CountdownTimer();
        StartCoroutine(countdownTimer);
    }

    private void OnResume(object sender, EventArgs e)
    {
        //Resume Timer
        StopCoroutine(countdownTimer);
        countdownTimer = CountdownTimer();
        StartCoroutine(countdownTimer);

        ////Turn on other panels
        //gameplayCanvas.enabled = true;
        ////Turn off Pause canvas
        //gamePausedCanvas.enabled = false;
        SwitchCanvas?.Invoke(this, "Canvas - In Game");

    }
    private void OnPause(object sender, EventArgs e)
    {
        //Stop Timer
        StopCoroutine(countdownTimer);

        ////Turn on Pause Canvas
        //gamePausedCanvas.enabled = true;
        ////Turn off other panels
        //gameplayCanvas.enabled = false;
        SwitchCanvas?.Invoke(this, "Canvas - Pause");

    }
    private void OnBoardCleared(object sender, EventArgs e) => GameOver(victory: true);
    private void GameOver(bool victory)
    {
        //gameplayCanvas.enabled = false;
        SwitchCanvas?.Invoke(this, "none");

        if (victory)
        {
            //gameWonCanvas.enabled = true;
            SwitchCanvas?.Invoke(this, "Canvas - Victory");

        }
        else
        {
            //gameOverCanvas.enabled = true;
            SwitchCanvas?.Invoke(this, "Canvas - GameOver");
        }
    }
    private void OnClickedOnFreeTile(object sender, Tile clickedTile)
    {
        //First tile you click gets stored
        //If you click on a stuck tile, nothing happens. The stored tile doesn't change.
        //If you click on a free tile but of a different type, this new tile gets stored and the last one is forgotten.

        if (lastSelectedTile == null)
        {
            lastSelectedTile = clickedTile;
            return;
        }
        if (clickedTile == lastSelectedTile) //We are clicking on the same tile as last time, so we ignore this
        {
            return;
        }
        if (clickedTile.tileType == lastSelectedTile.tileType) //We have a match
        {
            score += scoreAwardedPerTilePair * GetMultiplier();
            ScoreChanged?.Invoke(this, score);

            PlayerFoundTilePair?.Invoke(this, (lastSelectedTile, clickedTile));

            lastSelectedTile = null;
            return;
        }

        lastSelectedTile = clickedTile;
    }
    private IEnumerator CountdownTimer()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;
            TimeLeftChanged?.Invoke(this, timeLeft);
        }
        TimeOutGameOver?.Invoke(this, EventArgs.Empty);
        GameOver(victory: false);
    }
    private int GetMultiplier()
    {
        if (Time.time < timeOfLastTileMatch + multiplierSecondsLimit)
        {
            multiplier++;
        }
        else
        {
            multiplier = 1;
        }
        timeOfLastTileMatch = Time.time;

        MultiplierChanged?.Invoke(this, multiplier);
        return multiplier;
    }

    //Methods of Testing
    [ContextMenu("Test GameOver Win")]
    void TestGameOverWin() => GameOver(true);
    [ContextMenu("Test GameOver Lose")]
    void TestGameOverLose() => GameOver(false);
}
