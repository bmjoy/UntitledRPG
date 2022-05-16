using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void Enter(Unit agent);
    public abstract void Execute(Unit agent);
    public abstract void Exit(Unit agent);
}

public abstract class P_State
{
    public abstract void Enter(Prowler agent);
    public abstract void Execute(Prowler agent);
    public abstract void Exit(Prowler agent);
}
