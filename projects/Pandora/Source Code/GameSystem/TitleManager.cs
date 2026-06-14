using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private List<UIScreen> allScreens;//遷移しうる画面のリスト
    [SerializeField] private UIScreen firstScreen;//最初に表示する画面

    [SerializeField] private AudioClip TitleBGM;
    [SerializeField] private AudioClip TitleES;

    private UIScreen currentScreen;//現在表示中の画面
    public bool isTransitioning { get; private set; } = false;

    void Start()
    {
        // 全画面の初期化
        foreach (var screen in allScreens) screen.Initialize();
        BGMManager.PlayBGM(TitleBGM);
        ESManager.PlayES(TitleES);

        // 最初の画面を表示
        ShowFirstScreen().Forget();
    }

    public async UniTaskVoid ShowFirstScreen()
    {
        if (firstScreen == null || isTransitioning) return;

        isTransitioning = true; // 1. 最初にロックをかける
        currentScreen = firstScreen; // 2. 最初に現在の画面を確定させる

        // フェード時間を 1.5s に。
        // 第2引数の selectionDelay も少し長め（例: 1.0s）にするとより「溜め」が効きます
        await firstScreen.ShowTitleAsync(1f);

        isTransitioning = false; // 3. 終わったらロックを解除
    }

    // 引数に直接「開きたい画面のスクリプト」を渡す
    public async UniTaskVoid ChangeScreen(UIScreen nextScreen)
    {
        if (isTransitioning || currentScreen == nextScreen) return;
        isTransitioning = true;

        // 1. 今の画面を閉じる
        if (currentScreen != null)
        {
            await currentScreen.HideAsync();
        }

        // 2. 次の画面を開く
        await nextScreen.ShowSequenceAsync();

        currentScreen = nextScreen;
        isTransitioning = false;
    }

    // ButtonのOnClickなどから呼び出すためのラッパー
    public void OnClickChangeScreen(UIScreen nextScreen)
    {
        ChangeScreen(nextScreen).Forget();
    }

    public void GameStart()
    {
        gameStartAfterDelayAsync().Forget();
    }

    private async UniTask gameStartAfterDelayAsync()
    {
        if (isTransitioning) return;
        isTransitioning = true; // 以降の操作をブロック
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        if (currentScreen != null)
        {
            var cg = currentScreen.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.interactable = false;   // ボタンの反応を消す
                cg.blocksRaycasts = false; // マウスを無視する
            }
            await currentScreen.HideAsync();
        }
        FadeManager.FadeOut();
        BGMManager.FadeOutBGM(2.0f).Forget();
        ESManager.FadeOutES(2.0f).Forget();
        await UniTask.Delay(2000);
        await SceneManager.LoadSceneAsync("GameScene").ToUniTask();
    }

    public void ExitGame()
    {
        if (isTransitioning) return;
        Debug.Log("アプリケーションを終了します...");
        Application.Quit();
    }
}
