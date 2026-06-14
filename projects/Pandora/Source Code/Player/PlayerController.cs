using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private EnvironmentContext env;
    [SerializeField] private PlayerMovement movement; // 旧 action から変更
    [SerializeField] private PlayerVisuals visuals; // 新規追加
    private PlayerAbilityContext abilityContext; // 新規追加
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private Animator animator;

    [SerializeField] private Health health = new Health();
    [SerializeField] private KnockbackDetector normalDamageDealer;
    [SerializeField] private KnockbackDetector downDamageDealer;
    [SerializeField] private KnockbackDetector upDamageDealer;
    [SerializeField] private PlayerDamageReceiver damageReceiver;
    [SerializeField] private HPView HPView;

    [SerializeField] private MagicPoint magicPoint;
    [SerializeField] private MagicGenerate magicGenerate;

    [SerializeField] private MagicElement magicElement;

    [SerializeField] private MagicRingController ringController;


    [SerializeField] private SelectedElement selectedElement;

    [SerializeField] private OptionPanelScript optionPanelScript;

    [SerializeField] private int HP;

    [SerializeField] private MPUI MPUI;

    public PlayerStateMachine stateMachine { get; private set; }

    public event Action OnPlayerDied;


    private void Awake()
    {
        abilityContext = new PlayerAbilityContext(env);
        stateMachine = new PlayerStateMachine(env, movement, visuals, abilityContext, input,animator);
        magicPoint = new MagicPoint(45, 5, 1);
        health.Init(HP);
        damageReceiver.Init(health);

        health.OnDied += OnDeath;

        HPView.InitUI(health.MaxHP);
        health.OnHPDamaged += HPView.DamageUI;
        health.OnHPHealed += HPView.HealUI;

        selectedElement.Init();
        magicElement = new MagicElement(selectedElement);
        ringController.Init(selectedElement,magicElement);
        selectedElement.DifferentList += magicElement.initializeIndex; //選択した魔法属性のリストが変更した時にChangeElementがバグらないため
        selectedElement.DifferentList += ringController.SetElementRing; //魔法の輪を更新するため


        input.OnRightMagicChangePressed += ringController.RightRotateZ; //魔法の輪が反時計回りするように
        input.OnLeftMagicChangePressed += ringController.LeftRotateZ; //上とほぼ同じ（反時計回り→時計回り）

        magicGenerate.Init(magicPoint,magicElement);
        input.OnMagicGeneratePressed += magicGenerate.magicGanerate;

        normalDamageDealer.OnHitEnemy += magicPoint.TakeMP;
        downDamageDealer.OnHitEnemy += magicPoint.TakeMP;
        upDamageDealer.OnHitEnemy += magicPoint.TakeMP;

        normalDamageDealer.OnPlayerRecoiled += OnRecoiled;
        downDamageDealer.OnPlayerRecoiled += OnRecoiled;

        magicPoint.OnMPChanged += MPUI.ChangeMPView;

        input.OnOpenOptionPressed += optionPanelScript.OpenOption;
        input.OnCloseOptionPressed += optionPanelScript.CloseOption;
    }

    private void Update()
    {
        abilityContext.CheckAbilityCondition();
        stateMachine.Update();
    }


    public void OnRestart()
    {
        health.Init(HP);
        HPView.InitUI(health.MaxHP);
        stateMachine.ChangeState(PlayerStateType.Platformer);
        visuals.Default();
    }

    private void OnDeath()
    {
        Debug.Log($"{name} died!");
        OnPlayerDied?.Invoke();
        visuals.Invincible();
        stateMachine.ChangeState(PlayerStateType.Dead);
        UniTask.Delay(TimeSpan.FromSeconds(2)).ContinueWith(() =>
        {
            FadeManager.FadeOut(() =>
            {
                OnRestart();
                FadeManager.FadeIn();
            });
        });
    }

    private void OnRecoiled(Vector2 playerRecoilDirection,float playerRecoilForce)
    {
        
        Vector2 playerRecoilVector;
        if (playerRecoilDirection.y > 0)
        {
            stateMachine.AbilityContext.GiveAirJump();
            stateMachine.AbilityContext.GiveDash();
            playerRecoilVector = new Vector2(stateMachine.Movement.Rb.linearVelocityX, 15);
            stateMachine.Movement.knockBack(playerRecoilVector);
        }
        else
        {
            playerRecoilVector = new Vector2(playerRecoilForce * Mathf.Sign(playerRecoilDirection.x), 0);
            stateMachine.Movement.knockBack(playerRecoilVector);
            stateMachine.ChangeState(PlayerStateType.Recoiled);
        }
    }

    private Vector2 _forcedDir;

    public void StartForcedMoveNormal(Vector2 direction)
    {
        stateMachine.ChangeState(PlayerStateType.Interact);
        _forcedDir = direction;
        movement.Rb.linearVelocity = Vector2.zero;
        if (_forcedDir.x != 0&& _forcedDir.y ==0)
        {
            movement.move(_forcedDir.x);
        }
        else if (_forcedDir.y > 0 && _forcedDir.x != 0)
        {
            visuals.ForceSetFacingDirection(_forcedDir.x);
            movement.jump(15f);
        }
    }

    public void StartForcedMoveAnim(Vector2 direction)
    {
        stateMachine.ChangeState(PlayerStateType.Interact);
        _forcedDir = direction;
        movement.Rb.linearVelocity = Vector2.zero;
        if (_forcedDir.x != 0 && _forcedDir.y == 0)
        {
            visuals.ForceSetFacingDirection(_forcedDir.x);
            movement.move(_forcedDir.x);
            visuals.SetAnimation("Running");

        }
        else if (_forcedDir.y > 0 && _forcedDir.x != 0)
        {
            visuals.ForceSetFacingDirection(_forcedDir.x);
            movement.move(_forcedDir.x);
            movement.jump(15f);
            visuals.SetAnimation("Jump");
        }
    }

    // 強制移動を停止する
    public void StopForcedMove()
    {
        stateMachine.ChangeState(PlayerStateType.Platformer);
    } 
}

