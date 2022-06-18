using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/EnemyTrap")]
public class EnemyTrapInfo : ItemInfo
{
    private const int size = 4;
    public EnemyUnit[] mEnemyUnits = new EnemyUnit[size];
    public EnemyTrapInfo() : base()
    {
    }
    void OnValidate()
    {
        if (mEnemyUnits.Length != size)
        {
            Debug.LogWarning("Don't change the 'EnemyList' field's array size!");
            Array.Resize(ref mEnemyUnits, size);
        }
    }
}
