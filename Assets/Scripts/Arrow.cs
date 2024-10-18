using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float moveDistance;
    private Rigidbody rb;
    private bool isFired = false;
    private TrailRenderer trailRenderer;

    // 화살이 충돌시 감속변수
    private bool isColliding = false;
    //private float stopTime = 0.5f;
    //private float stopTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }

    public void Fire(Vector3 direction, float power)
    {
        if (isFired) return;

        rb.isKinematic = false;
        rb.useGravity = true;
        float speed = Mathf.Lerp(5f, 25f, Mathf.Abs(power));
        
        // 방향에 따라 속도 설정 (Sign으로 방향설정.)
        rb.velocity = direction * speed * Mathf.Sign(power);

        isFired = true;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }

        Destroy(gameObject, 5f);
    }

    private void FixedUpdate()
    {
        if (isFired)
        {
            // 화살이 날아갈 때 화살촉이 아래로 향하도록 회전 조정
            Vector3 velocity = rb.velocity;
            if (velocity.magnitude > 0.1f)
            {
                // 현재 속도의 방향을 따라 회전
                Quaternion targetRotation = Quaternion.LookRotation(velocity);
                // 자연스럽게 회전
                rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 2f));
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision발생 {collision.transform.name}");
        Debug.Log($" rb.velocity {rb.velocity.magnitude}");
        Debug.Log($" collision.relativeVelocity {collision.relativeVelocity.magnitude}");

        // 총돌 직전의 반대방향의 화살 속도.
        Vector3 oringinVelocity = -collision.relativeVelocity;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        // 화살이 날아가야할 방향으로.
        transform.forward = oringinVelocity;
        transform.parent = collision.transform;


        if (!isColliding)
        {
            // 충돌 후 조금 더 이동하도록 설정
            isColliding = true;
            //stopTimer = 0f;
            StartCoroutine(SlowDownAndStop(oringinVelocity));
        }
    }

    private IEnumerator SlowDownAndStop(Vector3 oringinVelocity)
    {
        //Vector3 originalVelocity = rb.velocity;

        // 서서히 속도를 줄이면서 화살을 멈춤
        //while (stopTimer < stopTime)
        //{
        //    stopTimer += Time.deltaTime;
        //    float lerpFactor = stopTimer / stopTime;

        //    // 속도를 서서히 감소시킴
        //    transform.Translate(Vector3.forward * moveDistance, Space.Self);
        //    //Vector3.Lerp(oringinVelocity, Vector3.zero, lerpFactor);
        //    yield return null;

        //}


        yield return null;
        // 속도를 서서히 감소시킴
        transform.Translate(Vector3.forward * moveDistance, Space.Self);

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }
}
