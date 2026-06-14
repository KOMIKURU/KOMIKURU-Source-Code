using UnityEngine;
using TMPro;
using DG.Tweening;

public class TMPBlinker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private float duration = 0.5f; // “_–Å‚Ì‘¬‚³

    public void SetAlpha(float value)
    {
        targetText.alpha=value;
    }
    public Tween TMPBlink()
    {
        targetText.DOKill();
        return targetText.DOFade(1f, duration)
            .SetEase(Ease.InOutSine) // ‚¶‚ñ‚í‚è‚³‚¹‚é‚È‚ç‚±‚ê
            .SetLoops(-1, LoopType.Yoyo);
    }
}
