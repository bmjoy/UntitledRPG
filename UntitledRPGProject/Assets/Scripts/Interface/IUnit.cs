using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    void TakeDamage(float dmg, DamageType type);
    void TakeRecover(float val);
    IEnumerator AttackAction(IUnit opponent, DamageType type);
    void PlayAnimation(string name, bool active);
}
