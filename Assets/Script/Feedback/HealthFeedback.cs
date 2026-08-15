using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthFeedback : MonoBehaviour
{
    [Header("Hit Flash")]
    [SerializeField] private Color _hitTint = new Color(1f, 0.25f, 0.25f, 1f);
    [SerializeField] private float _flashDuration = 0.15f;

    [Header("Death")]
    [Tooltip("Optional: burst effect spawned at this object on death (e.g. a particle prefab). " +
             "Leave empty if you don't want one. Effect itself should be one-shot with Stop Action = Destroy.")]
    [SerializeField] private ParticleSystem _deathBurst;

    private Renderer _renderer;
    private Material _materialInstance;
    private Color _baseColor;
    private float _flashTimer;

    private EventBinding<HealthChangedEvent> _healthChangedBinding;
    private EventBinding<EntityDiedEvent> _entityDiedBinding;

    private void Awake()
    {
        _renderer = GetFirstVisualRenderer();
        if (_renderer != null && _renderer.material.HasProperty("_Color"))
        {
            _materialInstance = _renderer.material;
            _baseColor = _materialInstance.color;
        }
    }

    // First non-particle renderer on this object or any child (skip ParticleSystemRenderer, whose
    // material has no _Color property).
    private Renderer GetFirstVisualRenderer()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>(true))
        {
            if (renderer is ParticleSystemRenderer) continue;
            return renderer;
        }
        return null;
    }

    private void OnEnable()
    {
        _healthChangedBinding = new EventBinding<HealthChangedEvent>(OnHealthChanged);
        EventBus<HealthChangedEvent>.Register(_healthChangedBinding);

        _entityDiedBinding = new EventBinding<EntityDiedEvent>(OnEntityDied);
        EventBus<EntityDiedEvent>.Register(_entityDiedBinding);
    }

    private void OnDisable()
    {
        EventBus<HealthChangedEvent>.Deregister(_healthChangedBinding);
        EventBus<EntityDiedEvent>.Deregister(_entityDiedBinding);
    }

    private void Update()
    {
        if (_materialInstance == null || _flashTimer <= 0f) return;

        _flashTimer -= Time.deltaTime;
        float t = Mathf.Clamp01(_flashTimer / _flashDuration);
        _materialInstance.color = Color.Lerp(_baseColor, _hitTint, t);
    }

    private void OnHealthChanged(HealthChangedEvent evt)
    {
        if (evt.Source == null || evt.Source.gameObject != gameObject) return;

        _flashTimer = _flashDuration;
        if (_materialInstance != null)
            _materialInstance.color = _hitTint;
    }

    private void OnEntityDied(EntityDiedEvent evt)
    {
        if (evt.Source == null || evt.Source.gameObject != gameObject) return;

        if (_deathBurst != null)
            Instantiate(_deathBurst, transform.position, transform.rotation);

        if (_materialInstance != null)
        {
            _materialInstance.color = Color.Lerp(_baseColor, Color.black, 0.6f);
            _flashTimer = 0f;
        }
    }
}
