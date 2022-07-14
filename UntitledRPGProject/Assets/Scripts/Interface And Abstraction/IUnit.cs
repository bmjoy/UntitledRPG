using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    void SetBuff(TimedBuff buff);
    void SetNerf(TimedNerf nerf);
    bool TakeDamage(float dmg, DamageType type);
    void TakeDamageByTrap(float dmg);
    void TakeRecover(float val);
    void TakeRecoverMana(float val);
    KeyValuePair<bool, BonusStatus> LevelUP();
    IEnumerator AttackAction(DamageType type, Action onComplete);
    IEnumerator DefendAction(Action onComplete);
    IEnumerator MagicAction(Action onComplete);
    IEnumerator CounterState(float dmg);
    void DisableUnit(Vector3 pos);
    void EnableUnit(int index);
    void ResetUnit();

    void BeginTurn();
}
