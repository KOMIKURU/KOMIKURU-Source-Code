# Game System

ゲーム全体の進行管理、音声、画面遷移などを担当するシステム。

## 📁 ファイル構成

```
GameSystem/
├── BGMManager.cs
├── BGMPlayer.cs
├── ESManager.cs
├── FadeManager.cs
├── GameStateManager.cs
├── README.md
├── SEManager.cs
└── TitleManager.cs
```

### 各ファイルの役割

| ファイル | 説明 |
|---------|------|
| GameStateManager.cs | ゲーム内のフラグを管理する(会話でクエスト解禁など(未実装)) |
| TitleManager.cs | タイトル画面の制御 |
| BGMManager.cs | BGM の一元管理。シーン遷移時の切り替えなどを担当 |
| BGMPlayer.cs | BGM の再生・停止などの実処理 |
| SEManager.cs | SE（効果音）の一元管理・再生 |
| ESManager.cs | 環境音(Environmental Sound)の管理 |
| FadeManager.cs | 画面のフェードイン・フェードアウト処理 |

## 💡 工夫点

- 演出に関わるManager をシングルトンとして実装し、シーンを跨いでも状態を保持できるよう設計
- BGM / SE / 環境音を役割ごとに分離し、それぞれ独立して管理できる構成
- FadeManager によって画面遷移時の演出を一元化し、各シーンから簡単に呼び出せるようにした
