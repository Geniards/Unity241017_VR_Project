using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool isFired = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void Fire(Vector3 direction, float power)
    {
        if (isFired) return;

        rb.isKinematic = false;
        float speed = Mathf.Lerp(5f, 40f, Mathf.Abs(power));
        
        // ���⿡ ���� �ӵ� ���� (Sign���� ���⼳��.)
        rb.velocity = direction * speed * Mathf.Sign(power);

        isFired = true;

        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision�߻� {collision.transform.name}");

        // �浹 �� ȭ���� �� �̻� �������� �ʵ��� ����
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
