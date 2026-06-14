using UnityEngine;

public class EnvironmentContext : MonoBehaviour
{
    // --- 外部（PlayerMovementなど）が参照する情報 ---
    public bool IsGrounded { get; private set; }
    public Rigidbody2D GroundRb { get; private set; } // 足元のリフトなどの物理情報

    public bool IsTouchingLeftWall { get; private set; }
    public bool IsTouchingRightWall { get; private set; }
    public bool IsTouchingWall { get; private set; }

    public bool IsInWater { get; private set; }

    // --- インスペクターで設定する各センサー ---
    [Header("Detectors")]
    [SerializeField] private DetectorBase groundDetector;
    [SerializeField] private DetectorBase leftWallDetector;
    [SerializeField] private DetectorBase rightWallDetector;
    [SerializeField] private DetectorBase waterDetector;

    // --- エフェクト用：外部（PlayerEffectController）が購読するためのイベント公開 ---
    // Context自体はイベントを処理せず、Detectorのイベントをそのまま中継、またはDetectorを公開する

    public DetectorBase GroundDetector => groundDetector;
    public DetectorBase WaterDetector => waterDetector;
    // 必要ならWallDetectorも公開

    private void Update()
    {
        CheckEnvironment();
    }

    // 毎フレームセンサーの状態を確認して、自分のプロパティを更新する（Pull型）
    private void CheckEnvironment()
    {
        // 地面情報の更新
        if (groundDetector != null)
        {
            IsGrounded = groundDetector.IsDetected;
            GroundRb = groundDetector.CurrentExternalRb;
        }

        // 壁情報の更新
        if (leftWallDetector != null) IsTouchingLeftWall = leftWallDetector.IsDetected;
        if (rightWallDetector != null) IsTouchingRightWall = rightWallDetector.IsDetected;

        IsTouchingWall = IsTouchingLeftWall || IsTouchingRightWall;

        // 水情報の更新
        if (waterDetector != null) IsInWater = waterDetector.IsDetected;
    }
}


