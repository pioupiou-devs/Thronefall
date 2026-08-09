using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Entity))]
public class NavMeshMover : MonoBehaviour, IMovable
{
    [Header("Movement")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float angularSpeed = 120f;
    [SerializeField] private float stoppingDistance = 0.2f;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private Vector3 debugTargetPosition;
#endif

    private NavMeshAgent _agent;
    [HideInInspector]
    public Entity Entity;
    private EventBinding<EntityDiedEvent> _diedBinding;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;
        _agent.angularSpeed = angularSpeed;
        _agent.stoppingDistance = stoppingDistance;

        Entity = GetComponent<Entity>();
    }

    private void OnEnable()
    {
        _diedBinding = new EventBinding<EntityDiedEvent>(OnEntityDied);
        EventBus<EntityDiedEvent>.Register(_diedBinding);
    }

    private void OnDisable()
    {
        EventBus<EntityDiedEvent>.Deregister(_diedBinding);
    }

    public void SetDestination(Vector3 target)
    {
        if (!_agent.isActiveAndEnabled) return;
        _agent.isStopped = false;
        _agent.SetDestination(target);
    }

    public void Stop()
    {
        if (!_agent.isActiveAndEnabled) return;
        _agent.isStopped = true;
        _agent.ResetPath();
    }

    public bool IsMoving =>
        _agent.isActiveAndEnabled && !_agent.isStopped && _agent.remainingDistance > _agent.stoppingDistance;

    private void OnEntityDied(EntityDiedEvent e)
    {
        if (e.Source == Entity)
            Stop();
    }
}
