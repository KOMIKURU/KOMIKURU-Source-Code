using UnityEngine;

public class PlayerDetectContext : MonoBehaviour
{
    public bool DetectPlayer { get; private set; }//Player‚ðŒŸ’m‚µ‚½‚©‚Ç‚¤‚©
    public Transform PlayerTargetTransform;//Player‚ÌTransformŽQÆ
    public float DistanceToPlayer { get; private set; }//Player‚Æ‚Ì‹——£

    [SerializeField] private PlayerDetector playerDetector;

    private void Awake()
    {
        if (playerDetector != null)
        {
            playerDetector.OnPlayerDetected += HandlePlayerDetected;
            playerDetector.OnPlayerTargetChanged += HandlePlayerTargetChanged;
            playerDetector.OnDistanceToPlayerChanged += HandleDistanceToPlayerChanged;
        }
    }

    private void HandlePlayerDetected(bool value) => DetectPlayer = value;
    private void HandlePlayerTargetChanged(Transform target) => PlayerTargetTransform = target;
    private void HandleDistanceToPlayerChanged(float distance) => DistanceToPlayer = distance;

    public void DeletePlayerTransform()
    {
        PlayerTargetTransform = null;
        DistanceToPlayer = 0;
        playerDetector.DeletePlayerTransform();
    }

    private void OnDestroy()
    {
        if (playerDetector != null)
        {
            playerDetector.OnPlayerDetected -= HandlePlayerDetected;
            playerDetector.OnPlayerTargetChanged -= HandlePlayerTargetChanged;
            playerDetector.OnDistanceToPlayerChanged -= HandleDistanceToPlayerChanged;
        }

        //if (enemyDamageReceiver != null)
        //{
        //    enemyDamageReceiver.OnPlayerDetected -= HandlePlayerTargetChanged;
        //}
    }

}
