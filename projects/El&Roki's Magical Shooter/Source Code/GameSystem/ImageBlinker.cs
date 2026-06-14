using UnityEngine;
using UnityEngine.UI; // Imageを使用するために必要
using DG.Tweening;

public class ImageBlinker : MonoBehaviour
{
    [SerializeField] private Image targetImage; // 点滅させたいImage
    [SerializeField] private float duration = 0.5f; // 点滅の速さ

    // 外部からアルファ値を直接セットしたい場合（初期化用など）
    public void SetAlpha(float value)
    {
        if (targetImage == null) return;
        Color c = targetImage.color;
        c.a = value;
        targetImage.color = c;
    }

    public Tween HighLight()
    {
        if (targetImage == null) return null;
        targetImage.DOKill();
        targetImage.rectTransform.localScale = Vector3.one;

        // 1.3倍に大きくなってから、1.0倍に戻る（Punchスケール）
        // 学祭のようなキャッチーな演出に向いています
        return targetImage.rectTransform.DOPunchScale(new Vector3(0.5f, 0.5f, 0), 0.4f, 1, 0.5f);
    }

    public Tween HighLight2()
    {
        if (targetImage == null) return null;
        targetImage.DOKill();
        targetImage.rectTransform.localScale = Vector3.one;

        // 1.3倍に大きくなってから、1.0倍に戻る（Punchスケール）
        // 学祭のようなキャッチーな演出に向いています
        return targetImage.rectTransform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);
    }

    public void SetColor(Color color)
    {
        if (targetImage == null) return;
        targetImage.color = color;
    }

    public Tween ImageBlink()
    {
        if (targetImage == null) return null;

        // 既存のアニメーションを停止
        targetImage.DOKill();

        // 0(透明)から1(不透明)までループさせる演出
        // Imageの場合はDOFadeがImageコンポーネントに対して直接使えます
        return targetImage.DOFade(1f, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public Tween CreditBlink()
    {
        if (targetImage == null) return null;

        // 既存のアニメーションを停止し、初期状態（透明）にする
        targetImage.DOKill();

        // シーケンスの作成
        Sequence creditSequence = DOTween.Sequence();

        creditSequence
            // 1. フェードイン (0 -> 1)
            .Append(targetImage.DOFade(1f, duration).SetEase(Ease.InOutSine))
            // 2. 待機 (1秒待機させる場合。引数は秒数)
            .AppendInterval(2.0f)
            // 3. フェードアウト (1 -> 0)
            .Append(targetImage.DOFade(0f, duration).SetEase(Ease.InOutSine));

        return creditSequence;
    }

    // 演出を停止させたい時用
    public void StopBlink()
    {
        targetImage.DOKill();
    }
}
