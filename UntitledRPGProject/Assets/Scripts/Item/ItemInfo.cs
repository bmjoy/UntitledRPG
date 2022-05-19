using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Item")]
public abstract class ItemInfo : ScriptableObject
{
    public string mName;
    public int mCost;
    public Sprite mSprite;
}