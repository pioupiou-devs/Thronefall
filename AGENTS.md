# AGENTS.md

Unity 6 project (Unity **6000.5.7f1**, URP 17.5.0, Input System 1.20.0, AI Navigation 2.0.14). Solo scene: `Assets/Scenes/GameScene.unity`.

## First things to read
- `.github/copilot-instructions.md` — authoritative architecture rules, code conventions, and collaboration workflow. Follow it.
- `Assets/Script/` — all game code. **No `.asmdef` files and no `namespace` blocks**: every class lives in the global namespace and compiles into `Assembly-CSharp` / `Assembly-CSharp-Editor`.

## Commands / verification
- No build, test, or lint commands. The `.csproj`/`.slnx` files at the repo root are **Unity-generated** — never edit them. C# is only verified by compiling in the Unity Editor (auto-import on save).
- `com.unity.test-framework` is installed but there is **no `Assets/**/Tests`** yet; don't assume tests exist.
- `.github/` has no CI workflows.

## Architecture (verified in code)
- **Identity carrier**: `Entity` (`Assets/Script/Core/Entity.cs`) is a bare `MonoBehaviour`; gameplay components require it via `[RequireComponent(typeof(Entity))]`.
- **Strategy pattern** (attacks, targeting): data = ScriptableObject (`*Data.cs`), logic = plain C# class. `Attacker.cs` wires them in `Awake()` via `Activator.CreateInstance(_strategySelector.GetType(), _strategyData)` using a `[SerializeReference]` type picker. Strategy classes must be `[Serializable]` **and** have a parameterless constructor.
- **EventBus**: generic static `EventBus<T>` with `EventBinding<T>` (`Assets/Script/Core/EventBus/`). If an event struct changes, update **all** consumers, including `Assets/Script/ConsoleDebugger.cs` (registers/deregisters in `OnEnable`/`OnDisable`).
- **State machine**: generic `StateMachine<T>`/`State<T>` (`Assets/Script/Core/StateMachine.cs`), driven by enum states (`Enemy` uses `EnemyState`, `Building` uses `BuildingState`).

## Gotchas
- `using System;` + `using UnityEngine;` makes `Object` ambiguous — use `UnityEngine.Object` fully qualified.
- Use `FindObjectsByType<T>(FindObjectsInactive.Exclude)`; `FindObjectsOfType` is deprecated in Unity 6.
- `EditorGUI.DisabledScope` only greys out UI, it does **not** block execution — guard runtime reads with `if (Application.isPlaying)`.
- `Ennemy` is intentionally spelled with double-n (`Assets/Script/Enemies/Ennemy.cs`). — **Note: renamed to `Enemy` (2026-08-15).**
- `.meta` files are committed and required — never delete them.
- **Never hand-edit `.unity` scene or `.meta` files** — make scene-level changes (adding components, wiring references) in the Unity Editor only, and let Unity regenerate `.meta` files itself. Do not modify them.
- `.vscode/settings.json` hides `.asset`/`.meta`/`.prefab`/`.unity` files from the file explorer, but they are plain YAML and remain readable.
- Custom inspectors live in `Editor/` subfolders (`*Editor.cs`). New features should include editor debug hooks, gizmos, and `ConsoleDebugger` log hooks; clean up temporary logs after validation.
- **Design-first**: when the user asks for a *design proposal*, do **not** implement it. Only write code after the user explicitly says to implement.
- **ASCII diagrams**: the user cannot render Mermaid in-chat. Prefer **ASCII/plain-text diagrams** (flow, sequence, component, state) over Mermaid for design proposals and explanations.

## Stale docs to not trust
- `.continue/memory.md` references GameManager/PlayerController/UISystem/NetworkLayer — these do **not** exist in the codebase.
- `ReadMe.md` shows an outdated/approximate file tree.
