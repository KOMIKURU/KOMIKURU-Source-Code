using DG.Tweening;
using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerDetectContext playerDetectContext;
    [SerializeField] private float deadzone = 0.5f;

    public Animator Animator => animator;

    [SerializeField] private float enemyDirection=1;
    public float EnemyDirection => enemyDirection;

    private void Awake()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * EnemyDirection, transform.localScale.y, transform.localScale.z);
    }

    
    //public float EnemyDirection { get; private set; } = 1f;
    public void changeEnemyDirection(float direction)
    {
        if (direction == enemyDirection) return;
        enemyDirection = direction;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * EnemyDirection, transform.localScale.y, transform.localScale.z);
        Debug.Log("Change Enemy Sprite Direction");
    }

    public void DecideDirecttion()
    {
        enemyDirection = Random.Range(0, 2) * 2 - 1;
    }

    public void ReverseDirection()
    {
        float reverseDirectionX = -EnemyDirection;
        changeEnemyDirection(reverseDirectionX);
    }

    public void LookPlayer()
    {
        if (playerDetectContext.PlayerTargetTransform == null) return;
        float diffX = playerDetectContext.PlayerTargetTransform.position.x - transform.position.x;

        if (Mathf.Abs(diffX) > deadzone)
        {
            // 差がデッドゾーンより大きい時だけ向きを更新
            float newDir = Mathf.Sign(diffX);
            changeEnemyDirection(newDir);
        }
    }

    public void SetAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    public void DamageEffectDOTweenAsync(Color damageColor, float duration = 0.5f)
    {
        Debug.Log($"{gameObject.name} の色を変えます！");
        Color originalColor = Color.white;
        spriteRenderer.DOKill();

        // 1. 一瞬で色を変える
        spriteRenderer.color = damageColor;

        // 2. DOTweenで元の色へ戻す
        // .SetEase(Ease.OutQuad) をつけると、戻り際がより滑らかになります
        spriteRenderer.DOColor(originalColor, duration)
            .SetEase(Ease.OutQuad);
    }
}
