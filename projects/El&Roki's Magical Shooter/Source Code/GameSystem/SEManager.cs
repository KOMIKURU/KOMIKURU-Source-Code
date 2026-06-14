using UnityEngine;

public class SEManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private static SEManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // フェード用Canvasをシーン跨ぎで保持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Play(AudioClip clip)
    {
        Instance?.PlayInternal(clip);
    }

    private void PlayInternal(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
