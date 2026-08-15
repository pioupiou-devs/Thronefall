# Thronefall Project — Progress Tracker

> Living document comparing the project to the reference game **Thronefall** (Grizzly Games)
> and tracking advancement. Update after each feature lands.

## Reference: Thronefall features (research, Aug 2026)

Core loop = **day/night**: build kingdom by day (spend gold), defend from night waves as a
warrior king on horseback. Win by surviving all waves; lose if the castle is razed.

- **Economy buildings** (income each morning): Houses, Mills, Gold Mines, Harbors, Fields.
- **Defense buildings**: Towers, Walls, Barricades, Shrines.
- **Military buildings**: Barracks, Archery Ranges, Hero's Quarters → recruit units.
- **Buildings are upgradable** with branching specializations (Tower → Castle/Sniper/Armored/Bunker;
  Spire → Archer's/Ballistic/Fire/Healing).
- **Units**: Knights, Berserks, Spearmen, Crossbowmen, Longbow/Fire Archers, Flails, Golems, Mages,
  Hunters — melee/ranged/support/hero with anti-X specializations.
- **King combat**: 9 unlockable weapons (bow+dagger default → AoE, chain lightning), passive attack +
  active ability w/ cooldown, dash at full HP, respawn on death.
- **Perks** (≤5) passive modifiers; **mutators** (optional) difficulty increases for bonus XP/score.
- **10 campaign maps** + 27 challenge variants + endless roguelike "Eternal Trials".
- **Gameplay twists**: player controls *when* night falls; destroyed buildings/units auto-rebuild free
  next morning (no income if destroyed); predetermined build slots per map; enemy waves/future
  directions telegraphed before night.

## Current project state (Aug 2026)

Systems in code (all in global namespace / `Assembly-CSharp`, no asmdefs):
- `Entity` — bare identity MonoBehaviour.
- `Health` / `IDamageable` / `Damage` — HP, death, raises `EntityDiedEvent`.
- `EventBus<T>` / `EventBinding<T>` — cross-system events.
- `StateMachine<T>` / `State<T>` — generic state machine.
- `Targeting` (strategy) — finds a target (closest entity).
- `Attack` (strategy) — melee, range, cooldown; `IsInRange`/`TryAttack`.
- `Enemy` (`Enemy`) — Search/Move/Attack/Dead state machine + NavMesh movement.
- `Player` — movement only (Rigidbody + Input System), no combat yet.
- `Building` — Idle/Attack/Broken state machine.
- `ConsoleDebugger` — entity-death + wave-start log hooks.
- `Faction` — team enum (**Enemy / Neutral / Player**) on `Entity`; configurable in Inspector,
  defaulted via `Reset()` per type (`Player`→Player, `Building`→Player [player's buildings],
  `Enemy`→Enemy).
- `Targeting` is now **faction-aware** — `ClosestTargetStrategyData.targetFaction` selects which
  faction to hunt; `ClosestTargetStrategy` filters candidates by it and skips **dead** entities.
- `Health` — added `destroyOnDeath` flag: units (enemies) are destroyed on death, while
  buildings/player stay (dead buildings/units are also no longer targeted).
- `Enemy` (renamed from `Ennemy`) — on killing its current target it refreshes targeting so it
  picks the next living target instead of standing over the corpse.
- `Enemy`/`Building` — expose read-only `CurrentState` (state-machine state).
- `WaveStartEvent` + `EnemySpawner` — spawns `count` enemies round-robin across spawn points
  on `WaveStartEvent`; prefab-based; editor "Spawn Wave" debug button + spawn-point gizmo.
- `ConsoleDebugger` — logs `EntityDiedEvent`/`WaveStartEvent`; editor has "Raise WaveStartEvent" /
  "Raise EntityDiedEvent" trigger buttons for testing.
- `GamePhase` enum (`Day / Night / Victory / Defeat`).
- `Building` — `isProtected` flag (serialized, `DefaultProtected`/`Reset()`); any number of buildings can be flagged.
- `WaveClearedEvent` + `GameOverEvent` — wave-clear and win/lose broadcast events.
- `EnemySpawner` — now tracks spawned-alive enemies; raises `WaveClearedEvent` when the last one dies.
- `DayNightController` — phase state machine: "Start Night" (`StartNight` Input action or editor
  button) → Night, raises `WaveStartEvent`; on `WaveClearedEvent` → Day (or Victory on last wave);
  on any `IsProtected` building death → Defeat; exposes read-only `CurrentPhase`/`CurrentWave` +
  editor debug buttons ("Start Night" / "Force Day").

Scene: Player, one enemy "Cube", one Building placeholder, ground/lighting. No economy or night yet
(user wires `DayNightController` in the Editor).

## Gap analysis vs Thronefall

| Feature | Status | Gap |
|---|---|---|
| King (player) combat | Player moves only | Big |
| Day/Night + waves | None | Big (core loop) |
| Economy (gold) | None | Big (core loop) |
| Buildings (towers) | Idle/attack/broken | Medium (needs placement) |
| Enemies (waves/types) | Spawner + one melee type | Medium |
| Allied units / recruiting | None | Big |
| Building upgrades/specs | None | Medium |
| Walls / chokepoints | None | Medium |
| Faction targeting | Now added | Small |
| Perks/mutators/XP/weapons | None | Later |
| Maps / campaign | 1 scene | Later |

## Roadmap (phased)

> Phase 1 goal: a simple tower-defense (build + defend against waves, lose if the
> protected building is razed). No player combat until phase 5.

### Phase 1 — simple tower defense
- [x] **Faction targeting** — team enum on `Entity` + faction-aware `ClosestTargetStrategy` so towers/enemies don't hit allies.
- [x] **Spawner + waves** — spawn X enemies on `WaveStartEvent`.
- [x] **Day/night system** — manual "start night" trigger (Input action or button); toggle between build (day) and defend (night).
- [x] **Wave system** — trigger waves on night-start; count/clear waves (`WaveClearedEvent`); win condition (survive all waves → Victory).
- [ ] **Building placement** — place defense buildings at map slots; build cost (basic).
- [x] **Protected building / lose condition** — `isProtected` flag on `Building` (multiple supported); if any protected building is razed → Defeat.

### Phase 2 — RTS part
- [ ] **Troop management/placement** — recruit and position allied units on the map.

### Phase 3 — economy & upgrades
- [ ] **Money balance** — gold/treasury + methods to add/remove money; income.
- [ ] **Upgrade system** — spend money to raise a building's level (basic level system).

### Phase 4 — UI / menus
- [ ] UI/menus (build, upgrade, start night, victory/defeat screens, HUD).

### Phase 5 — improve existing features
- [ ] More enemy types, building types, troop types.
- [ ] **Player combat** — give the player a basic attack (reuse `Attack` strategy).

### Phase 6 — configurability
- [ ] Configurable map setup, level goals/constraints.

### Phase 7 — later cleanup
- [ ] Code cleanup, refactors, perf, polish.

## Log
- **2026-08-15** — Added `Faction` enum on `Entity` (Enemy/Neutral/Player; buildings = Player) defaulted via `Reset()` per type; added `Player : Entity` subclass.
- **2026-08-15** — Added `WaveStartEvent` + `EnemySpawner` (round-robin spawn of X enemies on wave start; prefab-based; editor debug button + gizmo).
- **2026-08-15** — Faction-aware targeting (Option A): `ClosestTargetStrategyData.targetFaction` + filter in `ClosestTargetStrategy`; `ConsoleDebugger` event trigger buttons (WaveStart/EntityDied).
- **2026-08-15** — Targeting skips dead entities (enemy now moves on after killing a building); added `Health.destroyOnDeath` (units vanish, buildings/player persist).
- **2026-08-15** — Renamed `Ennemy` → `Enemy` (file/class/refs, GUID preserved so prefab keeps working); enemy re-targets after killing its target; `Enemy`/`Building` expose read-only `CurrentState`.
- **2026-08-15** — Building: Idle/Attack/Broken state machine implemented.
- **2026-08-15** — Fixed enemy-not-attacking bug (use `args:` named param in `Activator.CreateInstance`).
- **2026-08-15** — Day/night + wave system: `GamePhase` enum, `WaveClearedEvent`/`GameOverEvent`, `DayNightController` (Start Night action/button → Night, WaveCleared → Day/Victory, protected building death → Defeat; editor debug buttons), `EnemySpawner` now tracks alive enemies and raises `WaveClearedEvent`; `ConsoleDebugger` logs + trigger buttons for the new events.
