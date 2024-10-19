using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Transform targetCenter;

    // ���� ���� ����
    private float ring1Radius = 0.05f;  // 10�� ����
    private float ring2Radius = 0.15f;  // 8�� ����
    private float ring3Radius = 0.3f;  // 5�� ����
    private float ring4Radius = 0.5f;  // 2�� ����

    private void OnCollisionEnter(Collision collision)
    {
        // ȭ���� �浹 ����
        Vector3 hitPoint = new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
        Vector3 centerPoint = new Vector3(targetCenter.position.x, targetCenter.position.y, 0);
        // ���� �߽ɰ� ȭ���� ���� ���� ���� �Ÿ�
        float distanceFromCenter = Vector3.Distance(hitPoint, centerPoint);

        // ���� ���
        int score = CalculateScore(distanceFromCenter);
        Debug.Log($"Hit Point: {hitPoint}, Score: {score}");

        Debug.Log($"�浹ü�� {collision.transform.name}");
        Debug.Log($"�浹ü�� �߽ɰ��� �Ÿ��� {distanceFromCenter}");


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
}
