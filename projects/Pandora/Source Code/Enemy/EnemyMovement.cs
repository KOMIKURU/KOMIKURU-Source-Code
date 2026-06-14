using UnityEngine;

public class EnemyMovement
{
    protected Rigidbody2D rb;
    private float knockbackDecayRate = 0.9f; // ノックバック速度の減衰率
    public Vector2 CurrentKnockbackVelocity { get; private set; } = Vector2.zero;

    public EnemyMovement(Rigidbody2D rb)
    {
        this.rb = rb;
    }

    public void SetKnockbackDetail(Vector2 direction, float initialForce)
    {
        CurrentKnockbackVelocity = direction.normalized * initialForce;
    }

    public void SetVelocityX(float velocityX)
    {
        float finalX = velocityX + CurrentKnockbackVelocity.x;
        rb.linearVelocity = new Vector2(finalX, rb.linearVelocity.y);
    }

    

    public void SetVelocityY(float velocity)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocity);
    }

    public void SetGravityScale(float gravityScale)
    {
        rb.gravityScale = gravityScale;
    }

    public void KnockbackUpdate()
    {
        if (CurrentKnockbackVelocity.magnitude > 0.1f)
        {
            CurrentKnockbackVelocity *= knockbackDecayRate;
        }
        else
        {
            CurrentKnockbackVelocity = Vector2.zero;
        }
    }

    public void ApplyKnockback()
    {
        rb.linearVelocity = new Vector2(CurrentKnockbackVelocity.x, rb.linearVelocity.y);
    }


    //private float verticalVelocity; // 現在の垂直速度
    //public void ApplyGravity(float gravityForce, float maxFallSpeed, bool isGrounded)
    //{
    //    if (isGrounded)
    //    {
    //        // 接地していたら落下速度をわずかなマイナス（地面に押し付ける力）で固定
    //        verticalVelocity = 0f;
    //    }
    //    else
    //    {
    //        // 自由落下：速度を加算し、最大落下速度でクランプ（制限）する
    //        verticalVelocity -= gravityForce * Time.fixedDeltaTime;
    //        verticalVelocity = Mathf.Max(verticalVelocity, -maxFallSpeed);
    //    }
    //}

    //public void FixedUpdateMovement()
    //{
    //    rb.linearVelocity = new Vector2(rb.linearVelocityX, verticalVelocity);
    //}
}
