using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    void SetBuff(TimedBuff buff);
    void SetNerf(TimedNerf nerf);
    void TakeDamage(float dmg, DamageType type);
    void TakeRecover(float val);
    IEnumerator AttackAction(DamageType type);
    IEnumerator MagicAction();
    void PlayAnimation(string name, bool active);
}
