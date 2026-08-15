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

Scene: Player, one enemy "Cube", one Building placeholder, ground/lighting. No economy, waves, or night.

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
| Faction targeting | Now added | Small || Perks/mutators/XP/weapons | None | Later |
| Maps / campaign | 1 scene | Later |

## Roadmap / next steps

### Phase 1 — make what exists playable
- [x] **Faction targeting** — team enum on `Entity` + faction-aware `ClosestTargetStrategy` so towers/enemies don't hit allies.
- [ ] **King combat** — give player a basic attack (reuse `Attack` strategy).
- [x] **Spawner + waves** — spawn X enemies on `WaveStartEvent`.

### Phase 2 — day/night economy loop
- [ ] Gold/treasury + income from surviving buildings.
- [ ] Building placement at map slots + build costs.
- [ ] Allied units (recruit from barracks, command placement).
- [ ] Walls / barricades.

### Phase 3 — depth
- [ ] Building upgrade branches, more enemy/unit types, perks/mutators, multiple maps.

## Log
- **2026-08-15** — Added `Faction` enum on `Entity` (Enemy/Neutral/Player; buildings = Player) defaulted via `Reset()` per type; added `Player : Entity` subclass.
- **2026-08-15** — Added `WaveStartEvent` + `EnemySpawner` (round-robin spawn of X enemies on wave start; prefab-based; editor debug button + gizmo).
- **2026-08-15** — Faction-aware targeting (Option A): `ClosestTargetStrategyData.targetFaction` + filter in `ClosestTargetStrategy`; `ConsoleDebugger` event trigger buttons (WaveStart/EntityDied).
- **2026-08-15** — Targeting skips dead entities (enemy now moves on after killing a building); added `Health.destroyOnDeath` (units vanish, buildings/player persist).
- **2026-08-15** — Renamed `Ennemy` → `Enemy` (file/class/refs, GUID preserved so prefab keeps working); enemy re-targets after killing its target; `Enemy`/`Building` expose read-only `CurrentState`.
- **2026-08-15** — Building: Idle/Attack/Broken state machine implemented.
- **2026-08-15** — Fixed enemy-not-attacking bug (use `args:` named param in `Activator.CreateInstance`).
