using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DayNightController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int _totalWaves = 3;
    [SerializeField] private PlayerInput _playerInput;

    public GamePhase CurrentPhase => _phase;
    public int CurrentWave => _currentWave;

    private GamePhase _phase = GamePhase.Day;
    private int _currentWave;
    private InputAction _startNightAction;

    private EventBinding<WaveClearedEvent> _waveClearedBinding;
    private EventBinding<EntityDiedEvent> _entityDiedBinding;

    private void Awake()
    {
        if (_playerInput == null)
            _playerInput = GetComponent<PlayerInput>();

        _startNightAction = _playerInput != null && _playerInput.actions != null
            ? _playerInput.actions.FindAction("StartNight")
            : null;
        if (_startNightAction == null)
            Debug.LogWarning($"[{nameof(DayNightController)}] No 'StartNight' action found on PlayerInput.", this);
    }

    private void OnEnable()
    {
        _waveClearedBinding = new EventBinding<WaveClearedEvent>(OnWaveCleared);
        EventBus<WaveClearedEvent>.Register(_waveClearedBinding);

        _entityDiedBinding = new EventBinding<EntityDiedEvent>(OnEntityDied);
        EventBus<EntityDiedEvent>.Register(_entityDiedBinding);
    }

    private void OnDisable()
    {
        EventBus<WaveClearedEvent>.Deregister(_waveClearedBinding);
        EventBus<EntityDiedEvent>.Deregister(_entityDiedBinding);
    }

    private void Update()
    {
        if (_phase != GamePhase.Day) return;

        if (_startNightAction != null && _startNightAction.WasPressedThisFrame())
            StartNight();
    }

    public void StartNight()
    {
        if (_phase != GamePhase.Day) return;

        _currentWave++;
        _phase = GamePhase.Night;
        Debug.Log($"[DayNightController] Night {_currentWave} started.");
        EventBus<WaveStartEvent>.Raise(new WaveStartEvent(_currentWave));
    }

    public void SetDay()
    {
        if (_phase == GamePhase.Victory || _phase == GamePhase.Defeat) return;
        _phase = GamePhase.Day;
    }

    private void OnWaveCleared(WaveClearedEvent evt)
    {
        if (_phase != GamePhase.Night) return;

        if (_currentWave >= _totalWaves)
        {
            EndGame(victory: true);
            return;
        }

        _phase = GamePhase.Day;
        Debug.Log($"[DayNightController] Night {evt.WaveIndex} cleared. Back to Day.");
    }

    private void OnEntityDied(EntityDiedEvent evt)
    {
        if (_phase != GamePhase.Night) return;

        // Defeat if any flagged-protected building is razed
        if (evt.Source is Building { IsProtected: true })
            EndGame(victory: false);
    }

    private void EndGame(bool victory)
    {
        _phase = victory ? GamePhase.Victory : GamePhase.Defeat;
        Debug.Log($"[DayNightController] Game over: {(victory ? "Victory" : "Defeat")}.");
        EventBus<GameOverEvent>.Raise(new GameOverEvent(victory));
    }
}
