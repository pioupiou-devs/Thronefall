using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _count = 3;

    private readonly List<Entity> _alive = new();
    private int _currentWave;

    private EventBinding<WaveStartEvent> _waveStartBinding;
    private EventBinding<EntityDiedEvent> _entityDiedBinding;

    private void OnEnable()
    {
        _waveStartBinding = new EventBinding<WaveStartEvent>(OnWaveStart);
        EventBus<WaveStartEvent>.Register(_waveStartBinding);

        _entityDiedBinding = new EventBinding<EntityDiedEvent>(OnEntityDied);
        EventBus<EntityDiedEvent>.Register(_entityDiedBinding);
    }

    private void OnDisable()
    {
        EventBus<WaveStartEvent>.Deregister(_waveStartBinding);
        EventBus<EntityDiedEvent>.Deregister(_entityDiedBinding);
    }

    private void OnWaveStart(WaveStartEvent evt)
    {
        _currentWave = evt.WaveIndex;
        SpawnWave();
    }

    private void OnEntityDied(EntityDiedEvent evt)
    {
        // Only decrement for enemies this spawner created
        if (_alive.Remove(evt.Source) && _alive.Count == 0)
        {
            Debug.Log($"[EnemySpawner] Wave {_currentWave} cleared.", this);
            EventBus<WaveClearedEvent>.Raise(new WaveClearedEvent(_currentWave));
        }
    }

    public void SpawnWave()
    {
        if (_enemyPrefab == null)
        {
            Debug.LogWarning($"[{nameof(EnemySpawner)}] No enemy prefab assigned on '{name}'.", this);
            return;
        }

        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogWarning($"[{nameof(EnemySpawner)}] No spawn points assigned on '{name}'.", this);
            return;
        }

        _alive.Clear();
        for (int i = 0; i < _count; i++)
        {
            var point = _spawnPoints[i % _spawnPoints.Length];
            var enemy = Instantiate(_enemyPrefab, point.position, point.rotation);
            _alive.Add(enemy);
            Debug.Log($"[EnemySpawner] Spawned '{enemy.name}' at '{point.name}' (spawn #{i}).", this);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_spawnPoints == null) return;

        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.8f);
        foreach (var point in _spawnPoints)
        {
            if (point == null) continue;
            Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
#endif
}
