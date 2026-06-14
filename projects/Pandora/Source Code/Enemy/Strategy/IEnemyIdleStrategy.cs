using UnityEngine;

public interface IEnemyIdleStrategy
{
    // ステートのUpdate()で毎フレーム呼び出され、移動ロジックを実行する
    void ExecuteIdle();

    // ストラテジーが初期化やクリーンアップが必要な場合のために
    void Enter();
    void Exit();
}

public class SimpleIdleStrategy : IEnemyIdleStrategy
{
    private EnemyMovement movement;

    public SimpleIdleStrategy(EnemyMovement enemyMovement)
    {
        this.movement = enemyMovement;
    }

    public void Enter()
    {
        // 特に初期化処理はなし
        movement.SetVelocityX(0f);
    }

    public void Exit()
    {

    }

    // 地面を歩くロジック
    public void ExecuteIdle()
    {
        //movement.SetVelocityX(0f);
    }
}
