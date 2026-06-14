using NUnit;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStateMachine: MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected EnemyVisuals visuals;
    [SerializeField] protected EnvironmentContext env;

    [SerializeField] private EnemyData enemyData;

    [SerializeField] private EnemyDamageReceiver damageReceiver;
    [SerializeField] private GameObject hitbox;
    [SerializeField] private GameObject hurtbox;
    [SerializeField] private Health health = new Health();
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deadSound;

    protected EnemyMovement movement;
    public EnemyMovement Movement => movement;
    public EnemyVisuals Visuals => visuals;
    public EnvironmentContext Env => env;

    public EnemyData EnemyData => enemyData;

    public Health Health => health;

    public EnemyHierarchicalState CurrentState { get; protected set; }

    protected Dictionary<EnemyStateType, EnemyHierarchicalState> cachedStates = new();

    protected virtual void Awake()
    {
        movement = new EnemyMovement(rb);
        InitializeStates();
        health.Init(enemyData.HP);
    }

    public virtual void InitializeStates() { }

    public virtual void TakeDamage(int amount,Vector2 recoilVector)
    {
        movement.SetKnockbackDetail(recoilVector.normalized, enemyData.knockbackForce);
        if (health.IsDead) return;
        visuals.DamageEffectDOTweenAsync(Color.red, 0.5f);
        health.TakeDamage(amount);
        SEManager.Play(damageSound);
        if (health.IsDead)
        {
            OnDeath();
        }
    }

    public virtual void PlayerTouchEnemy() { }
    public virtual void FixedUpdate()
    {
        CurrentState?.Update();
    }

    public EnemyHierarchicalState GetState(EnemyStateType stateType)
    {
        if (cachedStates.TryGetValue(stateType, out EnemyHierarchicalState state))
        {
            return state;
        }
        Debug.Log($"State {stateType} not found in cache.");
        return null;
    }

    public void ChangeState(EnemyStateType newState)
    {
        if (GetState(newState).parent != null) { Debug.LogWarning(newState + "はルートステートではありません。"); return; }
        CurrentState?.Exit();
        CurrentState = GetState(newState);
        //Debug.Log($"Change Root State: {newState}");
        CurrentState.Enter();
    }

    public virtual void OnDeath()
    {
        Debug.Log($"{name} died!");
        movement.SetGravityScale(3.5f);
        hurtbox.SetActive(false);
        if (hitbox!=null)
        {
            hitbox.SetActive(false);
        }
        SEManager.Play(deadSound);
        ChangeState(EnemyStateType.Dead);
    }
}
