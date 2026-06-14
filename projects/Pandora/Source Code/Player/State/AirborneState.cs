using UnityEngine;

public class AirborneState : PlayerHierarchicalState
{
    public AirborneState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Update()
    {
        if (inputHandler.JumpPressed&&abilityContext.CanAirJump)
        {
            ChangeSubState(PlayerStateType.AirJump);
        }
        if (!inputHandler.JumpHeld && abilityContext.CanJumpRelease && movement.Rb.linearVelocityY > 0)
        {
            movement.jumpRelease();
            abilityContext.ConsumeJumpRelease();
        }

        if (movement.Rb.linearVelocityY < 0 && (subState == null || subState.StateType != PlayerStateType.Fall))
        {
            ChangeSubState(PlayerStateType.Fall);
        }

        base.Update();
    }

    public override void Exit()
    {
        subState = null;
    }
}

public class AirJumpState : PlayerHierarchicalState
{
    public AirJumpState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent) : base(stateMachine, stateType, parent) { }

    public override void Enter()
    {
        visuals.SetAnimation("AirJump");
        float currentJumpSpeed = 18f;
        movement.jump(currentJumpSpeed);
        abilityContext.ConsumeAirJump();
    }
}

public class FallState : PlayerHierarchicalState
{
    private float fallTimer;
    private float limitToStiffness = 1f;
    public FallState(PlayerStateMachine ctx, PlayerStateType stateType,PlayerHierarchicalState parent) : base(ctx, stateType, parent) { }

    public override void Enter()
    {
        visuals.SetAnimation("Fall");
        fallTimer = 0f;
    }
    public override void Update()
    {
        fallTimer += Time.deltaTime;
        if (fallTimer >= limitToStiffness && env.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.FallStiffness);
        }
    }
}


