using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Item")]
public abstract class Item : ScriptableObject
{
    public Transform transform;

    public string mName;
    public int mValue;
    public int mAmount;

    public Item(string name, int val, int amount)
    {
        mName = name;
        mValue = val;
        mAmount = amount;
    }

    public abstract void Apply();
    public abstract void End();
}