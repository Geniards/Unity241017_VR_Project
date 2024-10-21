using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshPro[] playerScoreTexts;
    [SerializeField] private TextMeshPro currentModeText;
    [SerializeField] private int totalRounds = 5;

    public enum GameMode { FreeMode, SingleMode, GAMEMODE_MAX }
    public GameMode currentMode;

    [SerializeField] private GameObject scorePopupPrefab;

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

    public void HitProcess(int score, Vector3 hitPosition)
    {
        // 점수 팝업 텍스트 프리팹을 타격된 위치에 인스턴스화
        GameObject scorePopup = Instantiate(scorePopupPrefab);
        ScorePopUpText scorePopupText = scorePopup.GetComponent<ScorePopUpText>();
        scorePopupText.ShowScore(score, hitPosition);

        // 싱글모드에서 5턴 점수 반영
        if (currentMode == GameMode.SingleMode && currentRound < totalRounds)
        {
            playerScoreTexts[currentRound].text = score.ToString();
            totalScore += score;
            playerScoreTexts[totalRounds].text = totalScore.ToString();

            currentRound++;
        }

        // 5턴 종료시
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
        currentModeText.text = currentMode.ToString();
        Debug.Log("게임 모드 설정: " + mode);
    }
}
