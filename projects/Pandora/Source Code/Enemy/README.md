# Enemy

敵キャラクターの行動を管理するシステム。ここでは最も基本的な敵キャラクター「スライム(Slime)」のコードを紹介する。スライムの挙動は以下の通り。
- 基本的には地面を徘徊。
- 視界にプレイヤーを捉えると追跡を開始する。

プレイヤーにも用いたEnvironmentContext.csやDamageDealer.csも利用している。基本構造は階層型ステートマシン。その内部で宣言するステートの引数にストラテジーパターンを用いることで同じステートでも異なる挙動を表現できるようにしている。(例えば、突進攻撃をする敵Aと投擲攻撃をする敵Bがいた時にそれぞれに専用のステートを作るのではなく、「攻撃する」というインターフェース、つまりはストラテジーを用意することにより拡張性を高めている。)

## 📁 ファイル構成

```
Enemy/
├── DamageDealer.cs
├── EnemyDamageReceiver.cs
├── EnemyMovement.cs
├── EnemyStateMachine.cs
├── EnemyStateType.cs
├── EnemyVisuals.cs
├── EnvironmentContext.cs
├── PlayerDetectContext.cs
├── PlayerDetector.cs
├── README.md
├── SlimeStateMachine.cs
│
├── State/
│   ├── EnemyChaseState.cs
│   ├── EnemyDeadState.cs
│   ├── EnemyGiveUpState.cs
│   ├── EnemyHierarchicalState.cs
│   ├── EnemyIdleState.cs
│   ├── EnemyMoveState.cs
│   ├── EnemyNoticeState.cs
│   ├── EnemyPatrolState.cs
│   └── EnemyTurnState.cs
│
└── Strategy/
    ├── IEnemyIdleStrategy.cs
    └── IEnemyMovementStrategy.cs
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
| MagicGenerate.cs | 魔法の生成。チームメンバーと共同制作。 |
| MagicRingController.cs | 魔法リングUIの制御。チームメンバーと共同制作。|
| SelectedElement.cs | 選択されている魔法属性の管理。チームメンバーと共同制作。 |
| OptionPanelScript.cs | 魔法設定画面の制御コード。チームメンバーと共同制作。 |
| PlayerInteraction.cs | NPC・オブジェクトとのインタラクション |
