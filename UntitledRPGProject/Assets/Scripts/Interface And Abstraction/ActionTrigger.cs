using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionTrigger : MonoBehaviour
{
    protected Vector3 mPos;
    [HideInInspector]
    public float mTime;
    public bool _isUltimate;
    protected abstract void StartActionTrigger();
    protected abstract IEnumerator Action();
}
