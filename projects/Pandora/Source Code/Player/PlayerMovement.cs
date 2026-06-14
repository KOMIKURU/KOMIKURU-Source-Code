using UnityEngine;

public class PlayerMovement:MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private float knockbackDecayRate = 0.9f;
    private float originalGravity;
    public Rigidbody2D Rb => rb;
    public Vector2 CurrentKnockbackVelocity { get; private set; } = Vector2.zero;

    public void move(float moveInput,float moveSpeed=7f)
    {
        float finalX = moveInput * moveSpeed + CurrentKnockbackVelocity.x;
        rb.linearVelocityX = finalX;
    }

    public void SetKnockbackDetail(Vector2 direction, float initialForce)
    {
        CurrentKnockbackVelocity = direction.normalized * initialForce;
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

    public void jump(float jumpSpeed)
    {
        rb.linearVelocityY = jumpSpeed;
    }

    public void jumpRelease()
    {
        rb.linearVelocityY *= 0.5f;
    }

    public void SetGravity(float gravity)
    {
        originalGravity = rb.gravityScale;
        rb.gravityScale = gravity;
    }

    public void ResetGravity()
    {
        rb.gravityScale = originalGravity;
    }

    public void dashStart(float moveInput, float dashSpeed)
    {
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(moveInput*dashSpeed, 0);
    }

    public void dashEnd()
    {
        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public void wallJump(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void wallJumpCancel()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
    }

    public void wallSliding()
    {
        rb.linearVelocityY = -2f;
    }

    public void knockBack(Vector2 force)
    {
        rb.linearVelocity = force;
    }

    public void resetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void deadStart()
    {
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
    }

    public void deadEnd()
    {
        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}
