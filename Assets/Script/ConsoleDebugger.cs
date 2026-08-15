using UnityEngine;

public class ConsoleDebugger : MonoBehaviour
{
	private EventBinding<EntityDiedEvent> entityDiedBinding;
	private EventBinding<WaveStartEvent> waveStartBinding;

#if UNITY_EDITOR
	[Header("Debug event triggers (used by ConsoleDebuggerEditor)")]
	[SerializeField] private int _waveIndex = 1;
	[SerializeField] private Entity _entityDiedSource;
#endif

	private void OnEnable()
	{
		entityDiedBinding = new EventBinding<EntityDiedEvent>(OnEntityDied);
		EventBus<EntityDiedEvent>.Register(entityDiedBinding);

		waveStartBinding = new EventBinding<WaveStartEvent>(OnWaveStart);
		EventBus<WaveStartEvent>.Register(waveStartBinding);
	}

	private void OnDisable()
	{
		EventBus<EntityDiedEvent>.Deregister(entityDiedBinding);
		EventBus<WaveStartEvent>.Deregister(waveStartBinding);
	}

	private void OnEntityDied(EntityDiedEvent eventData)
	{
		string entityName = eventData.Source != null ? eventData.Source.name : "Unknown";
		Debug.Log($"[ConsoleDebugger] Entity died: {entityName}");
	}

	private void OnWaveStart(WaveStartEvent eventData)
	{
		Debug.Log($"[ConsoleDebugger] Wave {eventData.WaveIndex} started");
	}

#if UNITY_EDITOR
	public void TriggerWaveStart()
	{
		EventBus<WaveStartEvent>.Raise(new WaveStartEvent(_waveIndex));
	}

	public void TriggerEntityDied()
	{
		EventBus<EntityDiedEvent>.Raise(new EntityDiedEvent(_entityDiedSource));
	}
#endif
}
