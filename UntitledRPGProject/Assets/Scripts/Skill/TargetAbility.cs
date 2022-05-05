using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/TargetAbility")]
public class TargetAbility : Skill_Setting
{
    private Unit mTarget;
    private GameObject mProjectile;
    public GameObject mProjectilePrefab;
    public bool IsShootType = false;
    public LayerMask mAllyMask;
    public LayerMask mTargetMask;

    public override void Activate(MonoBehaviour parent)
    {
        isActive = false;
        parent.StopAllCoroutines();
        mOwner = parent.transform.GetComponent<Unit>();
        parent.StartCoroutine(WaitforDecision());
    }

    public override IEnumerator WaitforDecision()
    {
        if(mOwner.mStatus.mMana < mManaCost)
            BattleManager.Instance.Cancel();
        else
        {
            if (mOwner.mAiBuild.type == AIType.Manual)
            {
                UIManager.Instance.ChangeText_Target("Choose the Target");
                UIManager.Instance.DisplayAskingSkill(true);
                mTarget = null;
                while (mTarget == null)
                {
                    Raycasting();
                    if (Input.GetMouseButtonDown(1))
                    {
                        mTarget = null;
                        isActive = false;
                        break;
                    }
                    yield return null;
                }
                UIManager.Instance.ChangeText_Target("OK? (Y/N)");
                while (true)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        isActive = true;
                        break;
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        isActive = false;
                        mTarget = null;
                        break;
                    }
                    yield return null;
                }
                UIManager.Instance.DisplayAskingSkill(false);
            }
            else
            {
                isActive = true;
                mTarget = mOwner.mTarget;
            }


            if (isActive)
            {
                mOwner.PlayAnimation("Attack");
                mOwner.mStatus.mMana -= mManaCost;
                if (IsShootType && isActive)
                {
                    Shoot();
                    yield return new WaitUntil(() => mProjectile.GetComponent<Projectile>().isCollide == true);
                }
                switch (mSkillType)
                {
                    case SkillType.Attack:
                        {
                            mTarget.TakeDamage(mValue, DamageType.Magical);
                        }
                        break;
                    case SkillType.AttackBuff:
                        {
                            mTarget.TakeDamage(mValue, DamageType.Magical);
                        }
                        break;
                    case SkillType.AttackNerf:
                        {
                            mTarget.TakeDamage(mValue, DamageType.Magical);
                        }
                        break;
                    case SkillType.Buff:
                        {
                            foreach (GameObject buff in mBuffs)
                            {
                                mTarget.SetBuff(buff.GetComponent<TimedBuff>());
                            }
                        }
                        break;
                    case SkillType.BuffNerf:
                        {
                            foreach (GameObject buff in mBuffs)
                            {
                                mTarget.SetBuff(buff.GetComponent<TimedBuff>());
                            }
                            foreach (GameObject nerf in mNerfs)
                            {
                                mTarget.SetNerf(nerf.GetComponent<TimedNerf>());
                            }
                        }
                        break;
                    case SkillType.Nerf:
                        {
                            foreach (GameObject nerf in mNerfs)
                            {
                                mTarget.SetNerf(nerf.GetComponent<TimedNerf>());
                            }
                        }
                        break;
                    case SkillType.Heal:
                        {
                            mTarget.TakeRecover(mValue);
                            break;
                        }
                    case SkillType.HealBuff:
                        {
                            mTarget.TakeRecover(mValue);
                            foreach (GameObject buff in mBuffs)
                            {
                                mTarget.SetBuff(buff.GetComponent<TimedBuff>());
                            }
                            break;
                        }

                    case SkillType.HealNerf:
                        {
                            mTarget.TakeRecover(mValue);
                            foreach (GameObject nerf in mNerfs)
                            {
                                mTarget?.SetNerf(nerf.GetComponent<TimedNerf>());
                            }
                            break;
                        }
                    case SkillType.Summon:
                        break;
                }
            }
            else
                BattleManager.Instance.Cancel();
        }
        isComplete = true;
        yield return null;
    }

    private void Shoot()
    {
        Vector3 dir = (mTarget.transform.position - mOwner.transform.position).normalized;
        mProjectile = Instantiate(mProjectilePrefab, mOwner.transform.position + dir * 1.5f, Quaternion.identity);
        mProjectile.transform.LookAt(dir);
        mProjectile.GetComponent<Projectile>().Initialize(mTarget, dir, DamageType.Magical, null);
    }

    private void Raycasting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (mProperty == SkillProperty.Friendly)
        {
            if (Physics.Raycast(ray, out hit, 100, mAllyMask))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if(hit.transform.GetComponent<Unit>().mConditions.isDied == false)
                    {
                        mTarget = hit.transform.GetComponent<Unit>();
                        Debug.Log(hit.transform.name);
                    }
                }
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 100, mTargetMask))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.transform.GetComponent<Unit>().mConditions.isDied == false)
                    {
                        mTarget = hit.transform.GetComponent<Unit>();
                        Debug.Log(hit.transform.name);
                    }
                }
            }
        }
        if (mTarget)
            isActive = true;
    }

    public override void Initialize(Unit owner)
    {
        mOwner = owner;
        if (mValue <= 0.0f)
            mValue = owner.mStatus.mMagicPower;
    }
}
