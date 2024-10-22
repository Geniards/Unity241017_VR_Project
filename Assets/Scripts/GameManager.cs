using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("���� UI ����")]
    [SerializeField] private TextMeshProUGUI[] playerScoreTexts;
    [SerializeField] private TextMeshProUGUI currentModeText;
    [SerializeField] private int totalRounds = 5;

    public enum GameMode { FreeMode, SingleMode, GAMEMODE_MAX }
    public GameMode currentMode;

    [Header("�˾� ���� UI ����")]
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
        // �ٶ��� ������ �����ϰ� ����
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
        currentWindPower.text = "�ٶ��� ���� : " + windStrength.ToString();
    }

    public void PopUpScore(int score, Vector3 hitPosition)
    {
        // ���� �˾� �ؽ�Ʈ �������� Ÿ�ݵ� ��ġ�� �ν��Ͻ�ȭ
        GameObject scorePopup = Instantiate(scorePopupPrefab);
        ScorePopUpText scorePopupText = scorePopup.GetComponent<ScorePopUpText>();
        scorePopupText.ShowScore(score, hitPosition);
    }

    public void HitProcess(int score, Vector3 hitPosition)
    {
        // ���� �˾�
        PopUpScore(score, hitPosition);

        // ȭ���� �浹 �� �ٶ��� ���⵵ ����.
        UpdateWindArrow();

        // �̱۸�忡�� 5�� ���� �ݿ�
        if (currentMode == GameMode.SingleMode && currentRound < totalRounds)
        {
            playerScoreTexts[currentRound].text = score.ToString();
            totalScore += score;
            playerScoreTexts[totalRounds].text = totalScore.ToString();

            currentRound++;
        }

        // 5�� �����
        if (currentMode == GameMode.SingleMode && currentRound >= totalRounds)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("���� ����! �� ����: " + totalScore);
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
        Debug.Log($"���Ӹ�� ����: " + currentMode);
        currentMode = mode;
        currentModeText.text = currentMode.ToString();
        Debug.Log("���� ��� ����: " + mode);
    }
}
