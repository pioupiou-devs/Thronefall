using System;
using UnityEngine;

[RequireComponent(typeof(Attack), typeof(Health))]
[RequireComponent(typeof(Targeting), typeof(NavMeshMover))]
public class Enemy : Entity {

    protected override Faction DefaultFaction => Faction.Enemy;

    // Current state machine state (read-only)
    public EnemyState CurrentState => stateMachine.CurrentState;

    // States
    private StateMachine<EnemyState> stateMachine;
    private EnemyStateFactory stateFactory;

    // Events
	private EventBinding<EntityDiedEvent> entityDiedBinding;
    
    // Components
    private Targeting targeting;

    private void Awake()
    {
        // Components
        if(!TryGetComponent(out targeting)) throw new ArgumentNullException();

        InitializeStateMachine();

        // Register to events
		entityDiedBinding = new EventBinding<EntityDiedEvent>(OnEntityDied);
		EventBus<EntityDiedEvent>.Register(entityDiedBinding);
    }

    private void Start()
    {
        // When game start, the enemy search a target to attack
        stateMachine.ChangeState(EnemyState.Search);
    }

    private void Update()
    {
        stateMachine.Tick();
    }

    private void OnDestroy()
    {
        EventBus<EntityDiedEvent>.Deregister(entityDiedBinding);
    }

    private void InitializeStateMachine()
    {
        // Init state factory
        stateFactory = new();

        // Create state machine
        stateMachine = stateFactory.CreateStateMachine(this);
    }

    private void OnEntityDied(EntityDiedEvent evt)
    {
        // Current enemy is dead
        if(evt.Source == this) 
        {
            stateMachine.ChangeState(EnemyState.Dead);
            return;
        }

        // Target killed: go back to search so we drop the dead target and pick a new one
        if (evt.Source == targeting.CurrentTarget)
            stateMachine.ChangeState(EnemyState.Search);
    }
}
