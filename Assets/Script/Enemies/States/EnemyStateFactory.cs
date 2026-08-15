using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateFactory
{
    private StateMachine<EnemyState> stateMachine;

    public StateMachine<EnemyState> CreateStateMachine(Ennemy ennemy)
    {
        stateMachine = new StateMachine<EnemyState>(EnemyState.Idle, CreateStates(ennemy));

        return stateMachine;
    }

    public Dictionary<EnemyState, State<EnemyState>> CreateStates(Ennemy ennemy)
    {
        // Unfold components
        ennemy.TryGetComponent<Attack>(out var attack);
        ennemy.TryGetComponent<Targeting>(out var targeting);
        ennemy.TryGetComponent<NavMeshMover>(out var mover);
        ennemy.TryGetComponent<Health>(out var health);


        return new Dictionary<EnemyState, State<EnemyState>>
        {
            {
                EnemyState.Idle,
                CreateIdleState(mover)
            },
            {
                EnemyState.Search,
                CreateSearchState(targeting)
            },
            {
                EnemyState.Move,
                CreateMovingState(attack, targeting, mover)
            },
            {
                EnemyState.Attack,
                CreateAttackingState(attack, targeting)
            },
            {
                EnemyState.Dead,
                CreateDeadState(mover, health)
            }
        };
    }

    private State<EnemyState> CreateSearchState(Targeting targeting)
    {
        return new State<EnemyState>(
            isEnterAuthorized: () => targeting != null,
            onStateUpdate: () =>
            {
                // Search only acquires a target; range/attack decision belongs to Move.
                targeting.Refresh();

                if (targeting.CurrentTarget != null)
                    stateMachine.ChangeState(EnemyState.Move);
            }
            );
    }

    private State<EnemyState> CreateDeadState(NavMeshMover mover, Health health)
    {
        return new State<EnemyState>(
            isEnterAuthorized: () => health.isDead,
            onStateEnter: (from,to) =>
            {
                if(mover != null)
                    mover.Stop();
            }
        );
    }

    private State<EnemyState> CreateAttackingState(Attack attack, Targeting targeting)
    {
        return new State<EnemyState>(
            isEnterAuthorized: () => attack != null && targeting != null,
            onStateUpdate: () =>
            {
                // Find attack target
                var target = targeting.CurrentTarget;

                // Target lost or out of range: chase it again
                if (target == null || !attack.IsInRange(target))
                {
                    stateMachine.ChangeState(EnemyState.Move);
                    return;
                }

                // Attack
                attack.TryAttack(target);
            }
        );
    }

    private State<EnemyState> CreateMovingState(Attack attack, Targeting targeting, NavMeshMover mover)
    {
        return new State<EnemyState>(
            isEnterAuthorized: () => attack != null && mover != null && targeting != null,
            onStateUpdate: () =>
            {
                // Find attack target
                var target = targeting.CurrentTarget;

                // No target: go back to searching
                if (target == null)
                {
                    stateMachine.ChangeState(EnemyState.Search);
                    return;
                }

                // In range: stop chasing and attack
                if (attack.IsInRange(target))
                {
                    stateMachine.ChangeState(EnemyState.Attack);
                    return;
                }

                // Out of range: keep moving toward the target
                if(!mover.IsMoving)
                    mover.SetDestination(target.transform.position);
            },
            onStateExit: (from, to) =>
            {
                if(mover.IsMoving)
                   mover.Stop();
            }
            );
    }

    private State<EnemyState> CreateIdleState(NavMeshMover mover)
    {
        return new State<EnemyState>(
            onStateEnter: (from,to) =>
            {
                if(mover != null)
                    mover.Stop();
            }
        );
    }
}