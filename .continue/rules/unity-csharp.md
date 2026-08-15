# Unity C# Coding Guidelines

## Naming Conventions
- **Private Fields:** Use _camelCase (e.g., `_entityDiedBinding`).
- **Serialized Private Fields:** Use `[SerializeField] private int _speed;`.
- **Statics:** Use `s_StaticName`.
- **Constants:** Use `c_ConstantName`.
- **Public Properties/Methods:** Use PascalCase (e.g., `OnEnable`, `OnDisable`, `OnEntityDied`).

## Performance Guardrails
- **Avoid `GetComponent` or `Find` inside `Update()`:** These operations are costly and can lead to performance issues.
- **Ensure Events are Properly Unhandled:** Failing to deregister events can lead to memory leaks.
- **Minimize Garbage Collection Allocations:** Reuse objects where possible to reduce allocations.

## MonoBehavior Checklist
- **Initialize All Serialized Fields:** Ensure all serialized fields are properly initialized.
- **Use `OnEnable` and `OnDisable`:** Manage component activation and deactivation.
- **Avoid Expensive Operations in `Update` and `FixedUpdate`:** Offload expensive operations to other methods or use coroutines.

## Example of Following Guidelines

```csharp
using UnityEngine;

public class ConsoleDebugger : MonoBehaviour
{
    [SerializeField] private int _speed;
    private EventBinding<EntityDiedEvent> _entityDiedBinding;

    private void OnEnable()
    {
        _entityDiedBinding = new EventBinding<EntityDiedEvent>(OnEntityDied);
        EventBus<EntityDiedEvent>.Register(_entityDiedBinding);
    }

    private void OnDisable()
    {
        EventBus<EntityDiedEvent>.Deregister(_entityDiedBinding);
    }

    private void OnEntityDied(EntityDiedEvent eventData)
    {
        string entityName = eventData.Source != null ? eventData.Source.name : "Unknown";
        Debug.Log($"[ConsoleDebugger] Entity died: {entityName}");
    }
}
```