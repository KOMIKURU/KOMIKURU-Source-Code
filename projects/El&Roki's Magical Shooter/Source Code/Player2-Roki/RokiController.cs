using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class RokiController : MonoBehaviour// Rokiの操作を管理するクラス
{
    // Joycon関連
    private List<Joycon> joycons;//接続確認されているJoyconのリスト
    private int jc_ind = 1;//使用するJoyconのインデックス(右Joycon)

    // レーン移動関連
    private bool isMoving = false; // 移動中フラグを追加
    public int currentLane { private set; get; } = 1; // 0: left, 1: center, 2: right
    [SerializeField] private float laneDistance = 2.0f;// レーン間の距離
    [SerializeField] private float moveSpeed = 1.0f;// 移動速度
    [SerializeField] private float inputThreshold = 0.5f;// スティックの感度
    private float groundX=5f;//レーン移動の基準X座標

    // ジャンプ関連
    [SerializeField] private float jumpFirstVelocity;//ジャンプの初速度
    [SerializeField] private float gravity;//重力
    [SerializeField] private AudioClip jumpSE;//ジャンプの効果音
    [SerializeField] private AudioClip groundedSE;//着地の効果音
    [SerializeField] private AudioClip damagedSE;
    private float verticalVelocity = 0f;//現在の垂直方向の速度
    private bool isGrounded = true;//接地フラグ
    private float groundY=-2.5f;//ジャンプの基準Y座標

    // 二段ジャンプ関連
    [SerializeField] private float airJumpFirstVelocity;//ジャンプの初速度
    private bool canAirJump = true;
    private bool airJumpReady = false;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private Transform shieldSpawnPoint;
    [SerializeField] private Transform upShieldSpawnPoint;

    private float shieldCooldown = 5f;//魔法のクールタイム
    private float shieldTimer = 0f;//魔法のクールタイム管理用タイマー

    public void Init()
    {
        isMoving = false;
        currentLane = 1;
        verticalVelocity = 0f;//現在の垂直方向の速度
        isGrounded = true;//接地フラグ
        canAirJump = true;
        airJumpReady = false;
        shieldTimer= 0f;//魔法のクールタイム管理用タイマー

        transform.position = new Vector3(groundX, groundY, transform.position.z); // 初期位置を基準位置に設定

        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        shieldTimer = Mathf.Max(0, shieldTimer - Time.deltaTime);
        // 【追加】プレイ中（IsPlayerControllableがtrue）でないなら、以降の処理（入力検知）を無視する
        if (!isGrounded)
        {
            // 速度に重力を加算し続ける
            verticalVelocity += gravity * Time.deltaTime;

            // 位置を更新
            transform.Translate(new Vector3(0, verticalVelocity * Time.deltaTime, 0));

            // 地面（初期位置）より下にいかないように判定
            if (transform.position.y <= groundY)
            {
                SEManager.Play(groundedSE);
                transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
                verticalVelocity = 0;
                isGrounded = true;
                isMoving = false; // ジャンプ終了
                canAirJump = true; // 二段ジャンプのリセット
                airJumpReady = false; // 二段ジャンプの準備フラグをリセット
            }
        }
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.IsPlayerControllable)
        {
            return;
        }
        if (joycons.Count > 0 && jc_ind < joycons.Count)
        {
            Joycon j = joycons[jc_ind];
            float[] stick = j.GetStick();
            float verticalInput = stick[1];
            float horizontalInput = stick[0];

            // ==================== レーン移動 ====================
            if (!isMoving)
            {
                if (verticalInput > inputThreshold && currentLane < 2)
                {
                    currentLane++;
                    isMoving = true;
                    Debug.Log("Moved Right: Current Lane = " + currentLane);
                }
                else if (verticalInput < -inputThreshold && currentLane > 0)
                {
                    currentLane--;
                    isMoving = true;
                    Debug.Log("Moved Left: Current Lane = " + currentLane);
                }
                
            }

            float targetX = groundX + (currentLane - 1) * laneDistance;
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
            if (Mathf.Abs(transform.position.x - targetX) < 0.01f)
            {
                isMoving = false; // 移動完了
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // ==================== ジャンプ ====================
            if (j.GetButtonDown(Joycon.Button.DPAD_UP) && isGrounded)
            {
                Debug.Log("Jump");
                SEManager.Play(jumpSE);
                verticalVelocity = jumpFirstVelocity;
                isGrounded = false;
                isMoving = true;
            }

            //if (j.GetButtonDown(Joycon.Button.DPAD_RIGHT) && shieldTimer <= 0f)
            //{
            //    // 💡第4引数に「shieldSpawnPoint」を追加して、子オブジェクトとして生成する
            //    GameObject shield;
            //    if (horizontalInput < -inputThreshold)
            //    {
                    
            //        shield = Instantiate(shieldPrefab, upShieldSpawnPoint.position, shieldSpawnPoint.rotation, upShieldSpawnPoint);
            //        shield.transform.localRotation = Quaternion.Euler(0, 0, 90); // 右向き
            //    }
            //    else
            //    {
            //        shield = Instantiate(shieldPrefab, shieldSpawnPoint.position, shieldSpawnPoint.rotation, shieldSpawnPoint);
            //    }
            //        shieldTimer = shieldCooldown;
            //}

            if (j.GetButtonUp(Joycon.Button.DPAD_UP) && !isGrounded&&canAirJump)
            {
                airJumpReady = true; // 二段ジャンプの準備ができたことを示すフラグをオンにする
            }
            else if (j.GetButtonDown(Joycon.Button.DPAD_UP) && !isGrounded && airJumpReady)
            {
                Debug.Log("Air Jump!");
                SEManager.Play(jumpSE);
                verticalVelocity = airJumpFirstVelocity;
                airJumpReady = false; // 二段ジャンプの準備フラグをリセット
                canAirJump = false; // 二段ジャンプを使用した後はフラグをオフにする
            }
            
        }
    }

    public void Damaged(int addScore)
    {
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.IsPlayerControllable)
        {
            return;
        }
        Joycon j = joycons[jc_ind];
        SEManager.Play(damagedSE);
        j.SetRumble(160, 320, 1.0f, 200);

        // ★追加：エルに加算されたスコアを反映
        GameFlowManager.Instance.AddElScore(addScore);
        animator.Play("Damaged");
    }

    public void Warning(int time)
    {
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.IsPlayerControllable)
        {
            return;
        }
        Joycon j = joycons[jc_ind];
        j.SetRumble(160, 320, 0.6f, time);
    }
}
