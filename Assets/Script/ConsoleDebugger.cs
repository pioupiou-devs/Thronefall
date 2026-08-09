using UnityEngine;
using CityBuilder.EventBus;

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
		var entityComponent = eventData.Entity as MonoBehaviour;
		string entityName = entityComponent != null
			? entityComponent.name
			: (eventData.Entity != null ? eventData.Entity.GetType().Name : "Unknown");
		Debug.Log($"[ConsoleDebugger] Entity died: {entityName}");
	}
}