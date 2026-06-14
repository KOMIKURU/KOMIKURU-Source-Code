using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageDealer : MonoBehaviour
{
    //攻撃などによるダメージ量の設定
    [SerializeField] private int damage = 10;
    public int Damage => damage;

    [Header("敵をどのくらい弾き飛ばすか")]
    [SerializeField] private float recoilForce = 5f; // インスペクタで調整可能にする
    public float RecoilForce => recoilForce;
}

