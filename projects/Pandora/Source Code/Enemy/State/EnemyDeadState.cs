using UnityEngine;

public class EnemyDeadState : EnemyHierarchicalState
{
    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Movement.SetVelocityX(0f);
        stateMachine.Visuals.SetAnimation("Dead");
        Debug.Log("Enemy has died.");
    }

    public override void Update()
    {
        // €–Só‘Ô‚Å‚Í“Á‚É‰½‚à‚µ‚È‚¢
        stateMachine.Movement.KnockbackUpdate();
        stateMachine.Movement.ApplyKnockback();
    }
}
