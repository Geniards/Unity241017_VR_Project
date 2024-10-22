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

    private void FixedUpdate()
    {
        if (isFired)
        {
            // 바람의 힘을 적용
            Vector3 windForce = GameManager.Instance.windDirection * GameManager.Instance.windStrength;
            rb.AddForce(windForce * Time.fixedDeltaTime, ForceMode.Force);
        }
    }

    public void Fire(Vector3 direction, float power)
    {
        if (isFired) return;

        rb.isKinematic = false;
        rb.useGravity = false;  // 발사 후 잠시 동안은 중력의 영향을 무시하게함.
        float speed = Mathf.Lerp(2f, 20f, Mathf.Abs(power));
        
        // 방향에 따라 속도 설정 (Sign으로 방향설정.)
        rb.velocity = direction * speed * Mathf.Sign(power);

        rb.drag = 0.05f;
        rb.angularDrag = 0.01f;

        isFired = true;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }

        // 화살이 발사되었을 때 회전 보정 코루틴 실행
        StartCoroutine(AlignArrowRotation());
        // 일정 시간 후 중력 적용
        StartCoroutine(ApplyGravityAfterDelay(0.2f));

        Destroy(gameObject, 5f);

        // 쏘고 화살이 사라지면 바람의 방향도 변경.
        GameManager.Instance.UpdateWindArrow();
    }

    private IEnumerator AlignArrowRotation()
    {
        while (isFired)
        {
            // 화살의 속도 벡터에 기반해 회전 보정
            Vector3 velocity = rb.velocity;
            if (velocity.magnitude > 0.1f)
            {
                // 현재 속도 방향을 따라 화살의 회전 보정
                Quaternion targetRotation = Quaternion.LookRotation(velocity);
                rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 5f)); // 회전 보정 속도 설정
            }

            yield return null;
        }
    }

    private IEnumerator ApplyGravityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.useGravity = true;
        rb.drag = 0.1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision발생 {collision.transform.name}");
        //Debug.Log($" rb.velocity {rb.velocity.magnitude}");
        //Debug.Log($" collision.relativeVelocity {collision.relativeVelocity.magnitude}");

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
            StartCoroutine(SlowDownAndStop(oringinVelocity));
        }

        // 과녁을 제외한 곳에 부딪히면 점수를 0점으로 처리한다.
        if (collision.transform.name != "Target Transform" && GameManager.Instance.currentMode == GameManager.GameMode.SingleMode)
        {
            GameManager.Instance.HitProcess(0, collision.GetContact(0).point);
        }
    }

    private IEnumerator SlowDownAndStop(Vector3 oringinVelocity)
    {
        yield return null;
        // 속도를 서서히 감소시킴
        transform.Translate(Vector3.forward * moveDistance, Space.Self);

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }
}
