using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePopUpText : MonoBehaviour
{
    public TextMeshPro textMeshPro; // ����� TextMeshPro ������Ʈ
    private float moveSpeed = 1f;
    private float fadeDuration = 1f;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    public void ShowScore(int score, Vector3 hitPosition)
    {
        // Ÿ�ݵ� ��ġ���� �ؽ�Ʈ�� ǥ���մϴ�.
        transform.position = hitPosition;
        textMeshPro.text = score.ToString() + "��";
        // ���� �ʱ�ȭ
        textMeshPro.alpha = 1f;
        gameObject.SetActive(true);

        // ������ ǥ�õǸ鼭 ���� ������� �ڷ�ƾ�� ����
        StartCoroutine(FadeAndMoveText());
    }

    private IEnumerator FadeAndMoveText()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // �ؽ�Ʈ�� ������ ����.
            textMeshPro.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��Ȱ��ȭ �� ����
        gameObject.SetActive(false);
    }
}
