using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GameMode {
    idle,
    playing,
    levelEnd,
    gameOver
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header("Inscribed")]
    public TMP_Text uitLevel;
    public TMP_Text uitShots;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("UI")]
    public GameObject gameOverPanel; 
    public TMP_Text gameOverText;
    public Button restartButton;

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";

    void Start()
    {
        S = this;

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        StartLevel();    
    }

    void Update()
    {
        UpdateGUI();
    }

    void StartLevel() {
        if (castle != null) {
            Destroy(castle);
        }

        Projectile.DESTROY_PROJECTILES();

        castle = Instantiate(castles[level]);
        castle.transform.position = castlePos;

        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        FollowCam.SWITCH_VIEW( eView.both );

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void UpdateGUI() {
        if (uitLevel != null)
            uitLevel.text = "Level: " + (level + 1);

        if (uitShots != null)
            uitShots.text = "Shots: " + shotsTaken;

        if ((mode == GameMode.playing) && Goal.goalMet) {
            mode = GameMode.levelEnd;
            FollowCam.SWITCH_VIEW(eView.both);
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel() {
        level++;

        if (level == levelMax) {
            mode = GameMode.gameOver;
            ShowGameOver();
            return;
        }

        StartLevel();
    }

    void ShowGameOver() {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverText != null)
            gameOverText.text = "Congratulations! You beat all castles!\nTotal shots: " + shotsTaken;

        FollowCam.SWITCH_VIEW(eView.none);
    }

    public void RestartGame() {
        level = 0;
        shotsTaken = 0;
        StartLevel();
    }

    static public void SHOT_FIRED() {
        S.shotsTaken++;
    }

    static public GameObject GET_CASTLE() {
        return S.castle;
    }
}