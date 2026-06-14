using UnityEngine;
public class EnemyPatrolState : EnemyHierarchicalState
{
    private PlayerDetectContext playerDetectContext;
    private EnemyStateType firstSubState;
    private EnemyStateType nextRootState;
    public EnemyPatrolState(EnemyStateMachine stateMachine, EnemyStateType firstSubState, EnemyStateType nextRootState = EnemyStateType.None, PlayerDetectContext playerDetectContext = null) : base(stateMachine)
    {
        this.playerDetectContext = playerDetectContext;
        this.firstSubState = firstSubState;
        this.nextRootState = nextRootState;
    }

    public override void Enter()
    {
        ChangeSubState(firstSubState);
    }

    public override void Update()
    {
        base.Update();
        if (nextRootState == EnemyStateType.None && playerDetectContext == null) return;
        if (playerDetectContext.DetectPlayer)
        {
            stateMachine.ChangeState(nextRootState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
