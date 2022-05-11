using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    void SetBuff(TimedBuff buff);
    void SetNerf(TimedNerf nerf);
    void TakeDamage(float dmg, DamageType type);
    void TakeRecover(float val);
    void BuffAndNerfTick();
    IEnumerator AttackAction(DamageType type, Action onComplete);
    IEnumerator DefendAction(Action onComplete);
    IEnumerator MagicAction(Action onComplete);
    void PlayAnimation(string name);
    void DisableUI();
    void ResetUnit();
}
