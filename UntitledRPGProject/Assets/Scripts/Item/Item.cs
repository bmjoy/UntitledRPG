using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [HideInInspector]
    public int Value = 0;
    [HideInInspector]
    public string Name = string.Empty;
    [HideInInspector]
    public Unit mOwner;
    public int ID = -1;
    public ItemInfo Info;

    public virtual void Initialize(int id)
    {
        if (Info == null)
        {
            Debug.Log("Fail to initialize!");
            return;
        }
        Value = Info.mCost;
        Name = Info.mName;
        ID = id;
        mOwner = null;
    }

    public abstract void Apply();
    public abstract void End();
}
