using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverPlace : InteractableEnvironment
{
    [SerializeField]
    private float _RecoverPercentage = 50.0f;

    public override void Initialize(int id)
    {
        base.Initialize(id);
        Canvas_Initialize();
    }

    public override IEnumerator Interact(Action action = null)
    {
        if(!_Completed)
        {
            foreach(GameObject go in PlayerController.Instance.mHeroes)
            {
                var status = go.GetComponent<Unit>().mStatus;
                var bonusstatus = go.GetComponent<Unit>().mBonusStatus;
                go.GetComponent<Unit>().TakeRecover((100 * _RecoverPercentage) / status.mMaxHealth + bonusstatus.mHealth);
                go.GetComponent<Unit>().TakeRecoverMana((100 * _RecoverPercentage) / status.mMaxMana + bonusstatus.mMana);
            }
            _Completed = true;
            mInteraction.SetActive(false);
            action?.Invoke();
        }
        yield return null;
    }

    public override void Reset()
    {
        _Completed = false;
    }
}
