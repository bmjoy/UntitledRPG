using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniHealthBar : HealthBar
{
    public override void Initialize(float currHP, float maxHP, float currMP, float maxMP)
    {
        base.Initialize(currHP, maxHP, currMP, maxMP);
    }
    public void ActiveDeathAnimation(bool active)
    {
        mAnimator.SetBool("Death", active);
    }
    private void OnDestroy()
    {
        Destroy(mBorader?.gameObject);
    }
}
