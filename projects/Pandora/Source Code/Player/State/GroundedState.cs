public class GroundedState : PlayerHierarchicalState
{
    public GroundedState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Enter()
    {
        if (inputHandler.MoveInput == 0)
        {
            ChangeSubState(PlayerStateType.Idle);
        }
        else
        {
            ChangeSubState(PlayerStateType.Run);
        }
    }

    public override void Update()
    {
        base.Update();
        if (inputHandler.JumpPressed)
        {
            // 親(Platformer)のサブステートをJumpに変更
            ChangeSubState(PlayerStateType.Jump);
        }
        if (inputHandler.MoveInput == 0 && subState.StateType != PlayerStateType.Idle && subState.StateType != PlayerStateType.Jump)
        {
            ChangeSubState(PlayerStateType.Idle);
        }
        else if (inputHandler.MoveInput != 0 && subState.StateType != PlayerStateType.Run && subState.StateType != PlayerStateType.Jump)
        {
            ChangeSubState(PlayerStateType.Run);
        }
    }
}

public class IdleState : PlayerHierarchicalState
{
    public IdleState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Enter()
    {
        visuals.SetAnimation("Idle");
    }
}

public class RunState : PlayerHierarchicalState
{
    public RunState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Enter()
    {
        visuals.SetAnimation("Running");
        //visuals.StartWalkSound();
    }

    public override void Exit()
    {
        //visuals.StopWalkSound();
    }
}

public class JumpState : PlayerHierarchicalState
{
    public JumpState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Enter()
    {
        visuals.SetAnimation("Jump");
        float currentJumpSpeed = 20f;
        movement.jump(currentJumpSpeed);
    }
}




