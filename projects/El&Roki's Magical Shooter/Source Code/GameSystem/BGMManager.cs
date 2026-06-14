using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    // AudioSourceの参照 (インスペクターで設定)
    [SerializeField] private AudioSource audioSource;

    // シングルトンインスタンス
    private static BGMManager Instance;
    private float defaultVolume;

    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            defaultVolume = audioSource.volume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlayInternal(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        if (audioSource.isPlaying && audioSource.clip == clip) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;

        // ★修正ポイント：再生開始時に音量を元に戻す
        audioSource.volume = defaultVolume;
        audioSource.Play();
    }

    public static async UniTask FadeOutBGM(float duration)
    {
        if (Instance != null && Instance.audioSource != null)
        {
            // DOTweenを使用して音量を0にする
            await Instance.audioSource.DOFade(0f, duration).SetUpdate(true).AsyncWaitForCompletion();
            Instance.audioSource.Stop();
            Instance.audioSource.clip = null;
        }
    }

    public static void PlayBGM(AudioClip clip) => Instance?.PlayInternal(clip);

    public static void StopBGM() => Instance?.StopBGMInternal();

    private void StopBGMInternal()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }
}
