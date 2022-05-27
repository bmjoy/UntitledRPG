using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionTrigger : MonoBehaviour
{
    public Vector3 mPos;
    public float mTime;
    protected abstract void StartActionTrigger();
    protected abstract IEnumerator Action();
}
