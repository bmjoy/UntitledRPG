using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniHealthBar : HealthBar
{
    public void ActiveDeathAnimation(bool active)
    {
        mAnimator.SetBool("Death", active);
    }
    private void OnDestroy()
    {
        Destroy(mBorader?.gameObject);
    }
}
