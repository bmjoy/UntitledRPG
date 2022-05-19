using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(menuName = "Items/Expendables")]
public abstract class ExpendablesInfo : ItemInfo
{
    public int mAmount;
}
