# Player

Pandoraの主人公「エル」の操作、移動、状態管理を担当する最も複雑なシステム。
[エル](../../Images/El_NormalAttack_0005.png)
階層型有限ステートマシン(HFSM)のデザインパターンを参考に作成。アクションゲームのプレイヤーはユーザーからの入力、環境(地面にいるか、壁に触れているかなど)、ダメージ被弾など様々な要素を考慮しながらシステムを構築しなくてはならないので一番時間がかかった。基本的には中野陽(KOMIKURU)が作成。ただし、魔法攻撃に関連する部分はサークルの後輩(Silver)と共同で開発。システムの一部(EnvironmentContext.cs,DamageDealer.csなど)は、敵キャラクター（Enemy）にも再利用されており、プロジェクト全体の設計の基盤となっている。

## 📁 ファイル構成

```
Player/
├── DamageDealer.cs
├── DetectorBase.cs
├── EnvironmentContext.cs
├── GroundDetector.cs
├── MagicGenerate.cs
├── MagicRingController.cs
├── OptionPanelScript.cs
├── PlayerController.cs
├── PlayerDamageReceiver.cs
├── PlayerInputHandler.cs
├── PlayerInteraction.cs
├── PlayerMovement.cs
├── README.md
├── SelectedElement.cs
├── WallDetector.cs
├── WaterDetector.cs
└── State/
    ├── AirborneState.cs
    ├── GroundedState.cs
    ├── InWaterState.cs
    ├── PlatformerState.cs
    ├── PlayerHierarchicalState.cs
    ├── SpecialActionState.cs
    └── WallState.cs
```


### コア制御
| ファイル | 説明 |
|---------|------|
| PlayerController.cs | Playerの主体。ステートマシンの初期化やHPの管理などを担う。 |
| PlayerInputHandler.cs | キーボード入力の処理 |
| PlayerMovement.cs | 移動・重力・速度管理 |

### 状態管理（State/フォルダ）
| ファイル | 説明 |
|---------|------|
| PlayerHierarchicalState.cs | 階層型ステートマシンのステート基盤 |
| GroundedState.cs | 地面に接地している状態 |
| AirborneState.cs | 空中にいる状態 |
| WallState.cs | 壁に張り付いている状態 |
| InWaterState.cs | 水中にいる状態 |
| PlatformerState.cs | 基本的なプラットフォーマー状態 |
| SpecialActionState.cs | 特殊アクション時の状態。ダッシュなど。 |

### 検知・判定
| ファイル | 説明 |
|---------|------|
| DetectorBase.cs | 検知の基底クラス |
| GroundDetector.cs | 地面判定 |
| WallDetector.cs | 壁判定 |
| WaterDetector.cs | 水判定 |
| EnvironmentContext.cs | プレイヤーの周辺環境情報を統合する。これを参照すれば接地状態や壁に触れているかなどが分かる。 |

### 攻撃・ダメージ
| ファイル | 説明 |
|---------|------|
| DamageDealer.cs | プレイヤー・敵共通の攻撃判定 |
| PlayerDamageReceiver.cs | プレイヤーへのダメージを受け取る。UnityではDamageDealerとPlayerDamageReceiverは同一のレイヤーに配置されている。 |

### 魔法・その他
| ファイル | 説明 |
|---------|------|
| MagicGenerate.cs | 魔法の生成。中野: 基本設計、Silver: 実装・詳細パラメータ調整 |
| MagicRingController.cs | 魔法リングUIの制御。中野: 基本設計・UI連携部分、Silver: 実装・詳細パラメータ調整|
| SelectedElement.cs | 選択されている魔法属性の管理。中野: 基本設計、Silver: 実装・詳細パラメータ調整 |
| OptionPanelScript.cs | 魔法設定画面の制御コード。中野: 基本設計・UI連携部分、Silver: 実装・詳細パラメータ調整 |
| PlayerInteraction.cs | NPC・オブジェクトとのインタラクション |
