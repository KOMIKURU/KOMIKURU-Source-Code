# Game System

ゲームそのものを管理したり、演出などに用いるシステムを構築。

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
| EnemyVisuals.cs | 敵キャラクターの見た目や進行方向を管理 |
| EnemyMovement.cs | 移動・重力・速度管理 |

### 状態管理（State/フォルダ）
| ファイル | 説明 |
|---------|------|
| EnemyStateMachine.cs| 敵キャラクターのステートマシン基底クラス|
| SlimeStateMachine.cs| スライムのステートマシン|
| EnemyStateType.cs | ステートを区別するラベル |
| EnemyHierarchicalState.cs | 階層型ステートマシンのステート基盤 |
| EnemyPatrolState.cs | パトロール状態 |
| EnemyMoveState.cs | 徘徊状態 |
| EnemyIdleState.cs | 待ち状態 |
| EnemyTurnState.cs | 徘徊時に壁などに当たって反転する状態 |
| EnemyNoticeState.cs | プレイヤーに気づいた状態 |
| EnemyChaseState.cs | プレイヤーを追跡する状態 |
| EnemyGiveUpState.cs | プレイヤーの追跡を諦めた状態 |
| EnemyDeadState.cs | 死亡状態 |

### ストラテジー（Strategy/フォルダ）
| ファイル | 説明 |
|---------|------|
| IEnemyIdleStrategy.cs| 待ちに関するストラテジー(今のところ「ただ止まるだけ」の単一のストラテジーのみ)|
| IEnemyMovementStrategy.cs| 徘徊に関するストラテジー(ex.線形に移動、上下に浮遊など)|

### 検知・判定
| ファイル | 説明 |
|---------|------|
| EnvironmentContext.cs | 敵キャラクターの周辺環境情報を統合する。これを参照すれば接地状態や壁に触れているかなどが分かる。(詳しくはPlayerを参照) |
| PlayerDetectContext.cs | プレイヤー検知情報を統合 |
| PlayerDetector.cs | プレイヤー検知。視界そのもの。 |

### 攻撃・ダメージ
| ファイル | 説明 |
|---------|------|
| DamageDealer.cs | プレイヤー・敵共通の攻撃判定 |
| EnemyDamageReceiver.cs | 敵へのダメージを受け取る。UnityではDamageDealerとEnemyDamageReceiverは同一のレイヤーに配置されている。 |
