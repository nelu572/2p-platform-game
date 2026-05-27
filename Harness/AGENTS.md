# AI 개발 하네스

## 프로젝트 개요

- 프로젝트 유형: Unity 2D 게임
- 주 사용 언어: C#
- Unity 버전: 6000.0.68f1
- 프로젝트명: DUOX
- 프로젝트 의도: 2명의 플레이어가 같은 공간에서 싸우는 2D 플랫포머 액션 PVP
- 핵심 플레이: 캐릭터와 맵을 선택한 뒤 낙사, 넉백, 캐릭터별 전투 스타일로 승부한다.
- 주요 패키지: Input System, Universal Render Pipeline, UGUI, Unity Test Framework
- 등록 씬: `MainScene`, `OptionScene`, `ModSelectScene`, `CharacterSelectScene`, `Boss Select Scene`, `Map1`

## 먼저 읽을 문서 순서

1. 현재 대화의 직접 지시
2. `Harness/WORK_ORDER.md`
3. `Harness/Docs/Conventions/AI_DEVELOPMENT_RULES.md`
4. `Harness/Docs/Conventions/CODING_CONVENTIONS.md`
5. `Harness/Docs/Conventions/GITHUB_HARNESS.md`
6. `Harness/Docs/Planning/PROJECT_PLAN.md`
7. `Harness/Docs/Planning/VISUAL_HARNESS.md`
8. `Harness/Docs/Logs/FILE_CHANGE_LOG.md`

## 문서 맵

- `Harness/WORK_ORDER.md`: 반복 작업 지시서와 작업 메모 템플릿
- `Harness/Docs/Planning/PROJECT_PLAN.md`: DUOX 기획서
- `Harness/Docs/Conventions/AI_DEVELOPMENT_RULES.md`: AI 작업 우선순위와 구현 원칙
- `Harness/Docs/Conventions/CODING_CONVENTIONS.md`: C# / Unity 코드 작성 규칙
- `Harness/Docs/Conventions/GITHUB_HARNESS.md`: GitHub, 브랜치, 커밋 운영 규칙
- `Harness/Docs/Planning/VISUAL_HARNESS.md`: 화면, UX, 아트 방향 기록
- `Harness/Docs/Logs/FILE_CHANGE_LOG.md`: 작업별 파일 변경 이력

## 프로젝트 구조 요약

- `Assets/Script/Game`: 플레이어, 캐릭터 공격, 스탯, 카메라, 풀링, 게임 관리
- `Assets/Script/Game/Player`: 플레이어 입력, 상태, 차지 입력, 전투 인터페이스
- `Assets/Script/Game/Warrior`: 전사 공격과 검격
- `Assets/Script/Game/Witch`: 마녀 공격과 물약 계열
- `Assets/Script/Game/BatMan`: 방망이 캐릭터 공격
- `Assets/Script/Game/RailGun`: 레일건 캐릭터 공격
- `Assets/Script/UI`: 버튼, 선택 그룹, 패널, UI 애니메이션
- `Assets/Script/Sound`: BGM과 사운드 관리
- `Assets/Scenes`: 메인, 옵션, 준비, 맵 씬
- `Assets/Prefab`: 캐릭터, 투사체, 공용 플레이어 프리팹

## 작업 우선순위

현재 대화의 직접 지시가 `Harness/WORK_ORDER.md`보다 우선한다.

우선순위는 다음 순서를 따른다.

1. 현재 대화의 직접 프롬프트
2. `Harness/WORK_ORDER.md`
3. 하네스 문서와 프로젝트 구조

충돌하는 지시가 있으면 더 높은 우선순위를 따른다. 프로젝트 고유 정보가 확실하지 않으면 지어내지 말고 `확인 필요`로 남긴다.

## 작업 규칙

- 작업 전 관련 씬, 프리팹, 스크립트, 설정 파일을 먼저 읽는다.
- 작업 전 `PROJECT_PLAN.md`의 게임 의도와 충돌하지 않는지 확인한다.
- 기존 public API, 씬 참조, 프리팹 연결, 에셋 GUID를 불필요하게 흔들지 않는다.
- 캐릭터별 전투 스타일, 유령 시스템, 낙사/넉백 중심 구조는 핵심 설계로 취급한다.
- 로컬 2인 입력과 선택 흐름을 변경할 때는 두 플레이어의 조작 충돌을 확인한다.
- `IAttackController`, `IChargeable`, `IDamageable`, `IKnockbackable` 같은 전투 인터페이스는 기존 계약을 먼저 확인한다.
- Unity 자동 생성 파일과 메타 파일의 의미를 확인하고 수정한다.
- 명시 요청이 없는 리팩터링은 피하고, 요청된 범위 안에서 변경한다.
- 가능하면 Unity 빌드, 에디터 검증, 테스트, 정적 확인 중 수행 가능한 검증을 한다.
- 작업 완료 후 `Harness/Docs/Logs/FILE_CHANGE_LOG.md`를 갱신한다.

## 문서 처리 규칙

- 하네스 문서는 실제 작업 착수를 빠르게 하기 위한 운영 문서로 유지한다.
- 너무 긴 설명보다 현재 프로젝트에서 바로 쓸 수 있는 규칙을 우선한다.
- 프로젝트 고유 컨벤션, 로드맵, 장르, 씬 이름, 비주얼 키워드는 확인된 내용만 기록한다.
- `WORK_ORDER.md`의 템플릿 구조는 명시 요청 없이 삭제하지 않는다.
- 사용자가 "메모 작업해"라고 말하면 `Harness/WORK_ORDER.md`를 읽고 그 안의 작업을 실제로 수행한다.
