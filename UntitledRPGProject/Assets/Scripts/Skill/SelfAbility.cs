using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SelfAbility")]
public class SelfAbility : Skill_Setting
{
    [SerializeField]
    SkillTarget mSkillNerfTarget;
    [SerializeField]
    SkillTarget mSkillBuffTarget;
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
            BattleManager.Instance.Cancel();
        else
        {
            if(mOwner.mAiBuild.type == AIType.Manual)
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
            }
            else
                isActive = true;

            if (isActive)
            {
                float newValue = mValue + mOwner.mStatus.mMagicPower + mOwner.mBonusStatus.mMagicPower;
                bool hasState = mOwner.GetComponent<Animator>().HasState(0, Animator.StringToHash("Skill"));
                mOwner.PlayAnimation((hasState) ? "Skill" : "Attack");
                yield return new WaitForSeconds(0.3f);
                mOwner.mStatus.mMana -= mManaCost;
                switch (mSkillType)
                {
                    case SkillType.Attack:
                        {
                            mOwner.TakeDamage(newValue, DamageType.Magical);
                            DoBuff();
                            DoNerf();
                        }
                        break;
                    case SkillType.Buff:
                        {
                            DoBuff();
                        }
                        break;
                    case SkillType.BuffNerf:
                        {
                            DoBuff();
                            DoNerf();
                        }
                        break;
                    case SkillType.Nerf:
                        {
                            DoNerf();
                        }
                        break;
                    case SkillType.Heal:
                        {
                            mOwner.TakeRecover(newValue);
                            DoBuff();
                            DoNerf();
                            break;
                        }
                    case SkillType.Summon:
                        break;
                    default:
                        break;
                }

                if (Resources.Load<GameObject>("Prefabs/Effects/" + mName + "_Effect") != null)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + mName + "_Effect")
    , mOwner.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
                    Destroy(go, go.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                }
            }
            else
                BattleManager.Instance.Cancel();
        }
 
        isComplete = true;
        yield return null;
    }

    private void DoBuff()
    {
        if (mSkillBuffTarget == SkillTarget.All)
        {
            IEnumerable<GameObject> group = group = (mOwner.mFlag == Flag.Player) ? BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Player)
                    : BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Enemy);
            foreach (var unit in group)
            {
                var i = unit.GetComponent<Unit>();
                foreach (var buff in mBuffList)
                    i.SetBuff(buff.Initialize(mOwner, i));
            }
        }
        else if (mSkillBuffTarget == SkillTarget.Random)
        {
            IEnumerable<GameObject> group = group = (mOwner.mFlag == Flag.Player) ? BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Enemy)
                    : BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Player);
            foreach (var unit in group)
            {
                var i = unit.GetComponent<Unit>();
                if (UnityEngine.Random.Range(1, 2) == 2)
                    continue;
                foreach (var buff in mBuffList)
                    i.SetBuff(buff.Initialize(mOwner, i));
            }
        }
        else if (mSkillBuffTarget == SkillTarget.Self)
        {
            foreach (var buff in mBuffList)
                mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
        }
        else
        {
            return;
        }
    }

    private void DoNerf()
    {
        if (mSkillNerfTarget == SkillTarget.All)
        {
            IEnumerable<GameObject> group = group = (mOwner.mFlag == Flag.Player) ? BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Enemy)
                    : BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Player);
            foreach (var unit in group)
            {
                var i = unit.GetComponent<Unit>();
                foreach (var nerf in mNerfList)
                    i.SetNerf(nerf.Initialize(mOwner, i));
            }
        }
        else if (mSkillNerfTarget == SkillTarget.Random)
        {
            IEnumerable<GameObject> group = group = (mOwner.mFlag == Flag.Player) ? BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Enemy)
                    : BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Player);
            foreach (var unit in group)
            {
                var i = unit.GetComponent<Unit>();
                if (UnityEngine.Random.Range(1, 2) == 2)
                    continue;
                foreach (var nerf in mNerfList)
                    i.SetNerf(nerf.Initialize(mOwner, i));
            }
        }
        else if (mSkillNerfTarget == SkillTarget.Self)
        {
            foreach (var nerf in mNerfList)
                mOwner.SetNerf(nerf.Initialize(mOwner, mOwner));
        }
        else
        {
            return;
        }
    }

}
