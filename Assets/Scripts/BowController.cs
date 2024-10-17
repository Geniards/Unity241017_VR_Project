using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// [활 시위 이동 단계 / 양쪽]
// 0. 활에 Grab Interactable을 추가.
// 1. 왼손이던지 오른손이던지 Direct Interactor를 이용하여 Select버튼으로 활의 몸통을 Grab한다.
// 2. Select버튼이 눌러진 상태의 Grab의 반대 컨트롤러의 Active버튼이 눌러진다면
//      활시위의 WB.StringCenter transform이 해당 컨트롤러로 움직일 수 있게 된다..
// 3. 이때 Active가 눌러진 컨트롤러가 움직이면 활시위가 움직인다.


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
        Debug.Log("활 손잡이 잡음.");
        bIsHandleGrabbed = true;
    }

    private void OnHandleReleased(SelectExitEventArgs args)
    {
        Debug.Log("활 손잡이 놓음.");
        bIsHandleGrabbed = false;
        pullingController = null;
        ResetBowString();
    }

    void Update()
    {
        if (bIsHandleGrabbed)
        {
            // 한 손으로 활을 잡고 있을 때만 Active 버튼 감지
            CheckActiveButton();
        }
    }

    private void CheckActiveButton()
    {
        // 왼손으로 활을 잡고
        // 오른손의 Active 버튼이 눌렸다면 활시위를 움직임
        if (pullingController == null && rightController.activateInteractionState.active)
        {
            Debug.Log("오른손의 Active 버튼 눌림, 활시위를 오른손으로 조정");
            pullingController = rightController;
            bIsStringPulled = true;
        }
        // 오른손으로 활을 잡고,
        // 왼손의 Active 버튼이 눌렸다면 활시위를 움직임
        else if (pullingController == null && leftController.activateInteractionState.active)
        {
            Debug.Log("왼손의 Active 버튼 눌림, 활시위를 왼손으로 조정");
            pullingController = leftController;
            bIsStringPulled = true;
        }

        // Active 버튼이 눌려 있는 동안 활시위 이동
        if (pullingController != null && pullingController.activateInteractionState.active)
        {
            MoveStringWithController(pullingController);
        }
        // Active 버튼을 놓으면 활시위를 원래 위치로 되돌림
        else if (pullingController != null && !pullingController.activateInteractionState.active)
        {
            ResetBowString();
        }
    }

    // 활시위를 컨트롤러의 위치로 이동시키는 함수
    private void MoveStringWithController(XRBaseController controller)
    {
        // 활시위(WB.StringCenter)의 위치를 컨트롤러의 위치로 이동
        stringCenterTransform.position = controller.transform.position;
        stringCenterTransform.rotation = controller.transform.rotation;
    }

    // 활시위를 원래 위치로 되돌리는 함수
    private void ResetBowString()
    {
        Debug.Log("활시위를 원래 위치로 복원.");
        stringCenterTransform.position = stringStartPos.position;
        stringCenterTransform.rotation = stringStartPos.rotation;
        pullingController = null;
        bIsStringPulled = false;
    }
}




