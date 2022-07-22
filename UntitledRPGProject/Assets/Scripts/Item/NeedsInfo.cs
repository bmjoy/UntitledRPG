using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NeedsInfo
{
    public string Name = string.Empty;
    public int Value = 0;
    public int Amount = 0;
    [HideInInspector]
    public bool onComplete = false;
    public NeedsInfo(string n, int v, int a, bool complete = false)
    {
        Name = n;
        Value = v;
        Amount = a;
    }
}