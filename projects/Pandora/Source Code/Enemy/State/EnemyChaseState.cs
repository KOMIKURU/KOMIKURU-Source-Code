using UnityEngine;
public class EnemyChaseState : EnemyHierarchicalState
{
    private IEnemyMovementStrategy _movementStrategy;
    public EnemyChaseState(EnemyStateMachine sm, IEnemyMovementStrategy movementStrategy, EnemyHierarchicalState parent = null) : base(sm, parent)
    {
        _movementStrategy = movementStrategy;
    }

    public override void Enter()
    {
        Debug.Log("追跡ステートへチェンジ");
        stateMachine.Visuals.SetAnimation("Chase");
    }

    public override void Update()
    {
        // 左右移動の実行
        stateMachine.Visuals.LookPlayer();
        float moveDir = stateMachine.Visuals.EnemyDirection;
        stateMachine.Movement.KnockbackUpdate();
        _movementStrategy.ExecuteMovement();
    }
}