using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyDamageReceiver : MonoBehaviour
{
    [SerializeField] private EnemyStateMachine stateMachine;

    [SerializeField] private float playerRecoilForce;

    public float PlayerRecoilForce => playerRecoilForce;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out DamageDealer damageDealer))
        {
            Vector2 enemyRecoilDirection = (transform.position - other.transform.position).normalized;
            Vector2 enemyRecoilVector;

            if (Mathf.Abs(enemyRecoilDirection.x) >= Mathf.Abs(enemyRecoilDirection.y))
            {
                enemyRecoilVector = new Vector2(damageDealer.RecoilForce * Mathf.Sign(enemyRecoilDirection.x), 0f);
            }
            else
            {
                enemyRecoilVector = new Vector2(damageDealer.RecoilForce * Mathf.Sign(enemyRecoilDirection.x), 0f);
                //enemyRecoilVector = new Vector2(0f, damageDealer.RecoilForce * Mathf.Sign(enemyRecoilDirection.y));
            }

            Debug.Log("Enemy Recoil");
            stateMachine.TakeDamage(damageDealer.Damage, enemyRecoilVector);
            return;

        }

        if (other.CompareTag("Player"))
        {
            stateMachine.PlayerTouchEnemy();
        }

    }
}

