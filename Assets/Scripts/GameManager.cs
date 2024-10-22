using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("점수 UI 세팅")]
    [SerializeField] private TextMeshProUGUI[] playerScoreTexts;
    [SerializeField] private TextMeshProUGUI currentModeText;
    [SerializeField] private int totalRounds = 5;

    public enum GameMode { FreeMode, SingleMode, GAMEMODE_MAX }
    public GameMode currentMode;

    [Header("팝업 점수 UI 세팅")]
    [SerializeField] private GameObject scorePopupPrefab;

    private int currentRound = 0;
    private int totalScore = 0;

    [Header("Wind Settings")]
    public Vector3 windDirection;
    public float windStrength;
    [SerializeField] private Transform windArrowTransform;
    [SerializeField] private TextMeshProUGUI currentWindPower;

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
        UpdateWindArrow();
    }

    private void SetRandomWind()
    {
        // 바람의 방향을 랜덤하게 설정
        windDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        windStrength = Random.Range(0, 5);
    }

    public void UpdateWindArrow()
    {
        SetRandomWind();
        if (windArrowTransform != null)
        {
            Quaternion windRotation = Quaternion.LookRotation(windDirection);
            windArrowTransform.rotation = Quaternion.Euler(0, windRotation.eulerAngles.y+180, 0);
        }
        currentWindPower.text = "바람의 세기 : " + windStrength.ToString();
    }

    public void PopUpScore(int score, Vector3 hitPosition)
    {
        // 점수 팝업 텍스트 프리팹을 타격된 위치에 인스턴스화
        GameObject scorePopup = Instantiate(scorePopupPrefab);
        ScorePopUpText scorePopupText = scorePopup.GetComponent<ScorePopUpText>();
        scorePopupText.ShowScore(score, hitPosition);
    }

    public void HitProcess(int score, Vector3 hitPosition)
    {
        // 점수 팝업
        PopUpScore(score, hitPosition);

        // 화살이 충돌 후 바람의 방향도 변경.
        UpdateWindArrow();

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
