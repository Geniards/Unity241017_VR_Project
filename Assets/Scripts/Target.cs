using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Transform targetCenter;

    // 점수 구간 설정
    private float ring1Radius = 0.05f;  // 10점 영역
    private float ring2Radius = 0.15f;  // 8점 영역
    private float ring3Radius = 0.3f;  // 5점 영역
    private float ring4Radius = 0.5f;  // 2점 영역

    private void OnCollisionEnter(Collision collision)
    {
        // 화살의 충돌 지점
        Vector3 hitPoint = new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
        Vector3 centerPoint = new Vector3(targetCenter.position.x, targetCenter.position.y, 0);
        // 과녁 중심과 화살이 맞은 지점 간의 거리
        float distanceFromCenter = Vector3.Distance(hitPoint, centerPoint);

        // 점수 계산
        int score = CalculateScore(distanceFromCenter);
        Debug.Log($"Hit Point: {hitPoint}, Score: {score}");

        Debug.Log($"충돌체는 {collision.transform.name}");
        Debug.Log($"충돌체와 중심과의 거리는 {distanceFromCenter}");


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
}
