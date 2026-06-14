using UnityEngine;
public class SlimeStateMachine : EnemyStateMachine
{
    [SerializeField] private PlayerDetectContext detectContext;
    public override void InitializeStates()
    {
        var patrolState = new EnemyPatrolState(this,EnemyStateType.Move,EnemyStateType.Notice,detectContext);
        var noticeState = new EnemyNoticeState(this, EnemyStateType.Combat);
        var combatState = new EnemyCombatState(this, new SlimeDecisionStrategy(), detectContext);

        var moveState = new EnemyMoveState(this, new RandomWalkerStrategy(Movement,Visuals, EnemyData.moveSpeed),true,3,patrolState);
        var idleState = new EnemyIdleState(this, new SimpleIdleStrategy(Movement),1,patrolState);
        var turnState = new EnemyTurnState(this, EnemyStateType.Patrol);

        var chaseState = new EnemyChaseState(this,new LinearWalkerStrategy(Movement,Visuals ,EnemyData.moveSpeed*2), combatState);
        var giveUpState = new EnemyGiveUpState(this,EnemyStateType.Patrol,combatState);

        var deadState = new EnemyDeadState(this);
        

        
        cachedStates.Add(EnemyStateType.Patrol, patrolState);
        cachedStates.Add(EnemyStateType.Notice, noticeState);
        cachedStates.Add(EnemyStateType.Combat, combatState);
        
        cachedStates.Add(EnemyStateType.Move, moveState);
        cachedStates.Add(EnemyStateType.Idle, idleState);
        cachedStates.Add(EnemyStateType.Turn, turnState);

        cachedStates.Add(EnemyStateType.Chase, chaseState);
        cachedStates.Add(EnemyStateType.GiveUp, giveUpState);

        cachedStates.Add(EnemyStateType.Dead, deadState);
        

        // 初期状態は移動
        ChangeState(EnemyStateType.Patrol);
    }

    public override void TakeDamage(int amount, Vector2 recoilVector)
    {
        // 攻撃を食らったらノックバックステートへ強制遷移
        base.TakeDamage(amount, recoilVector);
        if (Health.IsDead) return;
        if(CurrentState is EnemyCombatState) return;
        Visuals.ReverseDirection();
        ChangeState(EnemyStateType.Notice);

    }

    public override void PlayerTouchEnemy()
    {
        if (CurrentState is EnemyCombatState) return;
        Visuals.ReverseDirection();
        ChangeState(EnemyStateType.Notice);
    }


}
