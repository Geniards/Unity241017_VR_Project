using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshPro[] playerScoreTexts;
    [SerializeField] private int totalRounds = 5;

    public enum GameMode { FreeMode, SingleMode, GAMEMODE_MAX }
    public GameMode currentMode;

    private int currentRound = 0;
    private int totalScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetScoreUI();
    }

    public void HitProcess(int score)
    {
        if(currentMode == GameMode.SingleMode && currentRound < totalRounds)
        {
            playerScoreTexts[currentRound].text = score.ToString();
            totalScore += score;
            playerScoreTexts[totalRounds].text = totalScore.ToString();

            currentRound++;
        }

        if (currentMode == GameMode.SingleMode && currentRound >= totalRounds)
        {
            EndGame();
        }


    }

    private void EndGame()
    {
        Debug.Log("게임 종료! 총 점수: " + totalScore);
    }

    private void ResetScoreUI()
    {
        totalScore = 0;
        currentRound = 0;
        playerScoreTexts[totalRounds].text = "0";

        foreach (var score in playerScoreTexts)
        {
            score.text = "-";
        }
    }

    public void ResetGame()
    {
        ResetScoreUI();
    }

    public void SetGameMode(GameMode mode)
    {
        Debug.Log($"게임모드 해제: " + currentMode);
        currentMode = mode;
        Debug.Log("게임 모드 설정: " + mode);
    }
}
