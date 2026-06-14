public class InWaterState : PlayerHierarchicalState
{
    public InWaterState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Enter()
    {
        if (inputHandler.MoveInput == 0)
        {
            ChangeSubState(PlayerStateType.Float);
        }
        else
        {
            ChangeSubState(PlayerStateType.Swim);
        }
    }

    public override void Update()
    {
        base.Update();
        if (inputHandler.MoveInput == 0 && subState.StateType != PlayerStateType.Float)
        {
            ChangeSubState(PlayerStateType.Float);
        }
        else if (inputHandler.MoveInput != 0 && subState.StateType != PlayerStateType.Swim)
        {
            ChangeSubState(PlayerStateType.Swim);
        }
        if (inputHandler.JumpPressed)
        {
            ChangeSubState(PlayerStateType.WaterJump);
        }
    }
}

public class FloatState : PlayerHierarchicalState
{
    public FloatState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Enter()
    {
        visuals.SetAnimation("Float");
    }
}

public class SwimState : PlayerHierarchicalState
{
    public SwimState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Enter()
    {
        visuals.SetAnimation("Swim");

    }

    public override void Exit()
    {
        
    }
}

public class WaterJumpState : PlayerHierarchicalState
{
    public WaterJumpState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Enter()
    {
        visuals.SetAnimation("Jump");
        float currentJumpSpeed = 15f;
        movement.jump(currentJumpSpeed);
    }
}