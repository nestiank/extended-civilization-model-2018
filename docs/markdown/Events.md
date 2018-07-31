# Events

model에서 발생하는 이벤트의 종류는 3가지로 나뉜다.

1. Fixed Event

    항상 고정된 순서대로 발생하는 이벤트이다. observable event보다 먼저 발생한다.

2. Observable Event

    `Observable<T>` 객체를 통한 이벤트이다. 우선순위가 높은 observer가 먼저 호출되며, 우선순위가 같다면 먼저 등록된 observer가 먼저 호출된다.

## Fixed Event

fixed event는 observable event보다 먼저 발생하며, `Fixed~~~` 함수를 갖는 클래스가 observer 역할을 한다.

fixed event를 받는 객체는 객체 간 포함 관계 그래프의 reverse-BFS 순서대로 호출된다.

포함 관계는 다음과 같다.

1. 플레이어
  1. 플레이어의 도시
     1. 플레이어의 도시 건물
  2. 플레이어의 타일 건물
  3. 플레이어의 유닛

## Observable Event

Observer의 우선순위는 3가지가 있다.

1. Model Core Priority

  model core에서 사용하는 우선순위이다. 가장 먼저 호출된다.

2. Model Extension Priority

  model extension에서 사용하는 우선순위이다.

3. View Priority

  view에서 사용하는 우선순이다. 가장 나중에 호출된다.
