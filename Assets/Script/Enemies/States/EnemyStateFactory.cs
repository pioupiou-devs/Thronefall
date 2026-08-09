using System;
using System.Collections.Generic;

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
                CreateMovingState(targeting, mover)
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
                targeting.Refresh();

                var target = targeting.CurrentTarget;
                if(target == null) return;

                // TODO check if is range             
                bool isInAttackRange = false;
                if(isInAttackRange)
                    stateMachine.ChangeState(EnemyState.Attack);
                else
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

                // TODO : Add "if target not in range, move to it" + distance into targetting

                // Attack
                attack.TryAttack(target);
            }
        );
    }

    private State<EnemyState> CreateMovingState(Targeting targeting, NavMeshMover mover)
    {
        return new State<EnemyState>(
            isEnterAuthorized: () => mover != null && targeting != null,
            onStateUpdate: () =>
            {
                if(mover.IsMoving) return;

                // Find attack target
                var target = targeting.CurrentTarget;

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