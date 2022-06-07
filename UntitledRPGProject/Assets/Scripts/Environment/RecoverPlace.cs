using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverPlace : InteractableEnvironment
{
    [SerializeField]
    private float _RecoverPercentage = 50.0f;
    private bool _Recovered = false;

    public override void Initialize(int id)
    {
        base.Initialize(id);
        Canvas_Initialize();
    }

    public override IEnumerator Interact(Action action = null)
    {
        if(!_Recovered)
        {
            foreach(GameObject go in PlayerController.Instance.mHeroes)
            {
                var status = go.GetComponent<Unit>().mStatus;
                go.GetComponent<Unit>().TakeRecover((_RecoverPercentage * status.mHealth) / status.mMaxHealth);
                go.GetComponent<Unit>().TakeRecoverMana((_RecoverPercentage * status.mMana) / status.mMaxMana);
            }
            _Recovered = true;
            action?.Invoke();
        }
        yield return null;
    }

    public override void Reset()
    {
        _Recovered = false;
    }
}
