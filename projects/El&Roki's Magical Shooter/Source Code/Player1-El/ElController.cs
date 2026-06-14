using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ElController : MonoBehaviour// Elの操作を管理するクラス
{
    // Joycon関連
    private List<Joycon> joycons;//接続確認されているJoyconのリスト
    private int jc_ind = 0;//使用するJoyconのインデックス(左Joycon)

    // ジャンプ関連
    [SerializeField] private float jumpFirstVelocity;//ジャンプの初速度
    [SerializeField] private float gravity;//重力
    [SerializeField] private AudioClip jumpSE;//ジャンプの効果音
    [SerializeField] private AudioClip groundedSE;//着地の効果音
    [SerializeField] private AudioClip warningSE;
    [SerializeField] private AudioClip normalMagicSE;
    [SerializeField] private AudioClip chargedMagicSE;
    [SerializeField] private AudioClip chargedMagicFireSE;
    [SerializeField] private AudioClip upMagicSE;
    private float verticalVelocity = 0f;//現在の垂直方向の速度
    private bool isGrounded = true;//接地フラグ
    private float groundY=-2.5f;//ジャンプの基準Y座標

    [SerializeField] private Transform magicShotPoint;

    [SerializeField] private GameObject warningObject;
    [SerializeField] private GameObject warningObject2;

    //通常魔法用
    [SerializeField] private GameObject magicBullet;//通常魔法弾のPrefab
    private float magicCooldown = 0.5f;//魔法のクールタイム
    private float magicTimer = 0f;//魔法のクールタイム管理用タイマー

    //溜め魔法用
    [SerializeField] private GameObject chargedMagicBullet;//溜め魔法のPrefab
    private float chargeTimer = 0f;// 押し続けた時間
    private bool isCharging = false;// チャージ中フラグ
    private bool isCharged = false;//魔法チャージ完了フラグ
    private float magicChargeTime = 0.6f;// 溜め魔法に要するチャージ時間

    //上魔法用
    [SerializeField] private GameObject upMagicBullet;
    [SerializeField] private GameObject downMagicBullet;
    [SerializeField] private RokiController roki;
    private float upDownMagicTimer = 0f;// 押し続けた時間
    private float upDownMagicChargeTime = 0.85f;// 溜め魔法に要するチャージ時間

    private float jumpBufferCounter; // 先行入力を受け付ける残り時間
    [SerializeField] private float jumpBufferTime = 0.15f; // 先行入力を許容する時間（秒）

    [SerializeField] private Animator animator;

    // ★追加：サボり防止ペナルティ用の設定
    [Header("サボり防止ペナルティ設定")]
    [SerializeField] private float maxNoAttackTime; // 何秒攻撃しないとペナルティが始まるか
    [SerializeField] private float penaltyInterval; // ペナルティが発生する間隔（0.5秒ごと）
    [SerializeField] private int penaltyScoreAmount;  // ロキに入るペナルティスコア

    private float noAttackTimer = 0f;   // 攻撃していない時間を測るタイマー
    private float penaltyTickTimer = 0f; // ペナルティの間隔を測るタイマー

    [SerializeField] private float inputThreshold = 0.5f;// スティックの感度
    public void Init()
    {
        verticalVelocity = 0f;
        isGrounded = true;
        magicTimer = 0f;
        chargeTimer = 0f;
        isCharging = false;
        isCharged = false;
        upDownMagicTimer = 0f;

        // ★追加：タイマーの初期リセット
        ResetAttackTimer();

        transform.position = new Vector3(transform.position.x, groundY, transform.position.z); // 初期位置を基準位置に設定

        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {

        // 1. 入力処理（先行入力のタイマー更新含む）
        HandleInput();

        // 2. 物理・移動計算（接地判定、重力、ジャンプ実行）
        ApplyPhysics();

        // 3. アニメーションの状態更新
        UpdateAnimation();

        // ★追加：サボり監視システムの実行
        MonitorAttackNeglect();
    }

    private void MonitorAttackNeglect()
    {
        // プレイ中でないならタイマーは動かさない
        if (GameFlowManager.Instance == null || !GameFlowManager.Instance.IsPlayerControllable)
        {
            ResetAttackTimer();
            return;
        }

        // 攻撃していない時間をカウント
        noAttackTimer += Time.deltaTime;

        // 指定時間を超えたら、ロキにじわじわ点数が入る
        if (noAttackTimer >= maxNoAttackTime)
        {
            penaltyTickTimer += Time.deltaTime;
            if (penaltyTickTimer >= penaltyInterval)
            {
                penaltyTickTimer = 0f;

                // ロキにスコアを献上！
                GameFlowManager.Instance.AddRokiScore(penaltyScoreAmount,true);
                Debug.Log($"エルが攻撃をサボっているため、ロキに {penaltyScoreAmount} 点加算！");
            }
        }
    }

    private void ResetAttackTimer()
    {
        noAttackTimer = 0f;
        penaltyTickTimer = 0f;
    }
    private void HandleInput()
    {
        // タイマー類の更新
        jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - Time.deltaTime);
        magicTimer = Mathf.Max(0, magicTimer - Time.deltaTime);
        upDownMagicTimer = Mathf.Max(0, upDownMagicTimer - Time.deltaTime);

        // 操作不能な場合は入力を受け付けない
        if (!GameFlowManager.Instance.IsPlayerControllable) return;
        if (joycons == null || joycons.Count <= jc_ind) return;

        Joycon j = joycons[jc_ind];
        float[] stick = j.GetStick();
        float horizontalInput = stick[0];

        // ジャンプ先行入力
        if (j.GetButtonDown(Joycon.Button.DPAD_UP))
        {
            jumpBufferCounter = jumpBufferTime;
        }

        // 通常魔法 & チャージ開始
        if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
        {
            //// 上魔法
            //if (inputThreshold < horizontalInput && upMagicTimer <= 0)
            //{
            //    FireDownMagic();
            //}
            //else if (horizontalInput < -inputThreshold && upMagicTimer <= 0)
            //{
            //    FireUpMagic();
            //}
            //else if(Mathf.Abs(horizontalInput) <= inputThreshold)
            //{
            //    HandleMagicAttack(j);
            //}
            HandleMagicAttack(j);
        }

        if (j.GetButton(Joycon.Button.DPAD_RIGHT) && upDownMagicTimer <= 0)
        {
            FireUpMagic();
        }

        if (j.GetButton(Joycon.Button.DPAD_LEFT) && upDownMagicTimer <= 0)
        {
            FireDownMagic();
        }

        // チャージ継続
        if (j.GetButton(Joycon.Button.DPAD_DOWN) && isCharging)
        {
            chargeTimer -= Time.deltaTime;
            if (!isCharged && chargeTimer <= 0)
            {
                isCharged = true;
                j.SetRumble(160, 320, 0.6f, 150);
                animator.SetTrigger("MagicCharged");
                SEManager.Play(chargedMagicSE); // チャージ完了の効果音（必要に応じて変更）
                roki.Warning(200); // ロキに警告を出す（必要に応じて変更）
            }
        }

        // チャージ解放
        if (j.GetButtonUp(Joycon.Button.DPAD_DOWN) && isCharging)
        {
            if (isCharged) FireChargedMagic(j);
            isCharging = false;
            isCharged = false;
        }

        
    }

    private void ApplyPhysics()
    {
        // 接地判定の更新
        if (transform.position.y <= groundY)
        {
            if (!isGrounded)
            {
                SEManager.Play(groundedSE);
                animator.SetTrigger("OnGrounded");
                isGrounded = true;
                verticalVelocity = 0;
                transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
            }
        }
        else
        {
            isGrounded = false;
        }

        // ジャンプの実行（先行入力があり、かつ接地している場合）
        if (jumpBufferCounter > 0 && isGrounded)
        {
            SEManager.Play(jumpSE);
            verticalVelocity = jumpFirstVelocity;
            jumpBufferCounter = 0;
        }

        // 重力の適用
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        transform.Translate(0, verticalVelocity * Time.deltaTime, 0);
    }

    // --- 各アクションのロジックを切り出し ---

    private void HandleMagicAttack(Joycon j)
    {
        ResetAttackTimer();
        isCharging = true;
        isCharged = false;
        chargeTimer = magicChargeTime;

        if (magicTimer <= 0)
        {
            magicTimer = magicCooldown;
            GameObject bullet=Instantiate(magicBullet,magicShotPoint.position, Quaternion.identity);
            MagicBullet mb = bullet?.GetComponent<MagicBullet>();

            Joycon j_sub = joycons[jc_ind];
            float[] stick = j_sub.GetStick();
            float verticalInput = stick[1];

            if(verticalInput > inputThreshold)
            {
                mb.ChangeSpeed(7);
            }
            else if(verticalInput < -inputThreshold)
            {
                mb.ChangeSpeed(15);
            }


            SEManager.Play(normalMagicSE);
            j.SetRumble(160, 320, 0.4f, 200);
            animator.SetTrigger("Magic");
        }
    }

    private void FireChargedMagic(Joycon j)
    {
        ResetAttackTimer();
        //Transform spawnPoint = isGrounded ? magicShotPoint1 : magicShotPoint2;
        Instantiate(chargedMagicBullet,magicShotPoint.position, Quaternion.identity);
        SEManager.Play(chargedMagicFireSE);
        j.SetRumble(160, 320, 1.0f, 300);
        magicTimer = magicCooldown;
        animator.SetTrigger("ChargedMagic");
    }

    private void FireUpMagic()
    {
        ResetAttackTimer();
        upDownMagicTimer = upDownMagicChargeTime;
        animator.SetTrigger("UpMagic");
        ExecuteUpMagicAsync().Forget();
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }

    private async UniTaskVoid ExecuteUpMagicAsync()
    {
        float targetX = 5.0f + (roki.currentLane - 1) * 2.0f;
        Vector3 spawnPos = new Vector3(targetX, 0, 0);
        GameObject warning = Instantiate(warningObject, spawnPos, Quaternion.identity);
        SEManager.Play(warningSE);
        roki.Warning(150);
        await UniTask.Delay(TimeSpan.FromSeconds(0.45f));
        Instantiate(upMagicBullet, new Vector3(targetX, 6, 0), Quaternion.identity);
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        SEManager.Play(upMagicSE); // もし上魔法の発動音があればここで鳴らす
        if (warning != null) Destroy(warning);
    }

    private void FireDownMagic()
    {
        ResetAttackTimer();
        upDownMagicTimer = upDownMagicChargeTime;
        animator.SetTrigger("UpMagic");
        ExecuteDownMagicAsync().Forget();
    }

    private async UniTaskVoid ExecuteDownMagicAsync()
    {
        float targetX = 5.0f + (roki.currentLane - 1) * 2.0f;
        Vector3 spawnPos = new Vector3(targetX, 0, 0);
        GameObject warning = Instantiate(warningObject2, spawnPos, Quaternion.identity);
        SEManager.Play(warningSE);
        roki.Warning(150);
        await UniTask.Delay(TimeSpan.FromSeconds(0.45f));
        Instantiate(downMagicBullet, new Vector3(targetX, -6, 0), Quaternion.identity);
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        SEManager.Play(upMagicSE); // もし上魔法の発動音があればここで鳴らす
        if (warning != null) Destroy(warning);
    }

}