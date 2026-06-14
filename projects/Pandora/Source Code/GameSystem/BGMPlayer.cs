using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip BGM;
    void Start()
    {
        BGMManager.PlayBGM(BGM);

    }

}
