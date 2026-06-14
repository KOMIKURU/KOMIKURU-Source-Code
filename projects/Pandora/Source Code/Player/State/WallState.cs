public class WallState : PlayerHierarchicalState
{
    public WallState(PlayerStateMachine ctx, PlayerStateType stateType, PlayerHierarchicalState parent) : base(ctx, stateType,parent) { }

    public override void Update()
    {
        base.Update();

        if(inputHandler.JumpPressed)
        {
            stateMachine.ChangeState(PlayerStateType.WallJump);
            return;
        }

        if (movement.Rb.linearVelocityY < 0 && (subState == null || subState.StateType != PlayerStateType.WallSliding))
        {
            ChangeSubState(PlayerStateType.WallSliding);
        }
    }

    public override void Exit()
    {
        subState = null; 
    }
}

public class WallSlidingState : PlayerHierarchicalState
{
    public WallSlidingState(PlayerStateMachine ctx, PlayerStateType stateType,PlayerHierarchicalState parent) : base(ctx, stateType,parent) { }

    public override void Enter()
    {
        visuals.SetAnimation("WallSliding");
    }
    public override void Update()
    {
        movement.wallSliding();
    }
}
