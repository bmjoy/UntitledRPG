using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    private Unit mUnit;
    private Animator mAnimator;
    private Mirror other;
    public void Initialize(Unit unit, Vector3 pos)
    {
        mUnit = unit;

        transform.SetParent(unit.transform);
        transform.position = unit.transform.position;
        transform.position += pos;
        gameObject.AddComponent<SpriteRenderer>().sprite = mUnit.GetComponent<SpriteRenderer>().sprite;
        gameObject.GetComponent<SpriteRenderer>().flipX = mUnit.GetComponent<SpriteRenderer>().flipX;

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.35f);
        if (unit.mFlag == Flag.Player)
            GetComponent<SpriteRenderer>().flipX = true;
        gameObject.AddComponent<Billboard>();
        mAnimator = gameObject.AddComponent<Animator>();
        mAnimator.runtimeAnimatorController = mUnit.mAnimator.runtimeAnimatorController;
        if (mUnit.mirror == null)
            mUnit.mirror = this;
    }

    public void Link(Mirror mirror)
    {
        other = mirror;
        mAnimator.speed = 1.05f;
        other.mAnimator.speed = 1.5f;
    }

    public void SetBool(string name, bool active)
    {
        mAnimator.SetBool(name, active);
        other?.SetBool(name, active);
    }   
    public void SetFloat(string name, float value)
    {
        mAnimator.SetFloat(name, value);
        other?.SetFloat(name, value);
    }
    public void SetTrigger(string name)
    {
        mAnimator.SetTrigger(name);
        other?.SetTrigger(name);
    }
    public void Play(string name)
    {
        mAnimator.Play(name);
        other?.Play(name);
    }

    private void OnDestroy()
    {
        mUnit.mirror = null;
        mUnit = null;
        other = null;
    }
}
