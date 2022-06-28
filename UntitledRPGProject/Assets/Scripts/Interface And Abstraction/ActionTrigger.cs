using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionTrigger : MonoBehaviour
{
    protected Vector3 mPos;
    [HideInInspector]
    public float mTime;
    public bool isCompleted = false;
    protected virtual void StartActionTrigger() { }
    protected virtual IEnumerator Action() { yield return null; }
}
