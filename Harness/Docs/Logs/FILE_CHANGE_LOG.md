# 파일 변경 이력

## 현재 커밋 안 된 작업

<!-- 이 부분은 깃허브에 올릴때에는 항상 이 주석만 있도록 유지해야한다 -->

## 2026-06-24

### 마녀 포션 포물선 표시 거리 제한

- `WitchVisibleAttack`에 포물선 최대 표시 거리 값을 추가해 차징 중 궤적이 일정 거리 이상 길어지지 않도록 제한했다.
- 마녀 프리팹의 `WitchVisibleAttack`에 기본 최대 표시 거리를 8로 설정해 인스펙터에서 조절할 수 있게 했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

### 공격 범위 표시 컴포넌트 위치 수정

- 캐릭터 루트에 붙은 `*VisibleAttack`이 기존 `SpriteRenderer`와 같은 GameObject에 `MeshRenderer`를 추가하려다 NullReferenceException이 발생할 수 있던 구조를 수정했다.
- 전사 / 방망이 / 레일건 / 마녀 프리팹에 `*VisibleAttack` 전용 자식 오브젝트를 추가하고, 표시 컴포넌트는 해당 자식에서 렌더링하도록 정리했다.
- `BaseAttackController`가 루트의 `*VisibleAttack`을 사용하거나 런타임 GameObject를 생성하지 않고, 자식 표시 컴포넌트만 찾아 사용하도록 변경했다.
- `PotionAreaIndicator`는 포션 충돌 후 장판 범위만 표시하도록 `WitchPotionAreaVisibleAttack`으로 분리하고, 마녀 투척 포물선 설정 필드가 섞이지 않게 정리했다.
- 레일건 공격 범위 시각화의 전방 길이 배율을 0.5로 낮춰 기존 가로 길이의 절반만 표시되도록 조정했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

## 2026-06-22

### 공격 범위 표시 책임 분리

- `VisibleAttack`은 공통 메시 / 라인 렌더링 컴포넌트로 유지하고, 전사 / 방망이 / 레일건 / 마녀 전용 `*VisibleAttack` 스크립트를 추가했다.
- `VisibleAttack`을 추상 공통 렌더러로 줄이고, 박스 / 삼각형 버텍스 생성은 캐릭터별 `*VisibleAttack` 자식 스크립트에서 구현하도록 분리했다.
- `BaseAttackController`가 캐릭터별 표시 컴포넌트를 타입 기준으로 런타임 생성 / 재사용하도록 변경했다.
- 전사와 방망이 공격 컨트롤러는 직접 도형을 그리지 않고 전용 표시 스크립트만 호출하도록 정리했다.
- 레일건 차징 표시는 캐릭터 위치를 꼭짓점으로 두고 바라보는 방향 앞쪽에 밑변이 오도록 수정했으며, 차징 중 전방 길이는 유지하고 밑변 폭만 줄어들도록 했다.
- 레일건 공격 컨트롤러에 범위 표시 길이 배율 / 오프셋 필드를 추가해 표시 길이를 인스펙터에서 조정할 수 있게 했다.
- 공격 범위 표시가 별도 런타임 GameObject를 생성하지 않고 캐릭터에 붙은 `*VisibleAttack` 컴포넌트에서 월드 좌표 버텍스로 그려지도록 수정했다.
- 방망이와 레일건의 차징 범위 표시가 차징 비율에 따라 색상과 투명도가 더 진해지도록 조정했다.
- 마녀 포션 차징 중 실제 던지기 방향 / 힘 / 중력 기준의 점 형태 예측 포물선을 표시하고 릴리즈 시 숨기도록 했다.
- 전사 / 방망이 / 레일건 / 마녀 캐릭터 프리팹에 각 `*VisibleAttack` 컴포넌트를 붙여 인스펙터에서 표시값을 조절할 수 있게 했다.
- 마녀 포션 장판 프리팹은 `WitchVisibleAttack`을 사용하도록 스크립트 참조를 교체했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

## 2026-06-20

### 인게임 공격 범위 표시 추가

- `VisibleAttack`를 런타임 범위 표시 컴포넌트로 구현해 삼각형 / 사각형 범위를 반투명 채움과 외곽선으로 표시하도록 했다.
- `BaseAttackController`에서 공격 표시 오브젝트를 런타임 확보하도록 해 캐릭터 프리팹 연결 변경 없이 표시를 사용할 수 있게 했다.
- 전사 일반 공격은 기존 `_attackBoxSize` 판정 위치에 빨간 사각형을 짧게 표시하도록 했다.
- 방망이 일반 공격은 차징 중 기존 `_attackSize` 판정 위치에 주황에서 빨강으로 변하는 사각형을 표시하고, release 후 짧게 유지하도록 했다.
- 레일건 일반 공격 / 스킬 차징 중 기존 감지 폭 변화에 맞춘 노랑 / 청록 삼각형을 표시하고, release 후 즉시 숨기도록 했다.
- 마녀 포션 충돌 시 기존 `_overlapSize` 크기의 장판 표시를 잠깐 남기고, Pain / Poison / Slow / Wind 포션별 색상을 구분했다.
- `PotionAreaIndicator` 풀 프리팹을 추가하고 `Map1`의 `PoolManager`에 `PotionAreaIndicator` 키로 등록했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

## 2026-06-08

### 디버그 매니저 관찰 오버레이 추가

- `DebugManager`에 `F2` 상태 오버레이를 추가해 현재 씬, FPS와 frame time, 화면 해상도를 확인할 수 있도록 했다.
- `DebugManager`에 `F3` 로그 오버레이를 추가해 `Application.logMessageReceived`로 수집한 Unity 로그 최근 30개 중 8개를 표시하도록 했다.
- 로그 오버레이에서 `Log`, `Warning`, `Error/Exception` 계열을 색상으로 구분하고 메시지는 첫 줄과 길이 제한 중심으로 표시하도록 했다.
- 기존 `F1` 도움말과 `M` 커서 토글은 유지하고, 게임 로직 클래스 참조 없이 에디터 전용 관찰 기능 안에서만 확장했다.
- 오버레이 박스와 글자 크기를 키우고, 중복 표시를 줄이기 위해 상태 오버레이의 커서 표시 / 잠금 상태 줄은 제거했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

## 2026-06-07

### 디버그 매니저 기능 추가

- `DebugManager`를 에디터 전용 자동 생성 싱글톤으로 정리하고 씬 전환 후에도 유지되도록 했다.
- 게임 시작 시 마우스 커서를 기본 비활성화하고 `M` 키로 커서 표시 상태를 토글하도록 했다.
- `F1` 키로 현재 씬, 커서 상태, 디버그 키를 표시하는 간단한 도움말 오버레이를 추가했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

## 2026-06-06

### PR 리뷰 코멘트 반영

- `InitializePlayers`에서 유령 풀 초기화 후 기존 활성 유령을 정리하도록 해 강제 재초기화 시 남은 유령이 유지되지 않도록 했다.
- `GhostManager.IgnorePlayerCollisions`는 플레이어 콜라이더를 먼저 수집한 뒤 유령 콜라이더와 충돌 무시를 적용하는 현재 최적화 상태를 확인했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

### 유령 움직임 인터페이스 정의

- `IGhostMovementController`를 추가해 유령 이동 구현체의 초기화, 이동 갱신, 중지 계약을 정의했다.
- 유령 이동 초기화는 현재 생성 위치, 소유자, 대상 정보가 필요 없으므로 인자 없는 `Initialize()`로 유지했다.
- `GhostManager`가 유령 생성 직후 이동 컨트롤러가 붙어 있으면 초기화하고, 유령 정리 전에는 이동 중지를 호출하도록 했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

## 2026-06-05

### 유령 물리 충돌 분리

- `Ghost` 레이어를 추가하고 전사, 마녀, 방망이, 레일건 유령 프리팹의 루트 레이어를 `Ghost`로 변경했다.
- 기존 2D 물리 레이어 행렬에서 `Ghost` 레이어가 캐릭터 레이어와 충돌하지 않고 `Ground`와는 충돌하는 것을 확인했다.
- 유령 생성 시 런타임에서도 플레이어 입력 컴포넌트를 비활성화하고 `Ghost` 레이어를 자식까지 적용하도록 보강했다.
- 유령 콜라이더와 현재 씬의 플레이어 콜라이더 간 `Physics2D.IgnoreCollision`을 적용해 프리팹 설정 누락에도 캐릭터를 밀지 않도록 했다.
- `ChargeInputHandler`가 `PlayerInput` 없는 유령에서 NullReference를 내지 않도록 방어 조건을 수정했다.
- 검증
  - 유령 프리팹 4종의 `m_Layer`가 20번으로 설정된 것을 확인했다.
  - `Ghost`와 `Warrior`, `RailGun`, `Witch`, `BatMan` 레이어 충돌이 꺼져 있고 `Ground` 충돌은 켜져 있는 것을 확인했다.
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

### 라운드 종료 시 유령 정리

- `GhostManager`가 생성한 유령과 풀 키를 추적하도록 했다.
- 게임 종료 시 `InGameManager`의 라운드 종료 흐름에서 모든 활성 유령을 풀로 반환하도록 했다.
- `GhostCreateManager` 파일, 클래스, 참조 이름을 `GhostManager`로 변경했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

## 2026-06-02

### 공격 컨트롤러 공통 부모 클래스

- `BaseAttackController`를 추가해 공격 컨트롤러 공통 초기화와 보조 로직을 분리했다.
  - `PlayerController`, `PlayerStat`, `BoxCollider2D` 캐싱과 공격 / 스킬 핸들러 연결을 부모 클래스에서 처리하도록 했다.
  - 공격 / 스킬 쿨타임 처리, 바라보는 방향 계산, `ContactFilter2D`, 히트 버퍼, 팀 기반 적 판정 보조 메서드를 추가했다.
- 전사, 마녀, 방망이, 레일건 공격 컨트롤러가 `BaseAttackController`를 상속하도록 변경했다.
  - 캐릭터별 공격, 차징, 스킬 동작과 기존 직렬화 필드는 유지했다.
  - 차징형 캐릭터의 `IChargeable` 계약과 `ReleaseCharge` 흐름은 유지했다.
- `Assembly-CSharp.csproj`에 새 부모 클래스 스크립트를 포함해 로컬 C# 검증 대상에 반영했다.
- 검증
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

## 2026-05-28

### UI 입력 분리

- `UIInput`의 선택 상태를 P1/P2 커서로 분리했다.
  - 기존 `OnMove`, `OnSubmit`, `OnCancel` 메서드는 유지하면서 액션맵/액션 이름이 `Player2` 또는 `P2`일 때 P2 커서로 라우팅하도록 했다.
  - 인스펙터에서 명시 연결할 수 있도록 `OnPlayer1Move/Submit/Cancel`, `OnPlayer2Move/Submit/Cancel` 메서드를 추가했다.
  - P1/P2 입력을 개별로 켜고 끄는 메서드와 P1만 입력받는 전환 메서드를 추가했다.
- `PlayerInputActions`에 UI 전용 액션맵 `UI_P1`, `UI_P2`를 추가했다.
  - `UI_P1`: WASD 이동, F 확인, G 취소.
  - `UI_P2`: 방향키 이동, Numpad 0 확인, Numpad . 취소.
- `UIInputActionBinder`를 추가하고 메인씬 `UIInputManager`에 연결했다.
  - 기존 플레이어 프리팹의 `Player1/Player2` 전투 입력 이벤트는 건드리지 않고, UI 전용 액션맵만 코드로 구독하도록 분리했다.
- 기존 단일 `UI` 액션맵과 `UI/...` 이벤트 참조를 제거했다.
  - 메인씬 `UIInputManager`의 중복 UI 이벤트 연결을 제거하고 기본 액션맵을 `UI_P1`로 변경했다.
  - 맵 레이아웃 씬의 플레이어 프리팹 인스턴스에 남아 있던 오래된 `UI/...` 이벤트 오버라이드를 제거했다.
- `UIButton`의 `GoScene` 처리에 빈 씬명 방어를 추가했다.
  - `_nextSceneName`이 비어 있으면 경고만 남기고 `SceneManager.LoadScene`을 호출하지 않도록 했다.
- 캐릭터 선택 씬에 UI 선택 흐름 컨트롤러를 추가했다.
  - `UIInput`의 P1/P2 선택, 확인, 취소 이벤트를 캐릭터 선택 씬에서 가로챌 수 있도록 확장했다.
  - `CharacterSelectUIController`를 추가해 캐릭터 버튼 확인 시 P1/P2 hover를 각 플레이어 패널 버튼으로 이동시키고, 취소 시 확정했던 캐릭터 버튼으로 되돌리도록 했다.
  - P1 hover는 파랑, P2 hover는 빨강으로 표시하도록 했다.
  - 확인한 캐릭터 버튼은 노란색으로 표시하고, 취소하면 원래 색으로 복구하도록 했다.
  - P1/P2가 모두 확인하면 중앙 `LeftTime` 텍스트로 5초 카운트다운을 표시한 뒤 `Map1` 씬으로 이동하도록 했다.
  - 캐릭터 선택 씬 `MainPanel`에 컨트롤러를 연결했다.
  - 입력은 `DontDestroyOnLoad`되는 전역 `UIInputManager`를 사용하도록 유지했다.
- 검증
  - `PlayerInputActions.inputactions` JSON 파싱과 액션맵 목록을 확인했다.
  - `UI/Move`, `UI/Submit`, `UI/Cancel` 잔여 참조가 없는 것을 확인했다.
  - `dotnet build Assembly-CSharp.csproj --no-restore`로 C# 컴파일을 확인했다.

## 2026-05-27

### 하네스 구조

- AI 개발 하네스 문서 구조를 추가했다.
  - AI가 먼저 읽을 운영 안내서, 작업 지시서, 컨벤션, 기획, 비주얼, 변경 로그 문서를 생성했다.
  - `Harness/prompt.md`는 작업 완료 후 제거했다.
- 하네스 전체를 점검하고 실제 프로젝트 구조를 반영했다.
  - 주요 패키지, 등록 씬, 전투 인터페이스, UI/풀링 구조를 문서에 추가했다.
  - `WORK_ORDER.md` 작업 지시 내용을 비워 템플릿 상태로 복구했다.
- DUOX 기획서 기준으로 하네스 문서를 보강했다.
  - 2인 로컬 플랫포머 액션 PVP, 낙사/넉백 중심 전투, 유령 시스템, 캐릭터/맵 선택 흐름을 반영했다.
  - 고대비 1bit 기반 제한 팔레트 픽셀아트 방향을 화면 / UX 하네스에 반영했다.

### 작업 지시와 로그 규칙

- GitHub README에 작업 지시서 및 하네스 사용 가이드를 추가했다.
  - `Harness` 문서 읽기 순서, `WORK_ORDER.md` 작성법, `메모 작업해` 사용법을 정리했다.
  - 작업 지시서와 파일 변경 이력의 커밋 규칙을 README에 추가했다.
- 작업 지시서 템플릿 주석 유지 규칙을 하네스에 반영했다.
  - `WORK_ORDER.md`의 `작업 이름`, `상세 요구사항`, `유저 메모장` 주석은 작업 지시가 없을 때도 유지하도록 명시했다.
- `FILE_CHANGE_LOG.md`의 `현재 커밋 안 된 작업` 주석 유지 규칙을 복구했다.
  - 커밋 전 실제 작업 내용은 비우되, GitHub 업로드용 주석은 남기도록 명시했다.
- `현재 커밋 안 된 작업` 로그 작성 규칙을 변경했다.
  - 해당 섹션은 자동 갱신하지 않고 사용자가 요청했을 때만 작성하도록 README와 하네스 문서에 명시했다.
- 파일 변경 이력의 날짜별 로그를 주제별 소제목으로 묶어 가독성을 개선했다.

### GitHub / PR 규칙

- PR 제목 형식 규칙을 하네스에 추가했다.
  - PR 제목은 `[Type] 작업 설명` 형식으로 작성하고 대괄호 안 타입 첫 글자는 대문자로 쓴다.
- PR 본문 형식 규칙을 하네스에 추가했다.
  - PR 본문은 `개요`, `변경한 내용`, `기타 사항` 3개 섹션으로 작성하도록 정했다.

### PR 리뷰 반영

- 미확정 보스 선택 씬 언급을 하네스에서 제거했다.
  - 아직 제작 예정이 없는 씬을 등록 / 확인된 씬 기준에 포함하지 않도록 정리했다.
- 마크다운 파일 끝 개행을 보정했다.
  - `PROJECT_PLAN.md`와 `WORK_ORDER.md`가 개행으로 끝나도록 수정했다.
