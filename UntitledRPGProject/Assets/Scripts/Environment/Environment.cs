using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public int mID;
    public string mName;
    protected Animator mAnimator;

    public virtual void Initialize(int id)
    {
        mID = id;
        mAnimator = GetComponent<Animator>();
    }
}
