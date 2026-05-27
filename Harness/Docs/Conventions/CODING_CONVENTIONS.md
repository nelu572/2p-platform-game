# 코딩 컨벤션 하네스

## 프로젝트 기준

- 프로젝트명: DUOX
- 장르: 2인 로컬 플랫포머 액션 PVP
- 주요 시스템: 캐릭터 선택, 맵 선택, 낙사, 넉백, 유령 개입, 캐릭터별 공격 방식
- 도구: Unity, Aseprite
- 주요 Unity 패키지: Input System, Universal Render Pipeline, UGUI, Unity Test Framework
- 외부/서드파티 사용 흔적: DOTween

## 네이밍 컨벤션

| 대상 | 규칙 | 예시 |
| --- | --- | --- |
| 클래스 / 타입 | 파스칼 케이스 | `PlayerController` |
| 인터페이스 | `I` + 파스칼 케이스 | `IDamageable` |
| 로컬 변수 / 매개변수 | 카멜 케이스 | `moveSpeed` |
| private 필드 | `_camelCase` | `_rigidbody` |
| 프로퍼티 | 파스칼 케이스 | `MoveSpeed` |
| 메서드 | 파스칼 케이스, 동사 우선 | `Move()` |
| Unity 기본 함수 | Unity 기본 이름 유지 | `Start()`, `Update()` |
| 이벤트 변수 | 파스칼 케이스 | `PlayerDied` |
| 이벤트 함수 | `On` + 파스칼 케이스 | `OnPlayerDied()` |
| 파일명 | 클래스명과 동일 | `PlayerController.cs` |

## 한글 우선 규칙

- 오브젝트 명, 변수명 등은 영어를 사용한다.
- 에디터와 인스펙터에 보이는 요소는 한글을 우선한다.
- 디버그 오버레이, 로그, 테스트용 UI는 한글을 우선한다.
- 문서와 작업 템플릿은 한글을 우선한다.
- `FILE_CHANGE_LOG.md`는 `현재 커밋 안 된 작업`과 날짜별 작업 로그 형식을 유지한다.
- `현재 커밋 안 된 작업` 섹션은 사용자가 요청했을 때만 작성하고, 일반 작업 완료 시 자동 갱신하지 않는다.

## 주석 규칙

- 클래스, 메서드, 프로퍼티처럼 큰 선언부 설명은 XML 문서 주석을 우선한다.
- 짧은 보조 설명은 일반 주석을 사용한다.
- 불필요한 설명 주석은 피한다.

## 구조 규칙

- 함수가 길어지면 역할별로 분리한다.
- 매직 넘버는 가능한 한 설정값이나 데이터로 뺀다.
- 기존 시스템과 겹치는 새 매니저를 만들기 전에 재사용 가능성을 먼저 검토한다.
- 테스트 가능한 최소 단위부터 만든다.
- 캐릭터 공통 동작은 `Assets/Script/Game/Player`와 인터페이스 구조를 우선 검토한다.
- 캐릭터 고유 공격과 스탯은 캐릭터별 폴더와 ScriptableObject 구조를 우선한다.
- UI 관련 동작은 `Assets/Script/UI`, 사운드 관련 동작은 `Assets/Script/Sound` 책임 범위를 따른다.
- 씬 흐름은 메인, 캐릭터 선택, 맵 선택, 전투 씬의 연결을 고려해 수정한다.

## 현재 코드 구조 기준

- `IAttackController`: 캐릭터 공격 실행 계약
- `IChargeable`: 차징형 캐릭터 계약
- `IDamageable`: 피해 처리 계약
- `IKnockbackable`: 넉백 처리 계약
- `CharacterStatData`와 캐릭터별 `*StatData.asset`: 캐릭터 스탯 데이터
- `ObjectPoolManager`와 `PooledObject`: 투사체나 반복 생성 오브젝트 풀링 구조
- `UISelectionGroup`, `UIButton`, `UIInput`: 선택 UI 입력과 상태 처리 구조
