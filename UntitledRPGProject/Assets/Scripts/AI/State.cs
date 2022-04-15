using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected bool isAct = false;
    public abstract void Enter(Unit agent);
    public abstract void Execute(Unit agent);
    public abstract void Exit(Unit agent);
}

public abstract class P_State
{
    protected bool isAct = false;
    public abstract void Enter(EnemyProwler agent);
    public abstract void Execute(EnemyProwler agent);
    public abstract void Exit(EnemyProwler agent);
}
