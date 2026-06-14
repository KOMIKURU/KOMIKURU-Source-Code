using UnityEngine;
public class DashState : PlayerHierarchicalState
{
    private float dashTimer;
    private float dashDuration = 0.2f;
    public DashState(PlayerStateMachine ctx,PlayerStateType stateType) : base(ctx,stateType) { }

    public override void Enter()
    {
        visuals.SetAnimation("Dash");
        dashTimer = 0f;
        abilityContext.ConsumeDash();
        float dashSpeed = 19f;
        float direction = visuals.FacingDirection;
        movement.dashStart(direction, dashSpeed);
    }

    public override void Update()
    {
        var stateInfo = visuals.Animator.GetCurrentAnimatorStateInfo(0);

        // 1. 指定したアニメーションが再生中か確認
        // 2. 進行度が1.0（100%）を超えているか確認
        // 3. 遷移中（Transition）でないことを確認（これを入れないとループ時にバグる場合がある）
        if (stateInfo.IsName("Dash") && stateInfo.normalizedTime >= 1.0f && !visuals.Animator.IsInTransition(0))
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
    }

    public override void Exit()
    {
        movement.dashEnd();
    }
}

public class WallJumpState : PlayerHierarchicalState
{
    private float wallJumpTimer;
    private float wallJumpDuration = 0.1f;

    private const float WallJumpVelocityX = 10f;
    private const float WallJumpVelocityY = 15f;
    public WallJumpState(PlayerStateMachine ctx, PlayerStateType stateType) : base(ctx, stateType) { }

    public override void Enter()
    {
        visuals.SetAnimation("WallJump");
        wallJumpTimer = 0f;

        float jumpDirectionX = 0f;
        if (env.IsTouchingLeftWall)
        {
            jumpDirectionX = 1f;
        }
        else if (env.IsTouchingRightWall)
        {
            jumpDirectionX = -1f;
        }
        Vector2 wallJumpVelocity = new Vector2(WallJumpVelocityX * jumpDirectionX, WallJumpVelocityY);
        movement.wallJump(wallJumpVelocity*1.5f);
    }

    public override void Update()
    {
        wallJumpTimer += Time.deltaTime;
        if (wallJumpTimer >= wallJumpDuration)
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
        else if (!inputHandler.JumpHeld)
        {
            movement.wallJumpCancel();
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
    }
}

public class NormalAttackState : PlayerHierarchicalState
{
    private string animemationName;
    public NormalAttackState(PlayerStateMachine stateMachine, PlayerStateType stateType) : base(stateMachine, stateType) { }

    public override void Enter()
    {
        animemationName = "NormalAttack";
        if (!stateMachine.Env.IsGrounded)
        {
            animemationName = "AirNormalAttack";
        }
        visuals.SetAnimation(animemationName);
        visuals.NormalAttackStart();
    }

    public override void Update()
    {
        movement.move(inputHandler.MoveInput,6);
        var stateInfo = visuals.Animator.GetCurrentAnimatorStateInfo(0);

        // 1. 指定したアニメーションが再生中か確認
        // 2. 進行度が1.0（100%）を超えているか確認
        // 3. 遷移中（Transition）でないことを確認（これを入れないとループ時にバグる場合がある）
        if (stateInfo.IsName(animemationName) && stateInfo.normalizedTime >= 1.0f && !visuals.Animator.IsInTransition(0))
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }

        if(stateMachine.Env.IsGrounded&&animemationName== "AirNormalAttack")
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class UpAttackState : PlayerHierarchicalState
{
    public UpAttackState(PlayerStateMachine ctx, PlayerStateType stateType) : base(ctx, stateType) { }

    public override void Enter()
    {
        visuals.SetAnimation("UpAttack");
        visuals.UpAttackStart();
    }

    public override void Update()
    {
        movement.move(inputHandler.MoveInput);
        var stateInfo = visuals.Animator.GetCurrentAnimatorStateInfo(0);

        // 1. 指定したアニメーションが再生中か確認
        // 2. 進行度が1.0（100%）を超えているか確認
        // 3. 遷移中（Transition）でないことを確認（これを入れないとループ時にバグる場合がある）
        if ((stateInfo.IsName("UpAttack") && stateInfo.normalizedTime >= 1.0f && !visuals.Animator.IsInTransition(0)))
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
    }

    public override void Exit()
    {
        base.Exit();
        visuals.AttackEnd();
    }
}

public class DownAttackState : PlayerHierarchicalState
{
    public DownAttackState(PlayerStateMachine ctx, PlayerStateType stateType) : base(ctx, stateType) { }

    public override void Enter()
    {
        visuals.SetAnimation("DownAttack");
        visuals.DownAttackStart();
    }

    public override void Update()
    {
        movement.move(inputHandler.MoveInput);
        var stateInfo = visuals.Animator.GetCurrentAnimatorStateInfo(0);

        // 1. 指定したアニメーションが再生中か確認
        // 2. 進行度が1.0（100%）を超えているか確認
        // 3. 遷移中（Transition）でないことを確認（これを入れないとループ時にバグる場合がある）
        if ((stateInfo.IsName("DownAttack") && stateInfo.normalizedTime >= 1.0f && !visuals.Animator.IsInTransition(0))||env.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
    }

    public override void Exit()
    {
        base.Exit();
        visuals.AttackEnd();
    }
}

public class DamagedState : PlayerHierarchicalState
{
    private float damagedTimer;
    private float damagedDuration = 0.4f;
    public DamagedState(PlayerStateMachine stateMachine, PlayerStateType stateType) : base(stateMachine, stateType) { }

    public override void Enter()
    {
        visuals.SetAnimation("Damaged");
        _ = visuals.InvincibleAsync();
        damagedTimer = 0f;
    }

    public override void Update()
    {
        damagedTimer += Time.deltaTime;
        if (damagedTimer >= damagedDuration)
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
    }
}

public class HealState : PlayerHierarchicalState
{
    public HealState(PlayerStateMachine ctx, PlayerStateType stateType) : base(ctx, stateType) { }

    public override void Enter()
    {
        visuals.SetAnimation("Heal");
        movement.deadStart();
    }

    public override void Update()
    {
        movement.move(inputHandler.MoveInput);
        var stateInfo = visuals.Animator.GetCurrentAnimatorStateInfo(0);

        // 1. 指定したアニメーションが再生中か確認
        // 2. 進行度が1.0（100%）を超えているか確認
        // 3. 遷移中（Transition）でないことを確認（これを入れないとループ時にバグる場合がある）
        if ((stateInfo.IsName("Heal") && stateInfo.normalizedTime >= 1.0f && !visuals.Animator.IsInTransition(0)))
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
    }

    public override void Exit()
    {
        base.Exit();
        movement.deadEnd();
    }
}


public class RecoiledState : PlayerHierarchicalState
{
    private float timer;
    private float recoiledDuration = 0.2f;
    public RecoiledState(PlayerStateMachine stateMachine, PlayerStateType stateType) : base(stateMachine, stateType) { }

    public override void Enter()
    {
        timer = 0f;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= recoiledDuration)
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
    }
}

public class FallStiffnessState : PlayerHierarchicalState
{
    private float fallStiffnessTimer;
    private float fallStiffnessDuration = 1.4f;
    public FallStiffnessState(PlayerStateMachine stateMachine, PlayerStateType stateType) : base(stateMachine, stateType) { }

    public override void Enter()
    {
        visuals.SetAnimation("FallStiffness");
        movement.resetVelocity();
        fallStiffnessTimer = 0f;
    }

    public override void Update()
    {
        fallStiffnessTimer += Time.deltaTime;
        if (fallStiffnessTimer >= fallStiffnessDuration)
        {
            stateMachine.ChangeState(PlayerStateType.Platformer);
        }
    }
}

public class InteractState : PlayerHierarchicalState
{
    public InteractState(PlayerStateMachine stateMachine, PlayerStateType stateType) : base(stateMachine, stateType) { }

    public override void Enter()
    {
        movement.resetVelocity();
    }
}

public class DeadState : PlayerHierarchicalState
{
    public DeadState(PlayerStateMachine stateMachine, PlayerStateType stateType) : base(stateMachine, stateType) { }

    public override void Enter()
    {
        movement.deadStart();
    }

    public override void Exit()
    {
        movement.deadEnd();
    }
}
