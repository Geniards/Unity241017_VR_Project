using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float moveDistance;
    private Rigidbody rb;
    private bool isFired = false;
    private TrailRenderer trailRenderer;

    // ȭ���� �浹�� ���Ӻ���
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

    public void Fire(Vector3 direction, float power)
    {
        if (isFired) return;

        rb.isKinematic = false;
        rb.useGravity = true;
        float speed = Mathf.Lerp(5f, 25f, Mathf.Abs(power));
        
        // ���⿡ ���� �ӵ� ���� (Sign���� ���⼳��.)
        rb.velocity = direction * speed * Mathf.Sign(power);

        isFired = true;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }

        // ȭ���� �߻�Ǿ��� �� ȸ�� ���� �ڷ�ƾ ����
        StartCoroutine(AlignArrowRotation());

        Destroy(gameObject, 5f);
    }

    private IEnumerator AlignArrowRotation()
    {
        while (isFired)
        {
            // ȭ���� �ӵ� ���Ϳ� ����� ȸ�� ����
            Vector3 velocity = rb.velocity;
            if (velocity.magnitude > 0.1f)
            {
                // ���� �ӵ� ������ ���� ȭ���� ȸ�� ����
                Quaternion targetRotation = Quaternion.LookRotation(velocity);
                rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 5f)); // ȸ�� ���� �ӵ� ����
            }

            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision�߻� {collision.transform.name}");
        Debug.Log($" rb.velocity {rb.velocity.magnitude}");
        Debug.Log($" collision.relativeVelocity {collision.relativeVelocity.magnitude}");

        // �ѵ� ������ �ݴ������ ȭ�� �ӵ�.
        Vector3 oringinVelocity = -collision.relativeVelocity;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        // ȭ���� ���ư����� ��������.
        transform.forward = oringinVelocity;
        transform.parent = collision.transform;


        if (!isColliding)
        {
            // �浹 �� ���� �� �̵��ϵ��� ����
            isColliding = true;
            StartCoroutine(SlowDownAndStop(oringinVelocity));
        }
    }

    private IEnumerator SlowDownAndStop(Vector3 oringinVelocity)
    {
        yield return null;
        // �ӵ��� ������ ���ҽ�Ŵ
        transform.Translate(Vector3.forward * moveDistance, Space.Self);

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }
}
