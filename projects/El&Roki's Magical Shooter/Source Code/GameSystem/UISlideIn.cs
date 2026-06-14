using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISlideIn : MonoBehaviour
{
    [SerializeField] private RectTransform targetImage;
    [SerializeField] private Vector2 startPosition = new Vector2(-1000, 0);
    [SerializeField] private Vector2 endPosition = Vector2.zero;
    [SerializeField] private float duration = 0.5f;

    [Header("浮遊アニメーションの設定")]
    [SerializeField] private float floatDistance = 20f; // 下に移動する距離
    [SerializeField] private float floatDuration = 2f; // 片道の時間

    public Tween SimpleSlideIn()
    {
        targetImage.DOKill();
        targetImage.anchoredPosition = startPosition;

        return targetImage.DOAnchorPos(endPosition, duration)
            .SetEase(Ease.OutBounce);
    }

    public void SetupPosition()
    {
        targetImage.DOKill();
        targetImage.anchoredPosition = startPosition;
    }

    public Tween PlaySlideIn()
    {
        targetImage.DOKill();

        // Tweenを返却しつつ、完了後に浮遊を開始させる
        return targetImage.DOAnchorPos(endPosition, duration)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                StartFloating();
            });
    }

    private void StartFloating()
    {
        // 現在の目標地点（endPosition）から、少し下に移動して戻るを繰り返す
        Vector2 downPosition = new Vector2(endPosition.x, endPosition.y - floatDistance);

        targetImage.DOAnchorPos(downPosition, floatDuration)
            .SetEase(Ease.InOutSine) // ゆっくり動くように設定
            .SetLoops(-1, LoopType.Yoyo); // 無限ループ（行って帰って）
    }

    // こちらも同様に戻り値を Tween に変更
    
}
