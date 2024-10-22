using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Target Center Transform")]
    [SerializeField] private Transform targetCenter;

    // ����Ʈ ������� (������ ���� ����Ʈ)
    [Header("Effect Prefabs")]
    [SerializeField] private GameObject impactEffect10;
    [SerializeField] private GameObject impactEffect8;
    [SerializeField] private GameObject impactEffect5;
    [SerializeField] private GameObject impactEffect2;

    // ���� ���� ����
    private float ring1Radius = 0.05f;  // 10�� ����
    private float ring2Radius = 0.15f;  // 8�� ����
    private float ring3Radius = 0.3f;  // 5�� ����
    private float ring4Radius = 0.5f;  // 2�� ����

    private void OnCollisionEnter(Collision collision)
    {
        // ȭ���� �浹 ����
        Vector3 hitPoint = collision.GetContact(0).point;
        Vector3 centerPoint = new Vector3(targetCenter.position.x, targetCenter.position.y, collision.GetContact(0).point.z);
        // ���� �߽ɰ� ȭ���� ���� ���� ���� �Ÿ�
        float distanceFromCenter = Vector3.Distance(hitPoint, centerPoint);

        // ���� ���
        int score = CalculateScore(distanceFromCenter);

        PlayImpactEffect(score, hitPoint);

        GameManager.Instance.HitProcess(score, hitPoint);
    }

    private int CalculateScore(float distance)
    {
        // �Ÿ��� ���� ���� ���
        if (distance <= ring1Radius)
        {
            return 10;  // �߾� (10��)
        }
        else if (distance <= ring2Radius)
        {
            return 8;   // 8�� ��
        }
        else if (distance <= ring3Radius)
        {
            return 5;   // 5�� ��
        }
        else if (distance <= ring4Radius)
        {
            return 2;   // 2�� ��
        }
        else
        {
            return 0;   // ������ ���
        }
    }

    private void PlayImpactEffect(int score, Vector3 hitPoint)
    {
        GameObject effectPrefab = null;

        if (score == 10)
        {
            effectPrefab = impactEffect10;
        }
        else if (score == 8)
        {
            effectPrefab = impactEffect8;
        }
        else if (score == 5)
        {
            effectPrefab = impactEffect5;
        }
        else if (score == 2)
        {
            effectPrefab = impactEffect2;
        }

        if (effectPrefab != null)
        {
            // ī�޶��� ������ �����ؼ� ����Ʈ ȸ��
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                Vector3 directionToCamera = (mainCamera.transform.position - hitPoint).normalized;

                Quaternion effectRotation = Quaternion.LookRotation(directionToCamera);

                Instantiate(effectPrefab, hitPoint, effectRotation);
            }
        }
    }
}
