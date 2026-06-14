using UnityEngine;
using System;
using System.Linq; // LinqはFirstOrDefault()のために必要です

public class PlayerDetector : MonoBehaviour
{
    private bool detectPlayer;//Playerを検知したかどうか
    public float raycastCheckInterval = 0.2f;//RayCastの間隔
    private float timer = 0f;

    public event Action<bool> OnPlayerDetected;
    public event Action<Transform> OnPlayerTargetChanged;
    public event Action<float> OnDistanceToPlayerChanged;

    public LayerMask obstacleLayer;
    public LayerMask playerDetectionMask;

    [SerializeField] private BoxCollider2D detectionCollider;//検知範囲,視界
    [SerializeField] private EnemyVisuals enemyVisuals;
    private Collider2D playerInArea = null;//検知範囲にPlayerがいた際の参照
    private Transform playerTargetTransform = null;
    private float distanceToPlayer;



    private void Update()
    {
        CheckDetectionRange();
        CalcDistance();
        if (playerInArea != null)
        {
            timer += Time.deltaTime;
            if (timer >= raycastCheckInterval)
            {
                CheckLineOfSight(playerInArea.transform.position);
                timer = 0f;
            }
        }
        else
        {
            // プレイヤーが範囲外に出た場合、または範囲内にいない場合
            SetDetection(false);
        }
    }

    //範囲内にPlayerがいるか？
    private void CheckDetectionRange()
    {
        float direction =enemyVisuals.EnemyDirection;
        Vector2 baseOffset = detectionCollider.offset;
        Vector2 adjustedOffset = new Vector2(
            baseOffset.x * direction, // ★修正: オフセットを方向で調整★
            baseOffset.y
        );

        Vector2 position = (Vector2)transform.position + adjustedOffset;
        Vector2 size = detectionCollider.size;
        float angle = transform.eulerAngles.z;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(position, size, angle, playerDetectionMask);
        Collider2D foundPlayer = hitColliders.FirstOrDefault(col => col.CompareTag("Player"));
        playerInArea = foundPlayer;
        if (foundPlayer != null && playerTargetTransform == null)
        {
            playerTargetTransform = foundPlayer.transform;
            OnPlayerTargetChanged?.Invoke(playerTargetTransform);
        }
    }

    //視線に障害物があるか？
    private void CheckLineOfSight(Vector2 targetPosition)
    {
        Vector2 origin = transform.position;
        if (playerInArea != null)
        {
            targetPosition = playerInArea.bounds.center;
        }
        Vector2 direction = (targetPosition - origin).normalized;
        float distance = Vector2.Distance(origin, targetPosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleLayer);

        if (hit.collider != null)
        {
            SetDetection(false); // 障害物に当たった
        }
        else
        {
            SetDetection(true);  // プレイヤーが見える
        }
    }

    private void SetDetection(bool isDetected)
    {
        if (detectPlayer != isDetected)
        {
            detectPlayer = isDetected;
            OnPlayerDetected?.Invoke(detectPlayer);
        }
    }

    private void CalcDistance()
    {
        if (playerTargetTransform == null) return;
        Vector2 origin = transform.position;
        Vector2 targetPosition = playerTargetTransform.position;
        Vector2 direction = (targetPosition - origin).normalized;
        float distance = Vector2.Distance(origin, targetPosition);
        distanceToPlayer = distance;
        OnDistanceToPlayerChanged?.Invoke(distanceToPlayer);
    }

    public void DeletePlayerTransform()
    {
        playerTargetTransform = null;
    }

    // Gizmos描画ロジック (変更なし)
    void OnDrawGizmos()
    {
        DrawDetectionGizmos();
    }

    void OnDrawGizmosSelected()
    {
        DrawDetectionGizmos();
    }

    private void DrawDetectionGizmos()
    {
        if (detectionCollider == null) return;

        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(detectionCollider.offset, detectionCollider.size);
        Gizmos.matrix = Matrix4x4.identity;

        if (playerInArea == null) return;

        Vector2 origin = transform.position;
        Vector2 targetPosition = playerInArea.bounds.center;
        Vector2 direction = (targetPosition - origin).normalized;
        float distance = Vector2.Distance(origin, targetPosition);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleLayer);

        if (hit.collider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, hit.point);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, targetPosition);
        }
    }
}
