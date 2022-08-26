using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class LostWelderActionTrigger : ActionTrigger
{
    public Nerf _InitialNerf;
    public override void Initialize()
    {
        GetComponent<Unit>().mActionTrigger += StartActionTrigger;
    }
    public override void Eliminate()
    {
        GetComponent<Unit>().mActionTrigger -= StartActionTrigger;
    }

    protected override void StartActionTrigger()
    {
        StartCoroutine(Action());
    }

    protected override IEnumerator Action()
    {
        if(GetComponent<Unit>().mTarget)
            GetComponent<Unit>().mTarget.SetNerf(_InitialNerf.Initialize(GetComponent<Unit>(),GetComponent<Unit>().mTarget));
        yield return null;
    }
}
