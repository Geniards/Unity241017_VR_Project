using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// [Ȱ ���� �̵� �ܰ� / ����]
// 0. Ȱ�� Grab Interactable�� �߰�.
// 1. �޼��̴��� �������̴��� Direct Interactor�� �̿��Ͽ� Select��ư���� Ȱ�� ������ Grab�Ѵ�.
// 2. Select��ư�� ������ ������ Grab�� �ݴ� ��Ʈ�ѷ��� Active��ư�� �������ٸ�
//      Ȱ������ WB.StringCenter transform�� �ش� ��Ʈ�ѷ��� ������ �� �ְ� �ȴ�..
// 3. �̶� Active�� ������ ��Ʈ�ѷ��� �����̸� Ȱ������ �����δ�.


public class BowController : MonoBehaviour
{
    [Header("XR Interaction Settings")]
    [SerializeField] private XRGrabInteractable bowHandle;
    [SerializeField] private Transform stringCenterTransform;
    [SerializeField] private Transform stringStartPos;

    [SerializeField] private XRBaseController leftController;
    [SerializeField] private XRBaseController rightController;

    private XRBaseController pullingController;
    private bool bIsHandleGrabbed = false;
    private bool bIsStringPulled = false;

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
    }

    void Update()
    {
        if (bIsHandleGrabbed)
        {
            // �� ������ Ȱ�� ��� ���� ���� Active ��ư ����
            CheckActiveButton();
        }
    }

    private void CheckActiveButton()
    {
        // �޼����� Ȱ�� ���
        // �������� Active ��ư�� ���ȴٸ� Ȱ������ ������
        if (pullingController == null && rightController.activateInteractionState.active)
        {
            Debug.Log("�������� Active ��ư ����, Ȱ������ ���������� ����");
            pullingController = rightController;
            bIsStringPulled = true;
        }
        // ���������� Ȱ�� ���,
        // �޼��� Active ��ư�� ���ȴٸ� Ȱ������ ������
        else if (pullingController == null && leftController.activateInteractionState.active)
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
            ResetBowString();
        }
    }

    // Ȱ������ ��Ʈ�ѷ��� ��ġ�� �̵���Ű�� �Լ�
    private void MoveStringWithController(XRBaseController controller)
    {
        // Ȱ����(WB.StringCenter)�� ��ġ�� ��Ʈ�ѷ��� ��ġ�� �̵�
        stringCenterTransform.position = controller.transform.position;
        stringCenterTransform.rotation = controller.transform.rotation;
    }

    // Ȱ������ ���� ��ġ�� �ǵ����� �Լ�
    private void ResetBowString()
    {
        Debug.Log("Ȱ������ ���� ��ġ�� ����.");
        stringCenterTransform.position = stringStartPos.position;
        stringCenterTransform.rotation = stringStartPos.rotation;
        pullingController = null;
        bIsStringPulled = false;
    }
}




