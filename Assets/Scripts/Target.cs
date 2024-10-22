using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Target Center Transform")]
    [SerializeField] private Transform targetCenter;

    // 이펙트 프리펩들 (점수에 따른 이펙트)
    [Header("Effect Prefabs")]
    [SerializeField] private GameObject impactEffect10;
    [SerializeField] private GameObject impactEffect8;
    [SerializeField] private GameObject impactEffect5;
    [SerializeField] private GameObject impactEffect2;

    // 점수 구간 설정
    private float ring1Radius = 0.05f;  // 10점 영역
    private float ring2Radius = 0.15f;  // 8점 영역
    private float ring3Radius = 0.3f;  // 5점 영역
    private float ring4Radius = 0.5f;  // 2점 영역

    private void OnCollisionEnter(Collision collision)
    {
        // 화살의 충돌 지점
        Vector3 hitPoint = collision.GetContact(0).point;
        Vector3 centerPoint = new Vector3(targetCenter.position.x, targetCenter.position.y, collision.GetContact(0).point.z);
        // 과녁 중심과 화살이 맞은 지점 간의 거리
        float distanceFromCenter = Vector3.Distance(hitPoint, centerPoint);

        // 점수 계산
        int score = CalculateScore(distanceFromCenter);

        PlayImpactEffect(score, hitPoint);

        GameManager.Instance.HitProcess(score, hitPoint);
    }

    private int CalculateScore(float distance)
    {
        // 거리에 따른 점수 계산
        if (distance <= ring1Radius)
        {
            return 10;  // 중앙 (10점)
        }
        else if (distance <= ring2Radius)
        {
            return 8;   // 8점 링
        }
        else if (distance <= ring3Radius)
        {
            return 5;   // 5점 링
        }
        else if (distance <= ring4Radius)
        {
            return 2;   // 2점 링
        }
        else
        {
            return 0;   // 과녁을 벗어남
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
            // 카메라의 방향을 참조해서 이펙트 회전
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
