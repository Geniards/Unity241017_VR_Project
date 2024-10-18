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
        
        // 방향에 따라 속도 설정 (Sign으로 방향설정.)
        rb.velocity = direction * speed * Mathf.Sign(power);

        isFired = true;

        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision발생 {collision.transform.name}");

        // 충돌 시 화살이 더 이상 움직이지 않도록 설정
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
