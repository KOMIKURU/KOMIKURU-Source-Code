using UnityEngine;

public interface IEnemyMovementStrategy
{
    void ExecuteMovement();
    void Enter();
    void Exit();
}

public class LinearWalkerStrategy : IEnemyMovementStrategy
{
    private EnemyMovement movement;
    private EnemyVisuals visuals;
    private float moveSpeed;

    public LinearWalkerStrategy(EnemyMovement enemyMovement,EnemyVisuals visuals, float moveSpeed = 3)
    {
        this.movement = enemyMovement;
        this.visuals = visuals;
        this.moveSpeed = moveSpeed;
    }

    public void Enter()
    {
        // 特に初期化処理はなし
    }

    public void Exit()
    {
        movement.SetVelocityX(0f);
    }

    // 地面を歩くロジック
    public void ExecuteMovement()
    {
        float currentDirection = visuals.EnemyDirection;
        float targetVelocityX = currentDirection * moveSpeed;
        movement.SetVelocityX(targetVelocityX);
    }
}

public class RandomWalkerStrategy : IEnemyMovementStrategy
{
    private EnemyMovement movement;
    private EnemyVisuals visuals;
    private float moveSpeed;

    public RandomWalkerStrategy(EnemyMovement enemyMovement,EnemyVisuals visuals, float moveSpeed = 3)
    {
        this.movement = enemyMovement;
        this.visuals = visuals;
        this.moveSpeed = moveSpeed;
    }

    public void Enter()
    {
        int randomNumber = Random.Range(1, 3);
        float direction = randomNumber * 2 - 3; // 1→-1, 2→1
        visuals.changeEnemyDirection(direction);
    }

    public void Exit()
    {
        movement.SetVelocityX(0f);
    }

    // 地面を歩くロジック
    public void ExecuteMovement()
    {
        float currentDirection = visuals.EnemyDirection;
        float targetVelocityX = currentDirection * moveSpeed;
        movement.SetVelocityX(targetVelocityX);
    }
}

// 空中で一定のパターンで浮遊する敵のための移動ストラテジー
public class AerialFloaterStrategy : IEnemyMovementStrategy
{
    private EnemyMovement movement;
    private float floatSpeed = 1.5f; // 浮遊の基本速度
    private float floatHeightAmplitude = 2f; // 上下の振幅
    private float floatFrequency = 2f; // 浮遊の振動周期
    private float fryDuration;

    private float timeElapsed = 0f;

    public AerialFloaterStrategy(EnemyMovement enemyMovement, float speed = 1.5f,float fryDuration=2)
    {
        this.movement = enemyMovement;
        this.floatSpeed = speed;
        this.fryDuration = fryDuration;
    }

    public void Enter()
    {
        timeElapsed = Random.Range(0f, 10f); // 敵ごとに浮遊開始タイミングをずらす
        // 重力の影響を無視するため、Rigidbodyの重力スケールを0にするなどの処理が本来は必要
        // ただし、ここでは既存のEnemyMovementのSetVelocityYを使用
    }

    public void Exit()
    {
        movement.SetVelocityY(0f);
    }

    // 空中でふわふわと浮遊するロジック
    public void ExecuteMovement()
    {
        timeElapsed += Time.deltaTime;
        float verticalOffset = Mathf.Sin(timeElapsed * floatFrequency) * floatHeightAmplitude;
        float targetVelocityY = verticalOffset * floatSpeed;
        movement.SetVelocityY(targetVelocityY);
    }
}

public class FloaterStrategy : IEnemyMovementStrategy//タユタ専用ステート
{
    private EnemyMovement movement;
    private EnemyVisuals visuals;
    private EnvironmentContext env;

    private float floatSpeed;
    private float floatDuration;
    private float timer = 0f;
   

    public FloaterStrategy(EnemyMovement enemyMovement,EnemyVisuals visuals,EnvironmentContext env, float speed = 7f)
    {
        this.movement = enemyMovement;
        this.visuals = visuals;
        this.env = env;
        this.floatSpeed = speed;
       
        floatDuration = Random.Range(5f, 5.5f);
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    // 空中でふわふわと浮遊するロジック
    public void ExecuteMovement()
    {
        timer += Time.deltaTime;
        if (timer >= floatDuration || (env.IsGrounded||env.IsInWater))
        {
            visuals.SetAnimation("FryUp");// 浮遊アニメーション
            movement.SetVelocityY(floatSpeed);
            timer = 0f;
        }

    }
}


// 一定間隔で「グッ」と加速し、また元の速度に戻る動き
public class IntervalBurstStrategy : IEnemyMovementStrategy
{
    private EnemyMovement movement;
    private EnemyVisuals visuals;

    private string dashAnimation;

    private float baseSpeed;      // 通常時の速度
    private float burstSpeed;     // 加速時の最大速度
    private float burstDuration;  // 加速している時間（秒）
    private float interval;       // 加速が起きる間隔（秒）

    private float timer = 0f;
    private bool isBursting = false;
    private float burstTimer = 0f;

    public IntervalBurstStrategy(EnemyMovement enemyMovement, EnemyVisuals visuals, string dashAnimation, float baseSpeed = 2f, float burstSpeed = 6f, float interval = 3f, float burstDuration = 1f)
    {
        this.movement = enemyMovement;
        this.visuals = visuals;

        this.dashAnimation = dashAnimation;

        this.baseSpeed = baseSpeed;
        this.burstSpeed = burstSpeed;
        this.interval = interval;
        this.burstDuration = burstDuration;
    }

    public void Enter()
    {
        timer = 0f;
        burstTimer = 0f;
        isBursting = false;
    }

    public void Exit()
    {
        movement.SetVelocityX(0f);
    }

    public void ExecuteMovement()
    {
        float currentSpeed = baseSpeed;

        // タイマー更新
        if (!isBursting)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                // 加速開始
                isBursting = true;
                timer = 0f;
                burstTimer = 0f;
                visuals.SetAnimation(dashAnimation);
            }
        }
        else
        {
            // 加速中
            burstTimer += Time.deltaTime;

            // 進捗率 (0.0 ～ 1.0)
            float progress = burstTimer / burstDuration;

            if (progress >= 1f)
            {
                // 加速終了、定速に戻る
                isBursting = false;
                currentSpeed = baseSpeed;
            }
            else
            {
                // Sinカーブを使って滑らかに Base -> Burst -> Base と変化させる
                // Mathf.Sin(progress * Mathf.PI) は 0 -> 1 -> 0 の動きをする
                float speedAdd = (burstSpeed - baseSpeed) * Mathf.Sin(progress * Mathf.PI);
                currentSpeed = baseSpeed + speedAdd;
            }
        }

        // 現在の向きに合わせて速度を適用
        float currentDirection = visuals.EnemyDirection;
        movement.SetVelocityX(currentDirection * currentSpeed);
    }
}
