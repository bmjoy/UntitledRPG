using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IUnit
{
    private Animator mAnimator;
    public Unit_Setting mSetting;
    private bool onBattle = false;
    [SerializeField]
    private LayerMask mTargetMask;

    void Start()
    {
        mSetting.Health = mSetting.MaxHealth;
        mSetting.Death = false;
        mAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, mTargetMask))
        {
            mSetting.mTarget = hit.transform.gameObject;
        }
    }

    public IEnumerator AttackAction(IUnit opponent, DamageType type)
    {
        yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(1.0f);
        opponent.TakeDamage(mSetting.Attack, type);
    }

    public void PlayAnimation(string name, bool active)
    {
        mAnimator.SetBool(name, active);
    }

    public void TakeDamage(float dmg, DamageType type)
    {
        if (mSetting.Death)
            return;
        if(type == DamageType.Physical)
        {
            float value = dmg - (dmg * mSetting.Defend * 100.0f);
            mSetting.Health = mSetting.Health - (value - mSetting.Armor);
            if (mSetting.Health <= 0)
                mSetting.Death = true;
        }
        else
        {
            mSetting.Health = mSetting.Health - (dmg - mSetting.Magic_Resistance);
            if (mSetting.Health <= 0)
                mSetting.Death = true;
        }

    }
    public void TakeRecover(float val)
    {
        mSetting.Health += val;
        if (mSetting.Health >= mSetting.MaxHealth)
            mSetting.Health = mSetting.MaxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onBattle)
            return;
        if (other.GetComponent<EnemyProwler>() != null)
        {
            onBattle = true;
            GameManager.Instance.OnBattleStart(other.GetComponent<EnemyProwler>().id);
            //TODO call game manager to start the battle
        }
    }
}
