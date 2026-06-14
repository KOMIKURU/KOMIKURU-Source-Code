using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }//シングルトンインスタンス

    void Awake()//シングルトンの初期化
    {
        // インスタンスの登録
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    // ゲームの状態を定義
    public enum GameState { Title, Tutorial, Countdown, Playing, GameEnd, Result }//ゲーム状態のenum
    public GameState CurrentState { get; private set; }//現在のゲーム状態

    [Header("タイトル画面UI")]
    [SerializeField] private CanvasGroup titlePanel;//タイトル画面のパネル

    [SerializeField] private UISlideIn elImage;//エルの画像スライドインコンポーネント
    [SerializeField] private UISlideIn RokiImage;//ロキの画像スライドインコンポーネント
    [SerializeField] private UISlideIn LogoImage;//ロゴの画像スライドインコンポーネント
    [SerializeField] private ImageBlinker pressImage;
    [SerializeField] private ImageBlinker plusImage;
    [SerializeField] private ImageBlinker minusImage;

    [SerializeField] private Color plusColor;
    [SerializeField] private Color minusColor;
    [SerializeField] private Color pressColor;

    [Header("チュートリアル画面UI")]
    [SerializeField] private CanvasGroup tutorialPanel;
    [SerializeField] private ImageBlinker plusImage2;
    [SerializeField] private ImageBlinker minusImage2;
    [SerializeField] private ImageBlinker readyImage;

    [SerializeField] private UISlideIn elImage2;//エルの画像スライドインコンポーネント
    [SerializeField] private UISlideIn RokiImage2;//ロキの画像スライドインコンポーネント

    [Header("プレイング画面UI")]
    [SerializeField] private CanvasGroup playingPanel;

    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerImage;

    // ★追加：エルとロキのリアルタイムスコア用テキスト
    [SerializeField] private TextMeshProUGUI elPlayingScoreText;
    [SerializeField] private TextMeshProUGUI rokiPlayingScoreText;

    [Header("スコア演出UI")]
    [SerializeField] private GameObject scorePopupPrefab; // 「+10」とかが出るTMPのPrefab
    [SerializeField] private Transform elPopupContainer;  // エルのポップアップを出す位置（親）
    [SerializeField] private Transform rokiPopupContainer; // ロキのポップアップを出す位置（親）
    [SerializeField] private AudioClip scoreGainSE;       // スコア獲得時のピキーンって音

    // ★追加：リアルタイムスコアを保持するプロパティ
    public int ElScore { get; private set; }
    public int RokiScore { get; private set; }

    [Header("ゲームエンド画面UI")]
    [SerializeField] private CanvasGroup gameEndPanel;

    [Header("リザルト画面UI")]
    [SerializeField] private CanvasGroup resultPanel;
    [SerializeField] private UISlideIn resultImage;

    [SerializeField] private TextMeshProUGUI elResultText;
    [SerializeField] private TextMeshProUGUI rokiResultText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("クレジット画面UI")]
    [SerializeField] private CanvasGroup creditPanel;
    [SerializeField] private ImageBlinker komikuruLogoImage;

    [Header("ゲーム設定")]
    [SerializeField] private float gameTime; // ゲーム制限時間
    
    [SerializeField] private ElController elController;
    [SerializeField] private RokiController rokiController;

    [Header("BGM")]
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip gameBGM;
    [SerializeField] private AudioClip resultBGM;

    [Header("SE")]
    [SerializeField] private AudioClip pressSE;
    [SerializeField] private AudioClip goToTutorialSE;
    [SerializeField] private AudioClip countdownSE;
    [SerializeField] private AudioClip gameEndSE;

    [Header("後半戦演出UI")]
    [SerializeField] private RectTransform doubleTimeBanner; // 横から流れてくるバナーのRectTransform
    [SerializeField] private TextMeshProUGUI doubleTimeText;   // バナーの中のテキスト
    [SerializeField] private AudioClip slideInSE;             // 流れてくるときのSE

    private List<Joycon> joycons;//接続確認されているJoyconのリスト

    // 外部（Playerなど）から参照するフラグ
    public bool IsPlayerControllable { get; private set; }

    // ★変更：タイマーの残り時間をクラス全体から参照できるようにフィールドに変更
    private float currentTimer;

    // ★追加：後半戦の2倍バフを「固定」するための管理フラグ
    private bool isElDoubleBuff = false;
    private bool isRokiDoubleBuff = false;
    private bool checkedHalfTimeBuff = false; // 半分切った時の判定を1回だけに制限する用

    private async void Start()
    {
        joycons = JoyconManager.Instance.j;
        var token = this.GetCancellationTokenOnDestroy();
        ResetAllPanels();
        try
        {
            // ゲーム全体のループ（リザルトからタイトルへ戻るため）
            while (true)
            {
                await ShowCreditLoop(token);
                await ShowTitleLoop(token);
                await ShowTutorialLoop(token);
                await PlayGameLoop(token);
                await ShowResultLoop(token);
            }
        }
        catch (OperationCanceledException)
        {
            // 終了時のクリーンアップが必要ならここに書く
        }
    }


    private async UniTask ShowTitleLoop(CancellationToken token)
    {
        
        BGMManager.PlayBGM(titleBGM);
        elImage.SetupPosition();
        RokiImage.SetupPosition();
        LogoImage.SetupPosition();

        pressImage.SetColor(Color.white);
        plusImage.SetColor(Color.white);
        minusImage.SetColor(Color.white);

        pressImage.SetAlpha(0);
        plusImage.SetAlpha(0);
        minusImage.SetAlpha(0);

        
        ShowPanel(titlePanel);
        await FadeOutPanel(creditPanel);

        // 演出：エルとロキがスライドイン、そのあとロゴ
        await UniTask.WhenAll(
            elImage.PlaySlideIn().ToUniTask(cancellationToken: token),
            RokiImage.PlaySlideIn().ToUniTask(cancellationToken: token)
        );

        await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: token);
        await LogoImage.SimpleSlideIn().ToUniTask(cancellationToken: token);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: token);
        // 点滅開始
        var pressBlinkTween = pressImage.ImageBlink();
        var plusBlinkTween = plusImage.ImageBlink();
        var minusBlinkTween = minusImage.ImageBlink();

        bool plusPressed = false;
        bool minusPressed = false;

        await UniTask.WaitUntil(() =>
        {
            bool isPlusInput = Input.GetKeyDown(KeyCode.B) ||
                       (JoyconsConnected() && joycons[1].GetButtonDown(Joycon.Button.PLUS));

            if (!plusPressed &&isPlusInput)
            {
                SEManager.Play(pressSE);
                plusPressed = true;
                plusBlinkTween.Kill(); // 押されたら点滅を止める
                pressBlinkTween.Kill();
                pressImage.SetAlpha(1.0f);
                pressImage.SetColor(pressColor); // ここで色を変えるとよりわかりやすい
                plusImage.SetAlpha(1.0f); // 明るく固定
                plusImage.SetColor(plusColor); // ここで色を変えるとよりわかりやすい
                plusImage.HighLight();

            }

            bool isMinusInput = Input.GetKeyDown(KeyCode.A) ||
                       (JoyconsConnected() && joycons[0].GetButtonDown(Joycon.Button.MINUS));

            if (!minusPressed && isMinusInput)
                {
                SEManager.Play(pressSE);
                minusPressed = true;
                minusBlinkTween.Kill();
                pressBlinkTween.Kill();
                pressImage.SetAlpha(1.0f);
                pressImage.SetColor(pressColor); // ここで色を変えるとよりわかりやすい
                minusImage.SetAlpha(1.0f);
                minusImage.SetColor(minusColor);
                minusImage.HighLight();

            }

            // 両方押されたら true を返して待機終了
            return plusPressed && minusPressed;
        }, cancellationToken: token);
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
        SEManager.Play(goToTutorialSE);
        await UniTask.Delay(TimeSpan.FromSeconds(0.8f), cancellationToken: token);
    }

    private async UniTask ShowTutorialLoop(CancellationToken token)
    {
        plusImage2.SetColor(Color.white);
        minusImage2.SetColor(Color.white);
        readyImage.SetColor(Color.white);

        readyImage.SetAlpha(0);

        elImage2.SetupPosition();
        RokiImage2.SetupPosition();

        ShowPanel(tutorialPanel);
        
        await FadeOutPanel(titlePanel);

        bool plusPressed = false;
        bool minusPressed = false;

        var readyBlinkTween = readyImage.ImageBlink();

        await UniTask.WaitUntil(() =>
        {
            bool isPlusInput = Input.GetKeyDown(KeyCode.B) ||
                       (JoyconsConnected() && joycons[1].GetButtonDown(Joycon.Button.PLUS));

            if (!plusPressed && isPlusInput)
            {
                SEManager.Play(pressSE);
                plusPressed = true;
                plusImage2.SetAlpha(1.0f); // 明るく固定
                plusImage2.SetColor(plusColor); // ここで色を変えるとよりわかりやすい
                plusImage2.HighLight();
                readyBlinkTween.Kill();
                readyImage.SetAlpha(1.0f);
                readyImage.SetColor(pressColor); // ここで色を変えるとよりわかりやすい

                
                RokiImage2.PlaySlideIn().ToUniTask(cancellationToken: token);

            }

            bool isMinusInput = Input.GetKeyDown(KeyCode.A) ||
                       (JoyconsConnected() && joycons[0].GetButtonDown(Joycon.Button.MINUS));

            if (!minusPressed && isMinusInput)
            {
                SEManager.Play(pressSE);
                minusPressed = true;
                minusImage2.SetAlpha(1.0f);
                minusImage2.SetColor(minusColor);
                minusImage2.HighLight();
                readyBlinkTween.Kill();
                readyImage.SetAlpha(1.0f);
                readyImage.SetColor(pressColor); // ここで色を変えるとよりわかりやすい

                elImage2.PlaySlideIn().ToUniTask(cancellationToken: token);

            }

            // 両方押されたら true を返して待機終了
            return plusPressed && minusPressed;
        }, cancellationToken: token);

        await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
        //readyImage.HighLight2().ToUniTask(cancellationToken:token).Forget();
        SEManager.Play(goToTutorialSE);
        await UniTask.Delay(TimeSpan.FromSeconds(1.2f), cancellationToken: token);
    }

    private async UniTask PlayGameLoop(CancellationToken token)
    {
        // 準備
        countdownText.text = "";
        timerText.text = gameTime.ToString();

        // ★追加：ゲーム開始時にスコアをゼロクラッシュしてUIを更新
        ElScore = 0;
        RokiScore = 0;
        UpdatePlayingScoreUI();

        // ★追加：ゲーム開始時にバフ用のフラグをリセット
        isElDoubleBuff = false;
        isRokiDoubleBuff = false;
        checkedHalfTimeBuff = false;

        elController.Init();
        rokiController.Init();
        BGMManager.StopBGM();

        ShowPanel(playingPanel);

        await FadeOutPanel(tutorialPanel);
        
        // カウントダウン演出
        SEManager.Play(countdownSE);
        string[] counts = { "3", "2", "1", "GO!" };
        foreach (var text in counts)
        {
            countdownText.text = text;
            await countdownText.transform.DOScale(1.5f, 0.5f).From(0).SetEase(Ease.OutBack).ToUniTask(cancellationToken: token);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
        }
        countdownText.text = "";

        // 本編開始
        IsPlayerControllable = true;
        BGMManager.PlayBGM(gameBGM);

        currentTimer = gameTime;
        while (currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            timerImage.fillAmount = currentTimer / gameTime;
            timerText.text = $"{Mathf.Max(0, currentTimer):F0}";
            // ★追加：ゲーム時間が半分を切った「その瞬間だけ」スコアを比較して負けている方を確定
            if (!checkedHalfTimeBuff && currentTimer <= gameTime / 2f)
            {
                checkedHalfTimeBuff = true; // 二度とこの判定を通らないようにロック
                string targetName = "";
                if (ElScore < RokiScore)
                {
                    isElDoubleBuff = true;
                    targetName = "エル";
                }
                else if (RokiScore < ElScore)
                {
                    isRokiDoubleBuff = true;
                    targetName = "ロキ";
                }

                if (targetName != "")
                {
                    // ★演出メソッドを呼び出す（非同期で実行）
                    ShowDoubleTimeEffect(targetName).Forget();
                }
                // ※同点の場合はどちらにもバフはかかりません
            }
            await UniTask.Yield(token); // 1フレーム待機
        }

        // 終了
        IsPlayerControllable = false;
        BGMManager.StopBGM();
        SEManager.Play(gameEndSE);
        
        ShowPanel(gameEndPanel);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: token);
        await FadeOutPanel(playingPanel);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: token);
        await FadeOutPanel(gameEndPanel);
    }

    private async UniTask ShowResultLoop(CancellationToken token)
    {
        BGMManager.PlayBGM(resultBGM);
        resultImage.SetupPosition();

        elResultText.text = $"エルのスコア:{ElScore}";
        rokiResultText.text = $"ロキのスコア:{RokiScore}";
        if (ElScore > RokiScore)
        {
            resultText.text = "エルの勝ち！";
        }
        else if (ElScore < RokiScore)
        {
            resultText.text = "ロキの勝ち！";
        }
        else
        {
            resultText.text = "引き分け！";
        }
        ShowPanel(resultPanel);

        await resultImage.SimpleSlideIn().ToUniTask(cancellationToken: token);
        
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: token);
        bool plusPressed = false;
        bool minusPressed = false;

        await UniTask.WaitUntil(() =>
        {
            bool isPlusInput = Input.GetKeyDown(KeyCode.B) ||
                       (JoyconsConnected() && joycons[1].GetButtonDown(Joycon.Button.PLUS));

            if (!plusPressed && isPlusInput)
            {
                SEManager.Play(pressSE);
                plusPressed = true;
            }

            bool isMinusInput = Input.GetKeyDown(KeyCode.A) ||
                       (JoyconsConnected() && joycons[0].GetButtonDown(Joycon.Button.MINUS));

            if (!minusPressed && isMinusInput)
            {
                SEManager.Play(pressSE);
                minusPressed = true;
            }

            // 両方押されたら true を返して待機終了
            return plusPressed || minusPressed;
        }, cancellationToken: token);

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: token);

        await FadeInPanel(creditPanel);
        BGMManager.StopBGM();

    }

    private async UniTask ShowCreditLoop(CancellationToken token)
    {
        komikuruLogoImage.SetAlpha(0f);
        

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: token);
        await komikuruLogoImage.CreditBlink();
        ResetAllPanels();
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: token);


    }
    private async UniTask FadeInPanel(CanvasGroup cg)
    {
        cg.alpha = 0;
        await cg.DOFade(1, 0.5f).ToUniTask();
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private async UniTask FadeOutPanel(CanvasGroup cg)
    {
        await cg.DOFade(0, 0.5f).ToUniTask();
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    private void ShowPanel(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private void HidePanel(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    private void ResetAllPanels()
    {
        HidePanel(titlePanel);
        HidePanel(tutorialPanel);
        HidePanel(playingPanel);
        HidePanel(gameEndPanel);
        HidePanel(resultPanel);
        
    }

    private bool JoyconsConnected()
    {
        return joycons != null && joycons.Count >= 2;
    }

    // ★追加：スコアを加算してUIを更新するメソッド
    // ★修正：固定されたバフフラグ（isElDoubleBuff）を見るように変更
    public void AddElScore(int amount)
    {
        if (isElDoubleBuff)
        {
            amount *= 2;
        }

        ElScore += amount;
        UpdatePlayingScoreUI();

        // 演出を実行（第3引数に現在のバフ状態を渡す）
        TriggerScoreEffects(elPopupContainer, amount, isElDoubleBuff);
    }

    // ★修正：固定されたバフフラグ（isRokiDoubleBuff）を見るように変更
    public void AddRokiScore(int amount,bool penalty=false)
    {
        if (isRokiDoubleBuff)
        {
            amount *= 2;
        }

        RokiScore += amount;
        UpdatePlayingScoreUI();

        // 演出を実行（第3引数に現在のバフ状態を渡す）
        TriggerScoreEffects(rokiPopupContainer, amount, isRokiDoubleBuff,penalty);
    }

    private void TriggerScoreEffects(Transform container, int amount, bool isDouble,bool penalty=false)
    {
        if (scoreGainSE != null) SEManager.Play(scoreGainSE);

        if (scorePopupPrefab == null || container == null) return;

        // ポップアップを生成
        GameObject popup = Instantiate(scorePopupPrefab, container);
        popup.transform.localPosition = Vector3.zero;

        TextMeshProUGUI popupText = popup.GetComponent<TextMeshProUGUI>();
        if (popupText != null)
        {
            popupText.text = $"+{amount}";

            // ★修正：固定バフがかかっている時だけテキストを黄色にして豪華にする
            if (isDouble) popupText.color = Color.yellow;
            else if (penalty) popupText.color = Color.red;
             else popupText.color = Color.white;

            // DOTweenで上に移動しながらフェードアウト
            popup.transform.DOLocalMoveY(50f, 0.5f).SetEase(Ease.OutQuad);
            popupText.DOFade(0f, 0.2f).SetDelay(0.3f).OnComplete(() =>
            {
                Destroy(popup);
            });
        }
        else
        {
            Destroy(popup);
        }
    }

    private void UpdatePlayingScoreUI()
    {
        if (elPlayingScoreText != null) elPlayingScoreText.text = $"エル: {ElScore}";
        if (rokiPlayingScoreText != null) rokiPlayingScoreText.text = $"ロキ: {RokiScore}";
    }

    private async UniTaskVoid ShowDoubleTimeEffect(string playerName)
    {
        if (doubleTimeBanner == null) return;

        // テキストをセット
        doubleTimeText.text = $"残り半分！\n{playerName}のスコア2倍タイム！";

        // 初期位置（画面左外）
        doubleTimeBanner.anchoredPosition = new Vector2(-1750, 0);

        // SE再生
        if (slideInSE != null) SEManager.Play(slideInSE);

        // 1. 中央へスライドイン
        await doubleTimeBanner.DOAnchorPos(new Vector2(0, 0), 0.5f).SetEase(Ease.OutQuart).ToUniTask();

        // 2. しばらく待機
        await UniTask.Delay(TimeSpan.FromSeconds(1.2f));

        // 3. 右へスライドアウト
        await doubleTimeBanner.DOAnchorPos(new Vector2(1750, 0), 0.5f).SetEase(Ease.InQuart).ToUniTask();
    }
}
