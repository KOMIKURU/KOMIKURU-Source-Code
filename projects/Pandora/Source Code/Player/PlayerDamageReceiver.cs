using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDamageReceiver : MonoBehaviour
{
    private Health health;
    [SerializeField] private PlayerController playerController;

    public void Init(Health health)
    {
        this.health = health;
    }

    public void ReceiveDamage(int damage)
    {
        health?.TakeDamage(damage);
    }
    private float a = 0.8f;

    private const float KnockBackForceX = 16f; // ノックバックの強さX
    private const float KnockBackForceY = 13f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("接触物を検知");
        if (other.TryGetComponent(out DamageDealer dealer))
        {
            playerController.stateMachine.ChangeState(PlayerStateType.Damaged);
            ReceiveDamage(dealer.Damage);
            if (health.currentHP - dealer.Damage <= 0) return;

            float hitDirectionX = Mathf.Sign(transform.position.x - other.transform.position.x);
            playerController.stateMachine.Visuals.ForceSetFacingDirection(-hitDirectionX);
            Vector2 knockBackVector = new Vector2(KnockBackForceX*a * hitDirectionX, a*KnockBackForceY);
            playerController.stateMachine.Movement.knockBack(knockBackVector);
            
        }
    }
}
