using UnityEngine;

public abstract class PlayerHierarchicalState : IState
{
    protected PlayerStateMachine stateMachine;
    protected EnvironmentContext env;
    protected PlayerMovement movement;
    protected PlayerVisuals visuals;
    protected PlayerAbilityContext abilityContext;
    protected PlayerInputHandler inputHandler;

    public PlayerHierarchicalState parent { get; private set; }
    public PlayerHierarchicalState subState;

    public PlayerStateType StateType { get; protected set; }

    public PlayerHierarchicalState(PlayerStateMachine stateMachine, PlayerStateType stateType, PlayerHierarchicalState parent = null)
    {
        this.stateMachine = stateMachine;
        this.parent = parent;
        env = this.stateMachine.Env;
        movement = this.stateMachine.Movement; // ActionManagerをMovementに置き換え
        visuals = this.stateMachine.Visuals; // Visualsを代入
        abilityContext = this.stateMachine.AbilityContext;
        inputHandler = this.stateMachine.InputHandler;
        StateType = stateType;
    }

    public virtual void Enter() { subState?.Enter(); }
    public virtual void Exit() { subState?.Exit(); }
    public virtual void Update() { subState?.Update(); }

    // 子を切り替える
    public void ChangeSubState(PlayerStateType newSubState)
    {
        if (stateMachine.GetState(newSubState).parent != this) { Debug.LogWarning(newSubState + "は" + this + "の子ステートではありません。"); return; }
        subState?.Exit();
        subState = stateMachine.GetState(newSubState);
        //Debug.Log($"Change Sub State: {newSubState}");
        subState.Enter();
    }

}

