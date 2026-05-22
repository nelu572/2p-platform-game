# Architecture

## 현재 방향

`[Project Name]`는 스마트 물류 창고 흐름을 작은 기능 단위로 구현하는 Unity 프로젝트입니다. 큰 프레임워크를 먼저 만들기보다, 컨베이어, 물류, 대기 구역, 로봇, 창고 시스템을 작은 수직 슬라이스로 누적하는 방식을 사용합니다.

## 구조 원칙

- 씬 연결 코드와 게임플레이 로직을 분리합니다.
- `MonoBehaviour`는 씬 동작, 참조 연결, Unity 라이프사이클 처리에 집중합니다.
- Unity 생명주기와 무관한 로직은 일반 C# 클래스로 분리합니다.
- Prefab 내부 참조와 씬 공용 참조를 구분합니다.
- 참조는 문자열 탐색보다 직렬화 참조와 명시적 바인딩을 우선합니다.

## 폴더 규칙

- `Assets/Scripts`: 게임플레이 C# 코드
- `Assets/Prefabs`: 컨베이어, 물류, 로봇, 창고, 대기 구역 프리팹
- `Assets/Scenes`: 작업 대상 씬과 검증용 씬
- `Assets/Materials`: 시각 구분용 머티리얼

## 권장 스크립트 하위 구조

필요할 때만 아래 폴더를 최소 단위로 추가합니다.

- `Assets/Scripts/Conveyor`
- `Assets/Scripts/Cargo`
- `Assets/Scripts/Robot`
- `Assets/Scripts/Storage`
- `Assets/Scripts/Common`

## 외부 에셋 경계

- 외부 에셋 폴더 내부에는 사용자 요청 없이 스크립트를 직접 추가하거나 구조를 바꾸지 않습니다.

## 변경 원칙

- 새 폴더, 새 패키지, 대규모 씬 재편은 필요가 명확할 때만 진행합니다.
- 작업 중인 task 범위를 넘어서는 구조 정리는 별도 요청 없이는 하지 않습니다.
