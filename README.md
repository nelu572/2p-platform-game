# DUOX

DUOX는 Unity 기반 2인 로컬 플랫포머 액션 PVP 게임이다.

플레이어는 캐릭터와 맵을 선택한 뒤 전투를 진행하며, 낙사와 넉백을 중심으로 승부한다. 프로젝트의 세부 기획과 AI 작업 규칙은 `Harness` 폴더의 하네스 문서를 기준으로 관리한다.

## AI 개발 하네스

이 프로젝트는 AI가 작업을 시작할 때 필요한 정보를 빠르게 확인할 수 있도록 `Harness` 폴더에 운영 문서를 둔다.

AI 작업자는 먼저 다음 문서를 확인한다.

1. 현재 대화의 직접 지시
2. `Harness/WORK_ORDER.md`
3. `Harness/AGENTS.md`
4. `Harness/Docs/Conventions/AI_DEVELOPMENT_RULES.md`
5. `Harness/Docs/Conventions/CODING_CONVENTIONS.md`
6. `Harness/Docs/Conventions/GITHUB_HARNESS.md`
7. `Harness/Docs/Planning/PROJECT_PLAN.md`
8. `Harness/Docs/Planning/VISUAL_HARNESS.md`
9. `Harness/Docs/Logs/FILE_CHANGE_LOG.md`

현재 대화의 직접 지시가 `Harness/WORK_ORDER.md`보다 우선한다.

## 작업 지시서 사용법

반복적으로 AI에게 맡길 작업은 `Harness/WORK_ORDER.md`에 작성한다.

작성 위치:

- `작업 이름`: 이번 작업을 짧게 적는다.
- `상세 요구사항`: 실제 수행할 내용을 구체적으로 적는다.
- `유저 메모장`: 참고용 메모만 적는다. 작업 대상으로 해석하지 않는다.

작업 지시서를 작성한 뒤 AI에게 `메모 작업해`라고 말하면, AI는 `Harness/WORK_ORDER.md`를 읽고 그 안의 작업을 수행한다.

## 작업 지시서 커밋 규칙

`Harness/WORK_ORDER.md`는 작업을 전달하기 위한 임시 지시서다.

- 실제 작업 지시 내용은 커밋하지 않는다.
- GitHub에 올릴 때는 `작업 이름`, `상세 요구사항`, `유저 메모장`에 지정된 주석만 남긴다.
- 작업 지시가 없을 때도 세 템플릿 섹션의 주석은 지우지 않는다.
- `사용 방법`과 `주의사항`은 유지한다.

## 변경 이력 관리

작업 후에는 `Harness/Docs/Logs/FILE_CHANGE_LOG.md`를 갱신한다.

- `현재 커밋 안 된 작업`은 자동으로 매번 갱신하지 않는다.
- 사용자가 해당 섹션 작성을 요청했을 때만 아직 커밋하지 않은 변경을 적는다.
- 커밋할 때는 `현재 커밋 안 된 작업` 섹션의 실제 작업 내용은 비우고 지정된 주석만 남긴다.
- 완료된 작업은 날짜별 섹션에 남긴다.

## 주요 하네스 문서

- `Harness/AGENTS.md`: AI가 먼저 읽는 운영 안내서
- `Harness/WORK_ORDER.md`: 반복 작업 지시서
- `Harness/Docs/Planning/PROJECT_PLAN.md`: DUOX 기획서
- `Harness/Docs/Planning/VISUAL_HARNESS.md`: 화면 / UX / 아트 기준
- `Harness/Docs/Conventions/AI_DEVELOPMENT_RULES.md`: AI 작업 원칙
- `Harness/Docs/Conventions/CODING_CONVENTIONS.md`: 코드 작성 규칙
- `Harness/Docs/Conventions/GITHUB_HARNESS.md`: GitHub 작업 규칙
- `Harness/Docs/Logs/FILE_CHANGE_LOG.md`: 파일 변경 이력
