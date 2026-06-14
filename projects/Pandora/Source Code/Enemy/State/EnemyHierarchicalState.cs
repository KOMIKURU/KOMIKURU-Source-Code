using UnityEngine;

public abstract class EnemyHierarchicalState : IState
{
    protected EnemyStateMachine stateMachine;

    public EnemyHierarchicalState parent { get; private set; }
    public EnemyHierarchicalState subState;

    public EnemyHierarchicalState(EnemyStateMachine stateMachine, EnemyHierarchicalState parent = null)
    {
        this.stateMachine = stateMachine;
        this.parent = parent;
    }

    public virtual void Enter() { subState?.Enter(); }
    public virtual void Exit() { subState?.Exit(); }
    public virtual void Update() { subState?.Update(); }

    // 子を切り替える
    public void ChangeSubState(EnemyStateType newSubState)
    {
        if (stateMachine.GetState(newSubState).parent != this) { Debug.LogWarning(newSubState + "は" + this + "の子ステートではありません。"); return; }
        subState?.Exit();
        subState = stateMachine.GetState(newSubState);
        Debug.Log($"Change Sub State: {newSubState}");
        subState.Enter();
    }

    // 親に「切り替えたい」と通知
    public void RequestTransition(EnemyStateType nextState)
    {
        if (stateMachine.GetState(nextState).parent != this.parent) { Debug.LogWarning(nextState + "は" + this + "と同レベルのステートではありません。"); return; }
        parent?.ChangeSubState(nextState);
    }
}
