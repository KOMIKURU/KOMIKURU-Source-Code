
public class PlatformerState : PlayerHierarchicalState
{
    public PlatformerState(PlayerStateMachine stateMachine, PlayerStateType stateType) : base(stateMachine, stateType) { }

    public override void Enter()
    {
        if (env.IsGrounded)
        {
            ChangeSubState(PlayerStateType.Grounded);
        }
        else if (!env.IsGrounded && env.IsTouchingWall && !env.IsInWater)
        {
            ChangeSubState(PlayerStateType.Wall);
        }
        else if (!env.IsGrounded && !env.IsTouchingWall && !env.IsInWater)
        {
            ChangeSubState(PlayerStateType.Airborne);
        }
        else if (!env.IsGrounded && !env.IsTouchingWall && env.IsInWater)
        {
            ChangeSubState(PlayerStateType.InWater);
        }
    }

    public override void Update()
    {
        CheckGlobalTransitions();
        if (stateMachine.CurrentState != this) return;
        //movement.KnockbackUpdate();
        movement.move(inputHandler.MoveInput);
        visuals.SetFacingDirection(inputHandler.MoveInput,env.IsTouchingWall,env.IsTouchingRightWall,env.IsTouchingLeftWall);
        base.Update();
        if (env.IsGrounded && subState.StateType != PlayerStateType.Grounded)
        {
            ChangeSubState(PlayerStateType.Grounded);
        }
        else if (!env.IsGrounded && env.IsTouchingWall && !env.IsInWater && subState.StateType != PlayerStateType.Wall)
        {
            ChangeSubState(PlayerStateType.Wall);
        }
        else if (!env.IsGrounded && !env.IsTouchingWall && !env.IsInWater && subState.StateType != PlayerStateType.Airborne)
        {
            ChangeSubState(PlayerStateType.Airborne);
        }
        else if (!env.IsGrounded && !env.IsTouchingWall && env.IsInWater && subState.StateType != PlayerStateType.InWater)
        {
            ChangeSubState(PlayerStateType.InWater);
        }
    }

    private void CheckGlobalTransitions()
    {
        // É_ÉbÉVÉÖ
        if (inputHandler.DashPressed && abilityContext.CanDash)
        {
            stateMachine.ChangeState(PlayerStateType.Dash);
            return;
        }

        // çUåÇ
        if (inputHandler.AttackPressed)
        {
            if (inputHandler.LookUpHeld)
                stateMachine.ChangeState(PlayerStateType.UpAttack);
            else if (!env.IsGrounded && inputHandler.LookDownHeld)
                stateMachine.ChangeState(PlayerStateType.DownAttack);
            else
                stateMachine.ChangeState(PlayerStateType.NormalAttack);

            return;
        }
    }
}