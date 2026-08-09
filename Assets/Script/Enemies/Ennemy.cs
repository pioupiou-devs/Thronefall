using System;
using UnityEngine;

[RequireComponent(typeof(Attack), typeof(Health))]
[RequireComponent(typeof(Targeting), typeof(NavMeshMover))]
public class Ennemy : Entity {
    
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
            stateMachine.ChangeState(EnemyState.Dead);

        // Target killed so move on
        if (evt.Source == targeting.CurrentTarget)
            stateMachine.ChangeState(EnemyState.Move);
    }
}