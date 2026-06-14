using DG.Tweening;
using UnityEngine;
using System;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeUI;
    [SerializeField] private float fadeDuration = 0.5f;

    private static FadeManager Instance;

    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void FadeOut(Action onComplete = null)
    {
        if (Instance != null)
        {
            // インスタンスが存在する場合のみ処理を実行
            Instance.fadeUI.blocksRaycasts = true; // フェード中は操作をブロック
            Instance.fadeUI.DOFade(1, Instance.fadeDuration)
                .OnComplete(() => {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }
        else
        {
            Debug.Log("FadeManagerのインスタンスがありません。シーンに配置されているか確認してください。");
        }
    }

    /// <summary>
    /// 画面を透明にフェードイン（明転）します。
    /// </summary>
    public static void FadeIn()
    {
        if (Instance != null)
        {
            // インスタンスが存在する場合のみ処理を実行
            Instance.fadeUI.DOFade(0, Instance.fadeDuration)
                .OnComplete(() =>
                {
                    Instance.fadeUI.blocksRaycasts = false; // フェード完了後に操作ブロックを解除
                });
        }
        else
        {
            // エラー処理はFadeOutに任せるか、必要に応じて追加
        }
    }
}
