# AGENTS.md

이 문서는 `[Project Name]` 저장소에서 작업하는 AI 에이전트의 시작 문서입니다.

## 프로젝트 개요

- 장르: 스마트 물류 창고
- 엔진: Unity 6.4
- 언어: C#
- 프로젝트명: `[Project Name]`

## 문서 맵

- `AGENTS.md`: AI 에이전트용 시작 문서와 공통 작업 규칙
- `docs/ARCHITECTURE.md`: 프로젝트 구조, 폴더 규칙, 참조 철학
- `docs/STYLEGUIDE.md`: C# 및 Unity 코딩 규칙
- `docs/TESTING.md`: 검증 기준과 컴파일 확인 원칙
- `docs/HARNESS.md`: 하네스 운영과 하네스 이탈 대응 기준
- `docs/WORKFLOW.md`: task 진행 순서와 상태 관리 규칙
- `docs/PLANS.md`: 단계별 진행 현황과 작업 로그
- `docs/TASK_WRITING.md`: task 문서 작성과 구현 후 갱신 규칙의 기준 문서
- `Tasks/_template.md`: 새 task 문서 작성용 최소 복사 템플릿

## 먼저 읽을 문서

세션 첫 작업 시작 전 아래 문서를 순서대로 읽습니다.

1. `docs/WORKFLOW.md`
2. `docs/PLANS.md`

이후 같은 세션에서는 아래 상황에서 관련 문서를 기준으로 판단합니다. 이미 확인한 규칙은 다시 전체를 읽지 않고, 불확실하거나 충돌이 있을 때만 관련 섹션 또는 키워드를 부분 확인합니다. 문서 재확인은 전체 파일 출력보다 `rg`나 필요한 섹션 확인을 우선합니다.

- 구조 변경 또는 새 컴포넌트/폴더 추가 전 → `docs/ARCHITECTURE.md`
- 코드 수정 또는 새 스크립트 작성 전 → `docs/STYLEGUIDE.md`
- 검증 단계 전 → `docs/TESTING.md`
- task 시작, 전환, 상태 갱신 전 → `docs/WORKFLOW.md`, `docs/PLANS.md`
- task 문서 작성 또는 갱신 전 → `docs/TASK_WRITING.md`, `Tasks/_template.md`
- 하네스 규칙 추가, 운영 문서 조정, 에이전트 절차 이탈 또는 재발 방지 논의 전 → `docs/HARNESS.md`

작업 중 판단이 애매하면 이 `AGENTS.md`의 문서 맵을 기준으로 관련 책임 문서를 먼저 확인합니다.

## 프로젝트 의도

이 프로젝트는 스마트 물류 창고 시뮬레이션을 Unity 6.4와 C#으로 구현하면서, 하네스 엔지니어링 방식의 작업 흐름을 저장소 안 문서로 고정하는 것을 목표로 합니다.

## 하네스 운영 원칙

하네스 운영, 규칙 추가 판단, 하네스 이탈 대응 기준은 `docs/HARNESS.md`를 따릅니다.

## 에이전트 원칙

- 사용자 요청과 현재 task 범위를 벗어나는 구현, 문서 확장, 미래 확장용 추상화는 하지 않습니다.
- “있으면 좋다”, “정리된다”, “나중에 쓸 수 있다”는 이유만으로 필드, 옵션, 컴포넌트, GameObject, 문서 절차를 추가하지 않습니다.
- 요청 또는 현재 task 완료 조건에 직접 필요하지 않은 선택 기능은 구현 전에 사용자 승인을 받습니다.
- 한 번에 하나의 task만 진행합니다.
- 구현 전 관련 코드와 참조 구조를 먼저 확인합니다.
- 큰 구조 변경이나 연관 리팩터링은 별도 요청 없이 진행하지 않습니다.
- Inspector 연결, Prefab 배치, NavMesh Bake, Animator 설정처럼 Unity Editor 수동 작업이 필요하면 이를 명확히 분리해서 안내합니다.
- 사람 확인이 필요한 단계는 확인 완료 전 다음 task로 넘어가지 않습니다.
- 반복 규칙은 task마다 복제하지 말고 저장소 문서에 둡니다.
- task 문서 작성과 갱신 규칙은 `docs/TASK_WRITING.md`를 기준으로 하고, 새 task 문서는 `Tasks/_template.md` 골격을 사용합니다.

## 문서 처리 규칙

- Markdown 문서는 UTF-8로 저장합니다.
- PowerShell에서 Markdown을 읽을 때는 `Get-Content -Encoding UTF8`을 사용합니다.
- 한글 출력이 깨지면 `[Console]::OutputEncoding = [System.Text.Encoding]::UTF8`을 먼저 설정합니다.
