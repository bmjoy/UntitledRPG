using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBActionTrigger : ActionTrigger
{
    public Buff _Counter;
    protected override IEnumerator Action()
    {
        yield return null;
    }

    protected override void StartActionTrigger()
    {

    }

    private void Start()
    {
        GetComponent<Unit>().SetBuff(_Counter.Initialize(GetComponent<Unit>(), GetComponent<Unit>()));
    }
    private void OnDestroy()
    {

    }

}
