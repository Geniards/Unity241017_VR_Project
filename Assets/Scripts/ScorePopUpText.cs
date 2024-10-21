using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePopUpText : MonoBehaviour
{
    public TextMeshPro textMeshPro; // 연결된 TextMeshPro 컴포넌트
    private float moveSpeed = 1f;
    private float fadeDuration = 1f;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    public void ShowScore(int score, Vector3 hitPosition)
    {
        // 타격된 위치에서 텍스트를 표시합니다.
        transform.position = hitPosition;
        textMeshPro.text = score.ToString() + "점";
        // 투명도 초기화
        textMeshPro.alpha = 1f;
        gameObject.SetActive(true);

        // 점수가 표시되면서 점점 사라지는 코루틴을 시작
        StartCoroutine(FadeAndMoveText());
    }

    private IEnumerator FadeAndMoveText()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // 텍스트의 투명도를 줄임.
            textMeshPro.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 비활성화 후 재사용
        gameObject.SetActive(false);
    }
}
