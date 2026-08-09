# Copilot Instructions for Thronefall (Unity 6.0.5)

## Core Architecture Rules
- Prefer composition over inheritance in gameplay and systems code.
- Separate data from logic using ScriptableObjects for configurable/static data and MonoBehaviours/services for behavior.
- Use the Strategy pattern for swappable runtime behavior (e.g., attacks, targeting, movement decisions, reactions).
- Use interfaces to define capabilities and compose behavior contracts (e.g., IDamageable, IAttackStrategy).
- Use the EventBus to broadcast cross-system/domain events to multiple listeners instead of direct coupling.

## Unity and C# Best Practices (2026, Unity 6.0.5)
- Always follow up-to-date Unity 6.0.5 and modern C# best practices when proposing or generating code.
- Keep components focused and single-responsibility.
- Minimize scene wiring friction with clear serialized references and safe defaults.
- Prefer explicit, testable seams for gameplay logic (strategy objects, interfaces, event-driven boundaries).
- Avoid introducing hidden dependencies or hard singleton coupling unless explicitly requested.

## Collaboration Requirement
- Before implementing a new feature or major refactor, validate the design with the user first.
- Provide a short proposed design (data model, behaviors, events, editor/debug plan), then wait for confirmation.
- If the user asks for immediate implementation, still include a concise design summary first and proceed.

## Communication Format Preference
- For explanations, design proposals, and research findings, prefer diagram-first output in chat.
- Use Mermaid diagrams, schemas, and flowcharts as the primary format; keep supporting text brief.
- Favor compact visual structure (flow, sequence, component, state) over long prose.

## Feature-Addition Debugging Standard
For each new feature, plan and include editor-first debugging aids:
- Add Inspector debug parameters and/or debug buttons where relevant.
- Add Gizmos helpers for visual validation when spatial logic is involved.
- Integrate quick logging hooks in ConsoleDebugger for fast verification.

After feature validation:
- Clean temporary debug noise in ConsoleDebugger.
- Keep useful diagnostics but reduce clutter by commenting out obsolete temporary logs and/or grouping debug blocks with C# regions.

## Code Change Quality Bar
- Keep public APIs and existing behavior stable unless change is requested.
- Prefer small, reversible changes with clear naming.
- Add succinct comments only where logic is non-obvious.
- Ensure generated code compiles in the current Unity project context.
