using System;
using UnityEngine;

[RequireComponent(typeof(Attack))]
[RequireComponent(typeof(Targeting))]
[RequireComponent(typeof(Health))]
public class Building : Entity
{
    protected override Faction DefaultFaction => Faction.Player;

    // Current state machine state (read-only)
    public BuildingState CurrentState => stateMachine.CurrentState;

    private StateMachine<BuildingState> stateMachine;
    private BuildingStateFactory stateFactory;

    private EventBinding<EntityDiedEvent> entityDiedBinding;

    private void Awake()
    {
        InitializeStateMachine();

        entityDiedBinding = new EventBinding<EntityDiedEvent>(OnEntityDied);
        EventBus<EntityDiedEvent>.Register(entityDiedBinding);
    }

    private void Start()
    {
        stateMachine.ChangeState(BuildingState.Idle);
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
        stateFactory = new BuildingStateFactory();
        stateMachine = stateFactory.CreateStateMachine(this);
    }

    private void OnEntityDied(EntityDiedEvent evt)
    {
        if (evt.Source == this)
            stateMachine.ChangeState(BuildingState.Broken);
    }
}
