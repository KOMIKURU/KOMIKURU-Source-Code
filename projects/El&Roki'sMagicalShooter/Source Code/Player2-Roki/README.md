# Player2-Roki

「ロキ」の操作を管理するシステム。

<img src="../../Images/roki.png" width="300">

## 📁 ファイル構成

```
Player2-Roki/
├── RokiController.cs
├── RokiAvoidanceReceiver.cs
└── RokiDamageReceiver.cs
```

### コードの説明
| ファイル | 説明 |
|---------|------|
| RokiController.cs | ロキの操作の主体。ステートマシンなども使っていない神クラス。時間制限があったので仕方ないと思っている。 |
| RokiAvoidanceReceiver.cs | 魔法を回避したことを判定 |
| RokiDamageReceiver.cs | 魔法に被弾したことを判定 |
