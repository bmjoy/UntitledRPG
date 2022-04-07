using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected bool isAct = false;
    public abstract void Enter(Unit agent);
    public abstract void Execute(Unit agent);
    public abstract void Exit(Unit agent);
}
