using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IUnit
{
    public Unit_Setting mSetting;
    public Rigidbody mRigidbody;
    public Animator mAnimator;
    public StateMachine mStateMachine;

    public EnemyProwler mEnemyProwler;

    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        GameObject[] agent = GameObject.FindGameObjectsWithTag("Enemy");
        if (agent.Length > 1)
        {
            for (int i = 0; i < agent.Length; i++)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
            }
        }
        mStateMachine = gameObject.AddComponent<StateMachine>();
        mStateMachine.mAgent = this.gameObject;
        //TODO Add behaviors
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: AI StateMachine
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
        if (type == DamageType.Physical)
        {
            float value = dmg - (dmg * mSetting.Defend * 100.0f);
            mSetting.Health = mSetting.Health - (value - mSetting.Armor);
            if (mSetting.Health <= 0)
            {
                mSetting.Death = true;
                mEnemyProwler.EnemySpawnGroup.Remove(this);
                Destroy(this.gameObject, 4.0f);
            }
        }
        else
        {
            mSetting.Health = mSetting.Health - (dmg - mSetting.Magic_Resistance);
            if (mSetting.Health <= 0)
            {
                mSetting.Death = true;
                mEnemyProwler.EnemySpawnGroup.Remove(this);
                Destroy(this.gameObject,4.0f);
            }
        }

    }

    public void TakeRecover(float val)
    {
        mSetting.Health += val;
        if (mSetting.Health >= mSetting.MaxHealth)
            mSetting.Health = mSetting.MaxHealth;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (mRigidbody == null)
            return;
        mRigidbody.velocity = Vector3.zero;
        mRigidbody.angularVelocity = Vector3.zero;
    }
}
