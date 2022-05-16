using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlState
{
    virtual public ControlState Handle()
    {
        return this;
    }
}

public class IdleState : ControlState
{
    public override ControlState Handle()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow))
            return new RunState();
        return this;
    }
}

public class RunState : ControlState
{
    public override ControlState Handle()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow))
            return new RunState();
        return new IdleState();
    }
}

public class BattleState : ControlState
{
    public override ControlState Handle()
    {
        return this;
    }
}
