using UnityEngine;

public class ConsoleDebugger : MonoBehaviour
{
	private EventBinding<EntityDiedEvent> entityDiedBinding;
	private EventBinding<WaveStartEvent> waveStartBinding;
	private EventBinding<WaveClearedEvent> waveClearedBinding;
	private EventBinding<GameOverEvent> gameOverBinding;
	private EventBinding<HealthChangedEvent> healthChangedBinding;
	private EventBinding<GamePhaseChangedEvent> gamePhaseChangedBinding;

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

		waveClearedBinding = new EventBinding<WaveClearedEvent>(OnWaveCleared);
		EventBus<WaveClearedEvent>.Register(waveClearedBinding);

		gameOverBinding = new EventBinding<GameOverEvent>(OnGameOver);
		EventBus<GameOverEvent>.Register(gameOverBinding);

		healthChangedBinding = new EventBinding<HealthChangedEvent>(OnHealthChanged);
		EventBus<HealthChangedEvent>.Register(healthChangedBinding);

		gamePhaseChangedBinding = new EventBinding<GamePhaseChangedEvent>(OnGamePhaseChanged);
		EventBus<GamePhaseChangedEvent>.Register(gamePhaseChangedBinding);
	}

	private void OnDisable()
	{
		EventBus<EntityDiedEvent>.Deregister(entityDiedBinding);
		EventBus<WaveStartEvent>.Deregister(waveStartBinding);
		EventBus<WaveClearedEvent>.Deregister(waveClearedBinding);
		EventBus<GameOverEvent>.Deregister(gameOverBinding);
		EventBus<HealthChangedEvent>.Deregister(healthChangedBinding);
		EventBus<GamePhaseChangedEvent>.Deregister(gamePhaseChangedBinding);
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

	private void OnWaveCleared(WaveClearedEvent eventData)
	{
		Debug.Log($"[ConsoleDebugger] Wave {eventData.WaveIndex} cleared");
	}

	private void OnGameOver(GameOverEvent eventData)
	{
		Debug.Log($"[ConsoleDebugger] Game over: {(eventData.Victory ? "Victory" : "Defeat")}");
	}

	private void OnHealthChanged(HealthChangedEvent eventData)
	{
		string entityName = eventData.Source != null ? eventData.Source.name : "Unknown";
		Debug.Log($"[ConsoleDebugger] Health changed: {entityName} {eventData.Current}/{eventData.Max}");
	}

	private void OnGamePhaseChanged(GamePhaseChangedEvent eventData)
	{
		Debug.Log($"[ConsoleDebugger] Phase changed: {eventData.Phase}");
	}

#if UNITY_EDITOR
	public void TriggerWaveStart()
	{
		EventBus<WaveStartEvent>.Raise(new WaveStartEvent(_waveIndex));
	}

	public void TriggerWaveCleared()
	{
		EventBus<WaveClearedEvent>.Raise(new WaveClearedEvent(_waveIndex));
	}

	public void TriggerEntityDied()
	{
		EventBus<EntityDiedEvent>.Raise(new EntityDiedEvent(_entityDiedSource));
	}

	public void TriggerGameOver()
	{
		EventBus<GameOverEvent>.Raise(new GameOverEvent(true));
	}
#endif
}
