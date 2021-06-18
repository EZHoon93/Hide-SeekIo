using System.Collections;

using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected SeekerAttack _seekerAttack;
    protected string _attackAnimationName;
    protected float _attackDelayTime;   //대미지 주기까지 시간
    protected float _afterAttackDelayTime;  //다음움직임 까지 시간

    public string AttackAnim => _attackAnimationName;
    public float AttackDelay => _attackDelayTime;
    public float AfaterAttackDelay => _afterAttackDelayTime;
    public virtual void SeutpSeekr(SeekerAttack newSeekerAttack)
    {
        _seekerAttack = newSeekerAttack;
    }

    public abstract void Zoom(Vector2 inputVector);
    public abstract void Attack(Vector2 inputVector);





}
