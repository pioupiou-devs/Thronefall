using UnityEngine;

public class ConsoleDebugger : MonoBehaviour
{
	private EventBinding<EntityDiedEvent> entityDiedBinding;

	private void OnEnable()
	{
		entityDiedBinding = new EventBinding<EntityDiedEvent>(OnEntityDied);
		EventBus<EntityDiedEvent>.Register(entityDiedBinding);
	}

	private void OnDisable()
	{
		EventBus<EntityDiedEvent>.Deregister(entityDiedBinding);
	}

	private void OnEntityDied(EntityDiedEvent eventData)
	{
		string entityName = eventData.Source != null ? eventData.Source.name : "Unknown";
		Debug.Log($"[ConsoleDebugger] Entity died: {entityName}");
	}
}