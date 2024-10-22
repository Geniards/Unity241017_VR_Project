using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
/*
    [Ȱ ���� �̵� �ܰ� / ����]
    0. Ȱ�� Grab Interactable�� �߰�.
    1. �޼��̴��� �������̴��� Direct Interactor�� �̿��Ͽ� Select��ư���� Ȱ�� ������ Grab�Ѵ�.
    2. Select��ư�� ������ ������ Grab�� �ݴ� ��Ʈ�ѷ��� Active��ư�� �������ٸ�
       Ȱ������ WB.StringCenter transform�� �ش� ��Ʈ�ѷ��� ������ �� �ְ� �ȴ�..
    3. �̶� Active�� ������ ��Ʈ�ѷ��� �����̸� Ȱ������ �����δ�.
    4. Ȱ ����, ȭ�� ���ư��� �Ҹ� Sound ����.

    // �߰�����
    4. Socket Interaction�� ����ϰ� ȭ�쿡 Grab Interactable�� ����Ѵ�.
    5. 

    [ȭ�� ���� �� �߻�]
    0. ȭ�쿡 rigidbody�߰� �� Ȱ�� ȭ�� ������ �浹 ����.
    1. Ȱ�� ����������� ȭ�� ���� ���� ������ �� �ְ� ������ ���� ����.
    2. Active��ư�� ���������� ȭ���� �߻��԰� ���ÿ� �ش� ȭ���� ������ ���� ���� Ǯ��.
    3. ȭ���� ���ư� ������ Ȱ Grab�� ��Ʈ�ѷ��� Forward �������� �������� �ϱ�. 
    4. ȭ���� ������ ��Ÿ���� �ؼ� ������ �����ش�.
    5. ȭ���� �߻�� ȭ�������� �߷��� �����Ͽ� ���������� �����̰� ����.

    // �߰�����
    6. ȭ���� ź�������� �����ش�.
    
    [���� ���� �� ���� UI����]
    0. ������ �����Ͽ� Center Transform���� ������ �ݰ濡 ���� ������ ��ġ�Ѵ�.
    1. ���ῡ ���� Render Textrue�� �����Ͽ� �ָ����� �ش� ���ῡ ��� ���߾������� �����ְ� �Ѵ�. 
    2. �ش� Ÿ������ HitPoint�� ���Ͽ� ����Ʈ�� �߰��Ͽ� Ÿ�ݽ� �ش� �������� ����Ʈ�� �߻��ϵ��� �Ѵ�.
    3. ����UI�� �����Ͽ� ���ῡ Ÿ�ݽÿ� �ش� ������ ǥ��.
    4. GameManager�� �����Ͽ� ��带 ����.(�̱� ��� : 5�� ��Ƽ� ������, ������� : ������� �����ϴ� ���) 
    5. Mode UI���� �� ��ư �������� �̿��Ͽ� Poke Interactor�� �����ϰ� �ϱ�.(XR_StartAsset�� XRPushButton��ũ��Ʈ Ȱ��)
    6. ȭ���� �浹�� �ش� ������ �����ִ� ScorePopUp UI ����.(TextMeshPro�� ����Ͽ� �ش� �������� õõ�� �ö󰡴ٰ� ������� ǥ��.)

    [ȯ���� ���]
    0. ���̵� ������.
    1. �ٶ��� ����� ���Ͽ� ȭ���� ������ ������ �޵��� ��� �����ϱ�.
        (Fixed Update�� ���Ͽ� ȭ�쿡 �ٶ��� ���� ����.)
    2. ������ �ڵ� ���� �� ���� �߰��� ���� �����ϱ�.
    

*/


public class BowController : MonoBehaviour
{
    [Header("XR Interaction Settings")]
    [SerializeField] private XRGrabInteractable bowHandle;
    [SerializeField] private Transform stringCenterTransform;
    [SerializeField] private Transform stringStartPos;

    [Header("ȭ�� ����")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    [Header("��Ʈ�ѷ� ����")]
    [SerializeField] private XRBaseController leftController;
    [SerializeField] private XRBaseController rightController;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource bowAudioSource;
    [SerializeField] private AudioClip stringPullSound;
    [SerializeField] private AudioClip arrowReleaseSound;
    private bool hasPlayedSound = false;
    private float soundTriggerDistance = 0.1f;

    private XRBaseController pullingController;
    private bool bIsHandleGrabbed = false;
    private bool bIsStringPulled = false;
    private float pullDistance = 0f;
    private float maxPullDistance = 0.5f;

    private GameObject currentArrow;

    void Start()
    {
        bowHandle.selectEntered.AddListener(OnHandleGrabbed);
        bowHandle.selectExited.AddListener(OnHandleReleased);
    }

    private void OnHandleGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Ȱ ������ ����.");
        bIsHandleGrabbed = true;
    }

    private void OnHandleReleased(SelectExitEventArgs args)
    {
        Debug.Log("Ȱ ������ ����.");
        bIsHandleGrabbed = false;
        pullingController = null;
        ResetBowString();

        
        DestroyArrow();
    }

    void Update()
    {
        if (bIsHandleGrabbed)
        {
            // ���� ȭ���� ���� ���ϸ� ����.
            if (currentArrow == null && bIsStringPulled)
            {
                CreateArrow();
            }

            // �� ������ Ȱ�� ��� ���� ���� Active ��ư ����
            CheckActiveButton();

            // ȭ���� �����Ǿ� ������ Ȱ������ ����� ��
            // ȭ�쵵 �Բ� �����̵��� ó��
            if (currentArrow != null && bIsStringPulled)
            {
                MoveArrowWithString();
            }

            // Ȱ ������ Ư�� �Ÿ��� �Ѿ�� �� ���� �Ҹ� ���
            if (pullDistance >= soundTriggerDistance && !hasPlayedSound)
            {
                PlayStringPullSound();
                hasPlayedSound = true;
            }

            // ������ ���� ��ġ�� ���ư��� �� �ٽ� ���� ��� �����ϰ� ����
            if (pullDistance < soundTriggerDistance && hasPlayedSound)
            {
                hasPlayedSound = false;
            }
        }
    }

    private void PlayStringPullSound()
    {
        if (bowAudioSource != null && stringPullSound != null)
        {
            bowAudioSource.clip = stringPullSound;
            bowAudioSource.Play();
        }
    }

    private void PlayArrowReleaseSound()
    {
        if (bowAudioSource != null && arrowReleaseSound != null)
        {
            bowAudioSource.clip = arrowReleaseSound;
            bowAudioSource.Play();
        }
    }

    private void CheckActiveButton()
    {
        // Ȱ�� ��� �ִ� ��Ʈ�ѷ� ��ȯ
        var grabbingController = bowHandle.interactorsSelecting[0] as XRBaseControllerInteractor;

        // �޼����� Ȱ�� ���
        // �������� Active ��ư�� ���ȴٸ� Ȱ������ ������
        if (pullingController == null 
            && grabbingController.xrController == leftController 
            && rightController.activateInteractionState.active)
        {
            Debug.Log("�������� Active ��ư ����, Ȱ������ ���������� ����");
            pullingController = rightController;
            bIsStringPulled = true;
        }
        // ���������� Ȱ�� ���,
        // �޼��� Active ��ư�� ���ȴٸ� Ȱ������ ������
        else if (pullingController == null 
            && grabbingController.xrController == rightController
            && leftController.activateInteractionState.active)
        {
            Debug.Log("�޼��� Active ��ư ����, Ȱ������ �޼����� ����");
            pullingController = leftController;
            bIsStringPulled = true;
        }

        // Active ��ư�� ���� �ִ� ���� Ȱ���� �̵�
        if (pullingController != null && pullingController.activateInteractionState.active)
        {
            MoveStringWithController(pullingController);
        }
        // Active ��ư�� ������ Ȱ������ ���� ��ġ�� �ǵ���
        else if (pullingController != null && !pullingController.activateInteractionState.active)
        {
            FireArrow();
        }
    }

    // Ȱ������ ��Ʈ�ѷ��� ��ġ�� �̵���Ű�� �Լ�
    private void MoveStringWithController(XRBaseController controller)
    {
        // Ȱ����(WB.StringCenter)�� ��ġ�� ��Ʈ�ѷ��� ��ġ�� �̵�
        stringCenterTransform.position = controller.transform.position;
        stringCenterTransform.rotation = controller.transform.rotation;

        // Ȱ���� ��� �Ÿ� ���
        pullDistance = Vector3.Distance(stringCenterTransform.position, stringStartPos.position);
    }

    // Ȱ������ �Բ� ȭ���� �̵���Ű�� �Լ�
    private void MoveArrowWithString()
    {
        if (currentArrow != null)
        {
            currentArrow.transform.position = stringCenterTransform.position;
            currentArrow.transform.rotation = stringCenterTransform.rotation;
        }
    }

    // Ȱ������ ���� ��ġ�� �ǵ����� �Լ�
    private void ResetBowString()
    {
        Debug.Log("Ȱ������ ���� ��ġ�� ����.");
        stringCenterTransform.position = stringStartPos.position;
        stringCenterTransform.rotation = stringStartPos.rotation;
        pullingController = null;
        bIsStringPulled = false;
        pullDistance = 0f;
    }

    private void CreateArrow()
    {
        if (currentArrow == null)
        {
            currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        }
    }

    // ȭ�� �߻� �Լ�
    private void FireArrow()
    {
        if (pullDistance > 0f && currentArrow != null)
        {
            Arrow arrow = currentArrow.GetComponent<Arrow>();

            // Ȱ�� ���� �ִ� ��Ʈ�ѷ��� forward ������ ���
            Vector3 fireDirection = pullingController.transform.forward;

            // Ȱ���� ��� �Ÿ��� ����� ���� ���
            float power = Mathf.Clamp(pullDistance / maxPullDistance, -1f, 1f);

            arrow.Fire(arrowSpawnPoint.forward, power);
            currentArrow = null;

            PlayArrowReleaseSound();
        }

        ResetBowString();
    }

    private void DestroyArrow()
    {
        if (currentArrow != null)
        {
            currentArrow = null;
            Destroy(currentArrow);
        }
    }
}




