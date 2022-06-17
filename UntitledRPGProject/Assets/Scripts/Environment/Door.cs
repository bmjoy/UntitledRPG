using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableEnvironment
{
    [SerializeField]
    private Item mRequiredKey;

    public override IEnumerator Interact(Action action = null)
    {
        if(PlayerController.Instance.mInventory.Get(mRequiredKey.Name))
        {
            _Completed = true;
        }
        if(_Completed)
        {
            // TODO: Open/Close the gate
        }
        yield return null;
    }
    

    public override void Reset()
    {
        _Completed = false;
        // TODO: Close the gate
    }
}
