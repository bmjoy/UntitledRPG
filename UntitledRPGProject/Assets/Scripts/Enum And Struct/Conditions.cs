using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Conditions
{
    public bool isDied;
    public bool isDefend;
    public bool isCancel;
    public bool isBattleComplete;

    public Conditions(bool die, bool de, bool can, bool bat)
    {
        isDied = die;
        isDefend = de;
        isCancel = can;
        isBattleComplete = bat;
    }

    public Conditions(bool close)
    {
        isDied =
            isDefend =
            isCancel =
            isBattleComplete = close;
    }
}