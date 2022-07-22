using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterExist
{
    public NPCUnit mUnit = NPCUnit.None;
    public bool isExist = false;
    public CharacterExist(NPCUnit mUnit, bool isExist)
    {
        this.mUnit = mUnit;
        this.isExist = isExist;
    }
}