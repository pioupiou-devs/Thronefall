using System.Collections.Generic;

public class BuildingStateFactory
{
    private StateMachine<BuildingState> stateMachine;

    public StateMachine<BuildingState> CreateStateMachine(Building building)
    {
        stateMachine = new StateMachine<BuildingState>(BuildingState.Idle, CreateStates(building));
        return stateMachine;
    }

    public Dictionary<BuildingState, State<BuildingState>> CreateStates(Building building)
    {
        building.TryGetComponent<Attack>(out var attack);
        building.TryGetComponent<Targeting>(out var targeting);
        building.TryGetComponent<Health>(out var health);

        return new Dictionary<BuildingState, State<BuildingState>>
        {
            { BuildingState.Idle, CreateIdleState(attack, targeting) },
            { BuildingState.Attack, CreateAttackState(attack, targeting) },
            { BuildingState.Broken, CreateBrokenState(health) }
        };
    }

    private State<BuildingState> CreateIdleState(Attack attack, Targeting targeting)
    {
        return new State<BuildingState>(
            isEnterAuthorized: () => attack != null && targeting != null,
            onStateUpdate: () =>
            {
                targeting.Refresh();

                var target = targeting.CurrentTarget;
                if (target != null && attack.IsInRange(target))
                    stateMachine.ChangeState(BuildingState.Attack);
            }
        );
    }

    private State<BuildingState> CreateAttackState(Attack attack, Targeting targeting)
    {
        return new State<BuildingState>(
            isEnterAuthorized: () => attack != null && targeting != null,
            onStateUpdate: () =>
            {
                var target = targeting.CurrentTarget;

                // Target lost or out of range: go back to idle
                if (target == null || !attack.IsInRange(target))
                {
                    stateMachine.ChangeState(BuildingState.Idle);
                    return;
                }

                attack.TryAttack(target);
            }
        );
    }

    private State<BuildingState> CreateBrokenState(Health health)
    {
        return new State<BuildingState>(
            isEnterAuthorized: () => health != null && health.isDead,
            onStateEnter: (from, to) =>
            {
                // Building is broken: hook for effects/cleanup
            }
        );
    }
}
