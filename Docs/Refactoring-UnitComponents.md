# Player / Enemy 공용 유닛 컴포넌트 리팩토링

- 브랜치: `Develop`
- 관련 커밋: `ac9d032` (Player/Enemy 공용 유닛 컴포넌트로 리팩토링)
- Unity 6000.3.10f1

## 1. 배경

Player와 Enemy는 체력, 이동, 애니메이션, 공격이라는 같은 기능을 쓰지만 컴포넌트가 따로 있었다.

| 기능 | Player | Enemy |
|---|---|---|
| 체력 | `Player` 안에 직접 구현 (`currentHp`, `TakeDamage`, `Heal`) | `UnitHealth` |
| 이동 | `PlayerController`가 Rigidbody를 직접 조작 | `UnitController` |
| 애니메이션 | `PlayerAnimController` | `UnitAnimController` |
| 공격 | `PlayerAttack` | `UnitMeleeAtack`, `UnitRangeAttack` |

그 결과 같은 로직(체력 관리, 이동, 쿨다운, SFX 재생)이 여러 곳에 중복되었고,
`Player`와 `Enemy`가 서로 다른 API를 노출해 `HealEffect`, `HpBarController` 같은 공용 기능이 `Player` 타입에 묶여 있었다.

## 2. 목표와 원칙

- Player와 Enemy가 **같은 컴포넌트**를 사용하도록 통합한다.
- **게임 동작은 유지**한다. 의도한 수정만 4절에 따로 적는다.
- Unity의 직렬화를 깨지 않는다.
  - 기존 스크립트 GUID를 유지한다. 프리팹은 GUID로 스크립트를 참조한다.
  - 애니메이션 이벤트가 문자열로 호출하는 메서드 이름을 유지한다.

## 3. 구조

```
Unit (abstract) ─ UnitHealth / UnitController / UnitAnimController / UnitAttack 캐싱 + 위임 메서드
 ├─ Player
 └─ Enemy (abstract, IPoolable)
     ├─ MeleeEnemy   (FSM)
     └─ RangeEnemy   (Behavior Tree)

UnitAttack (abstract)
 ├─ UnitMeleeAttack   근접 (OverlapBox)
 ├─ UnitRangeAttack   원거리 (Bullet 풀링)
 └─ PlayerAttack      플레이어 무기 (Raycast 히트스캔 + 탄약/재장전)

UnitAnimController
 └─ PlayerAnimController   플레이어 전용 (이동 블렌딩, 재장전, 무기별 애니메이션)
```

### 3.1 공용 컴포넌트

| 컴포넌트 | 역할 | 이번에 추가한 것 |
|---|---|---|
| `Unit` (신규) | 컴포넌트 캐싱과 위임 메서드 (`MoveToward`, `StopMoving`, `Rotate`, `SetMoveState`, `AttackTrigger`, `DeathTrigger`), `Position`, `Health` | 전체 |
| `UnitHealth` | 체력, 피해, 사망 | `Heal`, `MaxHp`, `OnHpChanged`(현재/최대 체력), SFX 미설정 시에도 동작 |
| `UnitController` | Rigidbody 기반 이동/회전 | `Move(방향, 속도)`, `LookAt(지점)`. `MoveToward`는 이 둘 위에서 동작 |
| `UnitAnimController` | 공통 애니메이션 파라미터 | `MoveParam`, `AttackParam` override 지점 |
| `UnitAttack` (신규) | 공격 공통 부분: 스탯(`IAttackable`), `TargetLayerMask`, 쿨다운(`IsCoolingDown`), SFX | 전체 |
| `AudioClipListExtensions` (신규) | 목록에서 무작위 클립 선택 (`PickRandom`) | 전체 |

### 3.2 Player 쪽

- `Player`: `Unit`을 상속한다. 스탯 설정과 사망 처리(입력 비활성화, GameOver 전환)만 남겼다. 피해 처리는 `UnitHealth`가 한다.
- `PlayerController`: 입력(이동, 조준, 재장전)만 처리하고 실제 이동/회전은 `UnitController`에 위임한다.
- `PlayerAttack`: `UnitAttack`을 상속한다. 히트스캔, 탄약, 재장전을 담당한다.
- `PlayerAnimController`: `UnitAnimController`를 상속해 `IsMoved`, `IsShoot` 파라미터를 override하고, 이동 블렌딩, 재장전, 무기별 애니메이션만 추가한다.
- `WeaponManager`: 기능 변경 없이 정리했다.

### 3.3 Enemy 쪽

`Enemy`가 종류와 무관한 로직을 공통으로 처리한다.

- 스탯 로드: 하위 클래스는 `StatTableKey`만 제공한다. (`"Melee_Enemy_TB"`, `"Range_Enemy_TB"`)
- 공격 컴포넌트 스탯 적용: `AttackDamage`, `AttackRange`, `AttackDelay`, `TargetLayerMask`, SFX
- 풀링: `OnSpawn`(초기화와 이벤트 구독), `OnDespawn`(구독 해제)
- 사망: `HandleDeath()`(종류별 연출, abstract) 호출 후 점수 가산과 `OnAnyEnemyDeath` 알림
- 피격: `HandleDamaged()`
- 풀 반환: `ScheduleRelease()` (5초 후)
- 레이어 마스크 폴백: 미할당이면 `Player` / `Environment`

`MeleeEnemy`, `RangeEnemy`에는 종류별 AI와 사망 연출만 남았다.

| 클래스 | 이전 | 이후 | 남은 내용 |
|---|---|---|---|
| `MeleeEnemy` | 97줄 | 약 50줄 | FSM 구성, `Attack()`(애니메이션 이벤트) |
| `RangeEnemy` | 136줄 | 약 80줄 | Behavior Tree 구성, `rangeAttack.Initialize` |

## 4. 동작이 바뀐 부분 (의도한 수정)

| 항목 | 변경 |
|---|---|
| `MoveToward` | y를 0으로 만든 뒤 정규화한다. 목표와 높이 차이가 있을 때 이동 속도가 줄던 문제를 없앴다. |
| Player 중복 사망 | `UnitHealth`의 `isDead` 가드로 사망 처리가 한 번만 일어난다. |
| 재장전 이벤트 | 재장전 완료 시 `OnAmmoChanged`가 두 번 호출되던 것을 한 번으로 줄였다. |
| `CancelReload()` | 재장전 중이 아닐 때 호출해도 `OnReloadFailed`가 발생하지 않는다. |
| HP바 | 회복(`Heal`) 시에도 갱신된다. 이전에는 피해만 반영했다. |
| 무기 교체 | `WeaponManager.WeaponData`가 이벤트 발생 전에 새 무기로 갱신된다. |
| `UnitRangeAttack` | 스폰마다 `BulletData` ScriptableObject를 새로 만들던 누수를 고쳤다. |

## 5. 그 밖의 정리

- `HpBarController`: 부모 계층의 `UnitHealth`를 직접 구독한다. static 이벤트 `PlayerManager.GetPlayerObjAfterSpawned`를 제거했고, 사용하지 않던 `currentFill`, `speed` 필드를 삭제했다.
- `HealEffect`: `Player.Heal` 대신 `UnitHealth.Heal`을 사용한다. (Enemy에도 적용 가능) 이 파일은 아이템/이펙트 시스템(미커밋 작업)에 속해 이번 커밋에는 포함되지 않았다.
- BT 리프 노드(`AttackAction`, `CheckAttackDelay`): 특정 `RangeEnemy` 대신 `Enemy` 타입을 참조한다.
- FSM: `EnemyAttackState`는 `enemy.Rotate()`를 사용하고, `EnemyDeadState`는 공통 `ScheduleRelease()`를 사용한다. `EnemyIdleState`의 필드 오타를 수정했다(`detecInterval` → `DetectInterval`).
- 클래스명 오타 수정: `UnitMeleeAtack` → `UnitMeleeAttack` (파일명과 일치)

## 6. 프리팹 변경

- `Player.prefab`
  - `UnitHealth`, `UnitController` 컴포넌트를 추가했다.
  - `Player` 컴포넌트에서 더 이상 쓰지 않는 `currentHp` 직렬화 필드를 제거했다.
  - 기존 컴포넌트(`Player`, `PlayerController`, `PlayerAnimController`, `PlayerAttack`, `WeaponManager`)의 GUID와 값은 그대로다.
- `Melee_Skeleton_Enemy.prefab`: 클래스명 표기(`m_EditorClassIdentifier`)만 갱신했다.
- Enemy 프리팹의 GUID 참조는 모두 유지된다. `UnitRangeAttack`이 더 이상 직렬화하지 않는 필드(`targetLayerMask`)나 `RangeEnemy.rangeAttack`이 남아 있어도 Unity가 무시한다.

## 7. 유지보수 시 주의사항

- **애니메이션 이벤트 이름**: `PlayAttack`(원거리, `UnitRangeAttack`)과 `Attack`(근접, `MeleeEnemy`)은 `.anim` 파일이 문자열로 호출한다. 이름을 바꾸면 이벤트가 끊긴다.
- **`UnitHealth.Initialize`는 이벤트 구독자를 모두 지운다.** 풀링 시 이전 구독자를 남기지 않으려는 의도다.
  - 구독은 `Initialize` 뒤에 해야 한다. `HpBarController`는 이 때문에 `Awake`가 아닌 `Start`에서 구독한다.
- **Awake 실행 순서**: 같은 오브젝트의 컴포넌트 간 `Awake` 순서는 보장되지 않는다. `UnitController.Awake`(Rigidbody 캐싱)에 의존하는 호출(예: `controller.Initialize()`)을 다른 컴포넌트의 `Awake`에서 하면 안 된다. `Player.Awake`가 그 호출을 하지 않는 이유다.
- **`PlayerAnimController`의 파라미터**: `UnitAnimController`의 `IsAttack`, `OnMove`는 Enemy 애니메이터의 파라미터다. Player 애니메이터에는 없으므로 `MoveParam`, `AttackParam`을 override해서 쓴다. Player에서 `TakeDamaged()`, `DeathTrigger()`를 호출하면 파라미터 없음 경고가 난다.
- **입력 기반 이동**: `PlayerController`가 꺼지면(사망 등) `OnDisable`에서 `StopMoving()`을 호출한다. `UnitController`가 마지막 이동 방향을 유지하기 때문이다.

## 8. 검증

배치 모드 컴파일과 에디터 Play 모드에서 확인했다.

| 항목 | 결과 |
|---|---|
| 컴파일 | 에러 없음 (기존 `ItemData.maxStack` 경고만 존재) |
| 프리팹 | Player, Melee, Range 모두 Missing script 없음, 필수 컴포넌트 충족 |
| Player 스폰과 HP바 | HP바가 `UnitHealth`에 연결되어 피해와 회복 모두 반영 |
| 이동 | 이동 속도가 10으로 측정되고 `Move(zero)` 후 정지 |
| 히트스캔 사격 | 적 HP 50→35, 탄약 감소, `OnDamaged` 이벤트 1회, 쿨다운 시작 |
| 재장전 | R 키 경로로 탄약 4→30 |
| 적 사망 | 이벤트와 점수 1회만 반영, 죽은 적에 대한 추가 피해는 무시 |
| 풀 반환과 재스폰 | 5초 후 반환, 재사용 시 HP 복구, 사망 구독 1개 유지 |
| 근접/원거리 공격 | 종류별로 분리해 테스트했고 둘 다 Player에 피해를 줌 |
| Player 사망 | GameOver 전환, `timeScale` 0, `PlayerController`/`PlayerAttack` 비활성화, 이동 정지 |
| 콘솔 | 에러/경고 0건 |

## 9. 이번에 하지 않은 것

- 폴더 이름 오타(`Prefeb`, `Prefebs`)는 로딩 경로에 영향을 줄 수 있어 그대로 두었다.
- 아이템/이펙트 시스템, 씬, 패키지 등 이번 리팩토링과 무관한 작업 중 변경은 커밋하지 않았다.
- FSM 상태 클래스(`EnemyIdleState` 등)의 제네릭 타입은 `MeleeEnemy` 기준을 유지했다.
