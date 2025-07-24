# 🎯 VR Archer

> VR 환경에서 중력과 바람 등 환경 요소에 맞춰 활을 조준하고 과녁을 명중시키는 VR 활쏘기 게임입니다.

---

## 📌 Project Overview

- **프로젝트 명**: VR Archer  
- **개발 기간**: 2024.10.17 ~ 2024.10.23  
- **개발 인원**: 1인 개발  
- **개발 툴**: Unity 2021.3.42f1, Visual Studio 2022, GitHub  
- **XR 툴킷**: XR Interaction Toolkit  
- **언어**: C#

---

## 🎥 시연 영상

[YouTube 시연 영상 바로가기](https://youtu.be/aXjBkHACTnU)

---

## 📂 Repository

- [기술문서](https://www.notion.so/VR-Archery-23a28bcaf90480c89693f720db7f6787)

---

## 🔧 주요 기능 및 기술 구현

### 1. 플레이어 조작
- **양손 컨트롤러 기반 상호작용 구현**
  - `XR GrabInteractable`로 활을 잡고 당김
  - `XR BaseController`로 Select / Activate 입력 처리
- **UI 조작**
  - `XR PushButton`으로 게임 시작 및 모드 선택 구현
<img width="817" height="792" alt="image (3)" src="https://github.com/user-attachments/assets/afb62b63-4a37-4056-8a73-f122fb5c4ca4" />
    


### 2. 활 및 화살 시스템
- `arrowSpawnPoint.forward` 방향으로 발사
- `Rigidbody.velocity`로 초기 속도 부여
- `TrailRenderer`로 궤적 표현
- 중력은 `Coroutine`으로 지연 적용
- `AddForce()`를 통해 바람 영향 구현
  
![image (2)-1](https://github.com/user-attachments/assets/4c2bf304-2c1d-4f12-8e3f-2edd27d108f6)



### 3. 과녁 충돌 시스템
- `OnCollisionEnter()` 사용
- 충돌 후 `isKinematic = true`로 고정
- `relativeVelocity`로 충돌 방향을 역산하여 자연스럽게 정착 처리
  
![image (1)-1](https://github.com/user-attachments/assets/4ab67a55-d073-4010-a7c1-66357c588bf2)



### 4. 점수 시스템
- `TextMeshPro`로 점수 UI 구현
- 화살 충돌 지점 기준 거리 계산 → 점수화
- 5라운드 기록 → 최종 점수 표시
  
![image (3)](https://github.com/user-attachments/assets/704139ac-dbf1-4519-91cf-c14f9474aa9d)

---

<조작법>
* 컨트롤러의 Select 버튼으로 활을 잡으시면됩니다. 
  * 절대 놓치지마세요
    * (오른손잡이시면 왼손으로 활을 집으시면 됩니다.)
    * (왼손잡이는 오른손으로 활을 집으시면 됩니다.)
* 활을 잡은 컨트롤러의 반대 컨트롤러의 Active버튼을 꾸욱 누른상태로 활시위를 당겨주세요.
* 바람의 세기를 확인 후 Active버튼을 놓아주세요.  
  * 화살이 발사될것입니다.


<모드설정>
* toggle버튼으로 되어있으니 poke interactor로 잘 눌러서 선택해주세요.
* 프리모드와 싱글모드가 있습니다.
  * (기본은 프리모드로 되어있습니다.)
* 프리모드는 자유롭게 바람을 잘 제어하면서 화살을 쏘세요.
* 자유모드는 5발의 화살로 최고점수를 만들어보시면 됩니다.

<개발내용>
* 컨트롤러로 활과 화살을 제어가능.
  * 반대 손 호환 작업 완료.
* 화살의 발사시 중력적용 및 회전 보정 적용.
* 환경적 요소로 인한 바람의 세기가 매턴 변화
  * 바람의 의한 화살의 궤적변화 적용.
* 화살이 충돌시 해당 지점의 점수 PopUp으로 UI 표시.
* 과녁의 점수별 이펙트 개별적용.
* 싱글모드 및 자유모드 Toggle버튼으로 제작하여 컨트롤러로 동작.

