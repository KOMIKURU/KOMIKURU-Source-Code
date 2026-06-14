using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RokiAvoidanceReceiver : MonoBehaviour
{
    [SerializeField] private int scoreToAdd; // 回避成功時に加算するスコア
    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイ中でないなら処理しない
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.IsPlayerControllable)
        {
            return;
        }

        // 接触したオブジェクト（またはその親）に「MagicBullet」スクリプトが付いているか確認
        MagicBullet bullet = other.GetComponentInParent<MagicBullet>();
        
        if (bullet != null)
        {
            // ★ロキが躱した！その場でリアルタイムに11点加算！
            GameFlowManager.Instance.AddRokiScore(scoreToAdd);
            Debug.Log("ナイス回避！ロキにスコア加算");
        }
    }
}