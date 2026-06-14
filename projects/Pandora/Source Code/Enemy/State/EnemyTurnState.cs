using UnityEngine;
using System.Collections;

public class EnemyTurnState : EnemyHierarchicalState
{
    private EnemyStateType returnState;
    public EnemyTurnState(EnemyStateMachine sm,EnemyStateType returnState, EnemyHierarchicalState parent = null) : base(sm, parent)
    {
        this.returnState = returnState;
    }

    public override void Enter()
    {
        base.Enter();
        // Animatorで振り向きアニメーションを再生
        stateMachine.Visuals.SetAnimation("Turn");
        // 移動を止める
        stateMachine.Movement.SetVelocityX(0);
    }

    public override void Update()
    {
        stateMachine.Movement.KnockbackUpdate();
        stateMachine.Movement.ApplyKnockback();
        var stateInfo = stateMachine.Visuals.Animator.GetCurrentAnimatorStateInfo(0);

        // 1. 指定したアニメーションが再生中か確認
        // 2. 進行度が1.0（100%）を超えているか確認
        // 3. 遷移中（Transition）でないことを確認（これを入れないとループ時にバグる場合がある）
        if (stateInfo.IsName("Turn") && stateInfo.normalizedTime >= 1.0f && !stateMachine.Visuals.Animator.IsInTransition(0))
        {
            stateMachine.Visuals.ReverseDirection();
            stateMachine.ChangeState(returnState);
        }
    }
}
