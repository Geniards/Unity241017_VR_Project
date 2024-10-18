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
        
        // ���⿡ ���� �ӵ� ���� (Sign���� ���⼳��.)
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
            // ȭ���� ���ư� �� ȭ������ �Ʒ��� ���ϵ��� ȸ�� ����
            Vector3 velocity = rb.velocity;
            if (velocity.magnitude > 0.1f)
            {
                // ���� �ӵ��� ������ ���� ȸ��
                Quaternion targetRotation = Quaternion.LookRotation(velocity);
                // �ڿ������� ȸ��
                rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 2f));
            }
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
            //stopTimer = 0f;
            StartCoroutine(SlowDownAndStop(oringinVelocity));
        }
    }

    private IEnumerator SlowDownAndStop(Vector3 oringinVelocity)
    {
        //Vector3 originalVelocity = rb.velocity;

        // ������ �ӵ��� ���̸鼭 ȭ���� ����
        //while (stopTimer < stopTime)
        //{
        //    stopTimer += Time.deltaTime;
        //    float lerpFactor = stopTimer / stopTime;

        //    // �ӵ��� ������ ���ҽ�Ŵ
        //    transform.Translate(Vector3.forward * moveDistance, Space.Self);
        //    //Vector3.Lerp(oringinVelocity, Vector3.zero, lerpFactor);
        //    yield return null;

        //}


        yield return null;
        // �ӵ��� ������ ���ҽ�Ŵ
        transform.Translate(Vector3.forward * moveDistance, Space.Self);

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }
}
