
using UnityEngine;

public class EnemyIdleState : EnemyHierarchicalState
{
    private IEnemyIdleStrategy _idleStrategy;

    private float _idleDuration;

    private float timer = 0f;

    public EnemyIdleState(EnemyStateMachine sm, IEnemyIdleStrategy movementStrategy, float idleDuration,EnemyHierarchicalState parent = null) : base(sm, parent)
    {
        _idleStrategy = movementStrategy;
        _idleDuration = idleDuration;
    }

    public override void Enter()
    {
        _idleStrategy.Enter();
        stateMachine.Visuals.SetAnimation("Idle");
    }

    public override void Update()
    {
        _idleStrategy.ExecuteIdle();
        stateMachine.Movement.KnockbackUpdate();
        timer += Time.deltaTime;
        if (timer >= _idleDuration)
        {
            timer = 0f;
            RequestTransition(EnemyStateType.Move);
        }
    }

    public override void Exit()
    {
        _idleStrategy.Exit();
    }
}
