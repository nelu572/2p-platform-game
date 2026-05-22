# Style Guide

## C# 규칙

- 공개 타입, 메서드, 프로퍼티는 `PascalCase`를 사용합니다.
- private 직렬화 필드는 `_camelCase`를 사용합니다.
- 지역 변수와 파라미터는 `camelCase`를 사용합니다.
- Inspector 노출이 필요할 때는 `public` 필드보다 `[SerializeField] private`를 우선합니다.
- 외부에서 읽기만 필요한 값은 `private` + getter 또는 메서드로 제공합니다.
- bool 상태값은 기본적으로 `private`으로 두고 필요 시 읽기 전용 접근만 제공합니다.

## Unity 규칙

- `MonoBehaviour`는 씬 동작과 Unity 라이프사이클 책임에 집중합니다.
- 게임플레이 로직은 가능하면 일반 C# 클래스와 분리합니다.
- Script Execution Order에 의존하는 암묵적 초기화는 피합니다.
- Prefab, Animator, Collider, Rigidbody 설정으로 해결할 수 있는 문제를 코드로 과도하게 우회하지 않습니다.
- 조정 가능한 값은 매직 넘버로 흩뿌리지 말고 직렬화 필드 또는 명명된 상수로 둡니다.

## 금지 또는 제한 규칙

- `Resources.Load`를 기본 자원 접근 방식으로 사용하지 않습니다.
- `GameObject.Find`, `GameObject.FindWithTag`, `FindAnyObjectByType`, `FindFirstObjectByType`, `FindObjectOfType`를 기본 참조 방식으로 사용하지 않습니다.
- `GetComponent`, `TryGetComponent`를 `Update()`나 반복 루프에서 매번 호출하지 않습니다.
- 이름, 태그, 경로 문자열에 강하게 의존하는 탐색 로직을 기본 구조로 사용하지 않습니다.
- 명확한 필요와 생명주기 책임 없이 새 싱글톤을 도입하지 않습니다.

## 주석 규칙

- 초보자이기 때문에 기본적으로 주석은 달아줍니다.
- 주석은 기본적으로 한글로 작성합니다.
- Inspector 수동 연결 포인트, 상태 전이 이유, 라이프사이클 의도처럼 코드만으로 바로 드러나지 않는 맥락을 우선 기록합니다.

## 중복 구현 방지

- 새 변수나 메서드를 추가하기 전에 기존 동일 책임 코드가 있는지 먼저 확인합니다.
- 기존 매니저, 컴포넌트, 유틸리티, ScriptableObject를 우선 재사용합니다.
