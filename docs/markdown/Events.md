# Events

model에서 발생하는 이벤트의 종류는 2가지로 나뉜다.

1. Fixed Event

    항상 고정된 순서대로 고정된 객체에서 발생하는 이벤트이다. observable event보다 먼저 발생한다.

    model core에서 이벤트를 사용해야 할 경우 이 종류의 이벤트를 사용한다.

    몇몇 이벤트는 메서드를 override하는 방식으로 model extension에서도 사용 가능하다.

2. Observable Event

    observer 객체를 통해 notify 받을 수 있는 이벤트이다.

    우선순위가 높은 observer가 먼저 호출되며, 우선순위가 같다면 먼저 등록된 observer가 먼저 호출된다.

# Fixed Event

fixed event는 observable event보다 먼저 발생한다.

fixed event를 받는 객체는 객체 간 포함 관계 그래프의 forward/reverse DFS 순서대로 호출된다.

forward/reverse 여부는 각 이벤트마다 다르다.

포함 관계는 다음과 같다.

1. 플레이어
  1. 플레이어의 도시
     1. 플레이어의 도시 건물
  2. 플레이어의 타일 건물
  3. 플레이어의 유닛
     1. 플레이어의 `Actor`에 적용되고 있는 `Effect` (유닛 뿐만 아니라 다른 `Actor`도 이를 포함하고 있음.)
  4. 플레이어의 퀘스트

# Observable Event

Observer의 우선순위는 2가지가 있다.

1. Model Priority

  model extension에서 사용하는 우선순위이다.

2. View Priority

  view에서 사용하는 우선순이다. 가장 나중에 호출된다.

# List of Event

- Turn Event *(fixed & observable)*

  - Pre Turn *(forward DFS)*

     Turn이 시작될 때 발생한다.

  - After Pre Turn *(forward DFS)*

     `Pre Turn` 이벤트가 발생한 후에 발생한다.

  - Post Turn *(backward DFS)*

     Turn이 끝날 때 발생한다.

  - Before Post Turn *(backward DFS)*

     `Post Turn` 이벤트가 발생하기 전에 발생한다.

  - Pre SubTurn *(forward DFS)*

     SubTurn이 시작될 때 발생한다.

  - After Pre SubTurn *(forward DFS)*

     `Pre SubTurn` 이벤트가 발생한 후에 발생한다.

  - Post SubTurn *(backward DFS)*

     SubTurn이 끝날 때 발생한다.

  - Before Post SubTurn *(backward DFS)*

     `Post SubTurn` 이벤트가 발생하기 전에 발생한다.

- Production Event *(observable)*
  - OnProductionDeploy

     생산 결과물이 배치될 때 발생한다.

- Tile Object Event *(observable)*
  - TileObject Produced

     Tile Object가 생산되었을 때 발생한다.

  - TileObject Placed

     Tile Object가 타일에 배치되거나 배치된 타일이 변경되었을 때 발생한다.

- Battle Event *(observable)*
  - Before Battle

     전투가 벌어지기 전 발생한다.

  - After Battle

    전투가 벌어진 후 발생한다.

- Quest Event *(observable)*
  - Quest Accepted

     퀘스트가 accept 되었을 때 발생한다.

  - Quest Givenup

     accept된 퀘스트가 포기되었을 때 발생한다.

  - Quest Completed

     accept된 퀘스트가 완료되었을 때 발생한다.

- Victory Event *(observable)*
  - victory

     어떤 플레이어가 승리했을 때 발생한다.

  - defeat

     어떤 플레이어가 패배했을 때 발생한다.

  - draw

     어떤 플레이어가 무승부를 냈을 대 발생한다.
