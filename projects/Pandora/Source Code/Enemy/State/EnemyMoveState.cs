using NUnit;
using UnityEngine;

public class EnemyMoveState : EnemyHierarchicalState
{
    private IEnemyMovementStrategy _movementStrategy;
    private bool _isTurning;

    private float _moveDuration;

    private float timer = 0f;
    public EnemyMoveState(EnemyStateMachine sm,IEnemyMovementStrategy movementStrategy, bool isTurning = true,float moveDuration=0, EnemyHierarchicalState parent = null) : base(sm, parent)
    {
        _movementStrategy = movementStrategy;
        _isTurning = isTurning;
        _moveDuration = moveDuration;
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Visuals.SetAnimation("Move");
        _movementStrategy.Enter();
    }

    public override void Update()
    {
        float currentDir = stateMachine.Visuals.EnemyDirection;

        if (_isTurning&&(stateMachine.Env.IsTouchingLeftWall && currentDir < 0) ||
        (stateMachine.Env.IsTouchingRightWall && currentDir > 0))
        {
            // 即反転せず、Turnステートへ切り替える
            stateMachine.ChangeState(EnemyStateType.Turn);
            return;
        }

        stateMachine.Movement.KnockbackUpdate();
        // 左右移動の実行
        _movementStrategy.ExecuteMovement();

        if (_moveDuration == 0f) return;

        timer += Time.deltaTime;
        if (timer >= _moveDuration)
        {
            timer = 0f;
            //Debug.Log("MoveStateからIdleへ");
            RequestTransition(EnemyStateType.Idle);
        }
    }
    public override void Exit()
    {
        _movementStrategy.Exit();
    }
}
