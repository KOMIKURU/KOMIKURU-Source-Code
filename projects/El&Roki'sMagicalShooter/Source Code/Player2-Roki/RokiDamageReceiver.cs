using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RokiDamageReceiver : MonoBehaviour
{
    [SerializeField] private RokiController controller;
    [SerializeField] private int score; // ダメージ量（必要に応じて使用）

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("接触物を検知");
        controller.Damaged(score);
        // ★修正：当たったオブジェクト、またはその親に「MagicBullet」スクリプトが付いているか探す
        MagicBullet bullet = other.GetComponentInParent<MagicBullet>();

        if (bullet != null)
        {
            // 魔法弾の本体（親オブジェクト）を確実に消滅させる
            Destroy(bullet.gameObject);
        }
    }
}
