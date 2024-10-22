using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
/*
    [활 시위 이동 단계 / 양쪽]
    0. 활에 Grab Interactable을 추가.
    1. 왼손이던지 오른손이던지 Direct Interactor를 이용하여 Select버튼으로 활의 몸통을 Grab한다.
    2. Select버튼이 눌러진 상태의 Grab의 반대 컨트롤러의 Active버튼이 눌러진다면
       활시위의 WB.StringCenter transform이 해당 컨트롤러로 움직일 수 있게 된다..
    3. 이때 Active가 눌러진 컨트롤러가 움직이면 활시위가 움직인다.
    4. 활 시위, 화살 날아가는 소리 Sound 장착.

    // 추가예정
    4. Socket Interaction을 사용하고 화살에 Grab Interactable을 사용한다.
    5. 

    [화살 장착 및 발사]
    0. 화살에 rigidbody추가 및 활과 화살 사이의 충돌 방지.
    1. 활이 당겨져있을때 화살 또한 같이 움직일 수 있게 움직임 제한 설정.
    2. Active버튼이 놓아졌을때 화살이 발사함과 동시에 해당 화살의 움직임 또한 제한 풀기.
    3. 화살이 날아갈 방향은 활 Grab한 컨트롤러의 Forward 방향으로 날리도록 하기. 
    4. 화살의 궤적이 나타나게 해서 궤적을 보여준다.
    5. 화살의 발사시 화살촉으로 중력을 적용하여 포물선으로 움직이게 조정.

    // 추가사항
    6. 화살의 탄착지점을 보여준다.
    
    [과녁 생성 및 점수 UI구현]
    0. 과녁을 생성하여 Center Transform으로 부터의 반경에 따라 점수를 배치한다.
    1. 과녁에 대한 Render Textrue를 생성하여 멀리서도 해당 과녁에 어디를 맞추었는지를 보여주게 한다. 
    2. 해당 타격지점 HitPoint에 대하여 이펙트를 추가하여 타격시 해당 지점에서 이펙트가 발생하도록 한다.
    3. 점수UI를 생성하여 과녁에 타격시에 해당 점수를 표시.
    4. GameManager를 생성하여 모드를 구분.(싱글 모드 : 5발 쏘아서 기록재기, 자유모드 : 마음대로 연습하는 모드) 
    5. Mode UI생성 및 버튼 프리펩을 이용하여 Poke Interactor로 동작하게 하기.(XR_StartAsset의 XRPushButton스크립트 활용)
    6. 화살이 충돌시 해당 점수를 보여주는 ScorePopUp UI 생성.(TextMeshPro를 사용하여 해당 지점에서 천천히 올라가다가 사라지게 표현.)

    [환경적 요소]
    0. 난이도 조절용.
    1. 바람의 세기로 인하여 화살의 궤적이 영향을 받도록 기능 구현하기.
        (Fixed Update를 통하여 화살에 바람의 세기 전달.)
    2. 나머지 코드 정리 및 추후 추가할 사항 정리하기.
    

*/


public class BowController : MonoBehaviour
{
    [Header("XR Interaction Settings")]
    [SerializeField] private XRGrabInteractable bowHandle;
    [SerializeField] private Transform stringCenterTransform;
    [SerializeField] private Transform stringStartPos;

    [Header("화살 세팅")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    [Header("컨트롤러 세팅")]
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
        Debug.Log("활 손잡이 잡음.");
        bIsHandleGrabbed = true;
    }

    private void OnHandleReleased(SelectExitEventArgs args)
    {
        Debug.Log("활 손잡이 놓음.");
        bIsHandleGrabbed = false;
        pullingController = null;
        ResetBowString();

        
        DestroyArrow();
    }

    void Update()
    {
        if (bIsHandleGrabbed)
        {
            // 현재 화살이 존재 안하면 생성.
            if (currentArrow == null && bIsStringPulled)
            {
                CreateArrow();
            }

            // 한 손으로 활을 잡고 있을 때만 Active 버튼 감지
            CheckActiveButton();

            // 화살이 생성되어 있으면 활시위가 당겨질 때
            // 화살도 함께 움직이도록 처리
            if (currentArrow != null && bIsStringPulled)
            {
                MoveArrowWithString();
            }

            // 활 시위가 특정 거리를 넘어서면 한 번만 소리 재생
            if (pullDistance >= soundTriggerDistance && !hasPlayedSound)
            {
                PlayStringPullSound();
                hasPlayedSound = true;
            }

            // 시위가 원래 위치로 돌아갔을 때 다시 사운드 재생 가능하게 설정
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
        // 활을 잡고 있는 컨트롤러 반환
        var grabbingController = bowHandle.interactorsSelecting[0] as XRBaseControllerInteractor;

        // 왼손으로 활을 잡고
        // 오른손의 Active 버튼이 눌렸다면 활시위를 움직임
        if (pullingController == null 
            && grabbingController.xrController == leftController 
            && rightController.activateInteractionState.active)
        {
            Debug.Log("오른손의 Active 버튼 눌림, 활시위를 오른손으로 조정");
            pullingController = rightController;
            bIsStringPulled = true;
        }
        // 오른손으로 활을 잡고,
        // 왼손의 Active 버튼이 눌렸다면 활시위를 움직임
        else if (pullingController == null 
            && grabbingController.xrController == rightController
            && leftController.activateInteractionState.active)
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
            FireArrow();
        }
    }

    // 활시위를 컨트롤러의 위치로 이동시키는 함수
    private void MoveStringWithController(XRBaseController controller)
    {
        // 활시위(WB.StringCenter)의 위치를 컨트롤러의 위치로 이동
        stringCenterTransform.position = controller.transform.position;
        stringCenterTransform.rotation = controller.transform.rotation;

        // 활시위 당긴 거리 계산
        pullDistance = Vector3.Distance(stringCenterTransform.position, stringStartPos.position);
    }

    // 활시위와 함께 화살을 이동시키는 함수
    private void MoveArrowWithString()
    {
        if (currentArrow != null)
        {
            currentArrow.transform.position = stringCenterTransform.position;
            currentArrow.transform.rotation = stringCenterTransform.rotation;
        }
    }

    // 활시위를 원래 위치로 되돌리는 함수
    private void ResetBowString()
    {
        Debug.Log("활시위를 원래 위치로 복원.");
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

    // 화살 발사 함수
    private void FireArrow()
    {
        if (pullDistance > 0f && currentArrow != null)
        {
            Arrow arrow = currentArrow.GetComponent<Arrow>();

            // 활을 당기고 있는 컨트롤러의 forward 방향을 사용
            Vector3 fireDirection = pullingController.transform.forward;

            // 활시위 당긴 거리에 비례한 힘을 계산
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




