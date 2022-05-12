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
                    case SkillType.Attack: break;
                    case SkillType.AttackBuff:
                        {
                            mOwner.TakeDamage(mValue, DamageType.Magical);
                            mOwner.SetBuff(mBuff.Initialize(mOwner));
                        }
                        break;
                    case SkillType.AttackNerf:
                        {
                            mOwner.TakeDamage(mValue, DamageType.Magical);
                            mOwner.SetNerf(mNerf.Initialize(mOwner));
                        }
                        break;
                    case SkillType.Buff:
                        {
                            mOwner.SetBuff(mBuff.Initialize(mOwner));
                        }
                        break;
                    case SkillType.BuffNerf:
                        {
                            mOwner.SetBuff(mBuff.Initialize(mOwner));
                            mOwner.SetNerf(mNerf.Initialize(mOwner));
                        }
                        break;
                    case SkillType.Nerf:
                        {
                            mOwner.SetNerf(mNerf.Initialize(mOwner));
                        }
                        break;
                    case SkillType.Heal:
                        {
                            mOwner.TakeRecover(mValue);
                            break;
                        }
                    case SkillType.HealBuff:
                        {
                            mOwner.TakeRecover(mValue);
                            mOwner.SetBuff(mBuff.Initialize(mOwner));
                            break;
                        }

                    case SkillType.HealNerf:
                        {
                            mOwner.TakeRecover(mValue);
                            mOwner.SetNerf(mNerf.Initialize(mOwner));
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
