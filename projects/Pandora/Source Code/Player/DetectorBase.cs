using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DetectorBase : MonoBehaviour
{
    [Tooltip("検知対象のレイヤー")]
    [SerializeField] private LayerMask detectionMask;

    private List<Collider2D> currentColliders = new List<Collider2D>();

    // 外部から「今検知しているか？」を即座に知るためのプロパティ
    public bool IsDetected => currentColliders.Count > 0;

    // エフェクト再生用：状態が変わった瞬間だけ呼ばれるイベント
    // true = 検知開始（着地した、水に入った）
    // false = 検知終了（離れた、水から出た）
    public event Action<bool> OnDetectedStatusChanged;

    // リフトやギミック処理用：触れている相手の詳細情報
    public Collider2D CurrentCollider => IsDetected ? currentColliders[0] : null;
    public Rigidbody2D CurrentExternalRb
    {
        get
        {
            if (!IsDetected) return null;
            // 触れているものの中にRigidbody持ちがいればそれを返す（リフト移動用）
            // リストの最後（最新）のものを優先するなど調整も可能だが、基本は0番目でOK
            return currentColliders[0].attachedRigidbody;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 指定されたレイヤーか確認
        if (!IsTargetLayer(other)) return;

        // 2. まだリストになければ追加
        if (!currentColliders.Contains(other))
        {
            currentColliders.Add(other);

            // 3. 「0個から1個になった瞬間」＝「着地した瞬間」なのでイベント発火
            if (currentColliders.Count == 1)
            {
                OnDetectedStatusChanged?.Invoke(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 1. 指定されたレイヤーか確認
        if (!IsTargetLayer(other)) return;

        // 2. リストにあれば削除
        if (currentColliders.Contains(other))
        {
            currentColliders.Remove(other);

            // 3. 「空っぽになった瞬間」＝「完全に離れた瞬間」なのでイベント発火
            if (currentColliders.Count == 0)
            {
                OnDetectedStatusChanged?.Invoke(false);
            }
        }
    }

    private void OnDisable()
    {
        currentColliders.Clear();
    }

    private bool IsTargetLayer(Collider2D other)
    {
        return (detectionMask.value & (1 << other.gameObject.layer)) != 0;
    }
}