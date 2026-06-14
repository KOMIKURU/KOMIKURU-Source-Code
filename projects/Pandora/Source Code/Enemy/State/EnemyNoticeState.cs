using UnityEngine;

public class EnemyNoticeState : EnemyHierarchicalState
{
    private EnemyStateType nextRootState;
    public EnemyNoticeState(EnemyStateMachine sm, EnemyStateType nextRootState, EnemyHierarchicalState parent = null) : base(sm, parent)
    {
        this.nextRootState = nextRootState;
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Visuals.LookPlayer();
        stateMachine.Visuals.SetAnimation("Notice");
        Debug.Log("Notice!!");
    }

    public override void Update()
    {
        stateMachine.Movement.KnockbackUpdate();
        stateMachine.Movement.ApplyKnockback();
        var stateInfo = stateMachine.Visuals.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Notice") && stateInfo.normalizedTime >= 1.0f && !stateMachine.Visuals.Animator.IsInTransition(0))
        {
            stateMachine.ChangeState(nextRootState);
        }
    }
    public override void Exit()
    {
        
    }
}
