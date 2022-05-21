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

    public ItemInfo Info;

    public virtual void Initialize()
    {
        if (Info == null)
        {
            Debug.Log("Fail to initialize! " + name);
            return;
        }
        Value = Info.mCost;
        if (Info.mName == null)
            Info.mName = Info.name;
        Name = Info.mName;
        Debug.Log(Name);
        mOwner = null;
    }

    public abstract void Apply();
    public abstract void End();
}
