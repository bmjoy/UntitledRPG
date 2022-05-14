using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SelfAbility")]
public class SelfAbility : Skill_Setting
{
    public GameObject mEffect;
    public override void Activate(MonoBehaviour parent)
    {
        isActive = false;
        parent.StopAllCoroutines();
        parent.StartCoroutine(WaitforDecision());
    }

    public override IEnumerator Effect()
    {
        throw new NotImplementedException();
    }

    public override void Initialize(Unit owner)
    {
        mOwner = owner;
        if(mValue <= 0.0f)
            mValue = owner.mStatus.mMagicPower;
    }

    public override IEnumerator WaitforDecision()
    {
        if(mOwner.mStatus.mMana < mManaCost)
        {
            BattleManager.Instance.Cancel();
        }
        else
        {
            UIManager.Instance.DisplayAskingSkill(true);
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
                    break;
                }
                yield return null;
            }
            UIManager.Instance.DisplayAskingSkill(false);
            if (isActive)
            {
                mOwner.PlayAnimation("Attack");
                mOwner.mStatus.mMana -= mManaCost;
                switch (mSkillType)
                {
                    case SkillType.Attack:
                        {
                            mOwner.TakeDamage(mValue, DamageType.Magical);
                            foreach(var buff in mBuffList)
                            {
                                mOwner.SetBuff(buff.Initialize(mOwner,mOwner));
                            }
                            foreach(var nerf in mNerfList)
                            {
                                mOwner.SetNerf(nerf.Initialize(mOwner, mOwner));
                            }

                        }
                        break;
                    case SkillType.Buff:
                        {
                            foreach (var buff in mBuffList)
                            {
                                mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
                            }
                        }
                        break;
                    case SkillType.BuffNerf:
                        {
                            foreach (var buff in mBuffList)
                            {
                                mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
                            }
                            foreach (var nerf in mNerfList)
                            {
                                mOwner.SetNerf(nerf.Initialize(mOwner, mOwner));
                            }
                        }
                        break;
                    case SkillType.Nerf:
                        {
                            foreach (var nerf in mNerfList)
                            {
                                mOwner.SetNerf(nerf.Initialize(mOwner, mOwner));
                            }
                        }
                        break;
                    case SkillType.Heal:
                        {
                            mOwner.TakeRecover(mValue);
                            foreach (var buff in mBuffList)
                            {
                                mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
                            }
                            foreach (var nerf in mNerfList)
                            {
                                mOwner.SetNerf(nerf.Initialize(mOwner, mOwner));
                            }
                            break;
                        }
                    case SkillType.Summon:
                        break;
                    default:
                        break;
                }
            }
            else
                BattleManager.Instance.Cancel();
        }
 
        isComplete = true;
        yield return null;
    }
}
