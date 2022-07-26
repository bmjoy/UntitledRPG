using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SelfAbility")]
public class SelfAbility : DamagableAbility
{
    [SerializeField]
    public SkillTarget mSkillNerfTarget;
    [SerializeField]
    public SkillTarget mSkillBuffTarget;

    private GameObject effect;

    public override void Activate(MonoBehaviour parent)
    {
        isActive = false;
        effect = ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + mName + "_Effect");
        parent.StopAllCoroutines();
        parent.StartCoroutine(WaitforDecision());
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
            if(mOwner.mAiBuild.type == AIBuild.AIType.Manual)
            {
                UIManager.Instance.ChangeOrderBarText(UIManager.Instance.mStorage.mTextForAccpet);
                IEnumerable<GameObject> AllyALL = (mOwner.mFlag == Flag.Enemy) ? BattleManager.Instance.mEnemies : PlayerController.Instance.mHeroes;
                IEnumerable<GameObject> TargetALL = (mOwner.mFlag == Flag.Player) ? BattleManager.Instance.mEnemies : PlayerController.Instance.mHeroes;
                while (true)
                {
                    if (mSkillType == SkillType.Attack)
                    {
                        foreach (GameObject g in TargetALL)
                        {
                            Unit u = g.GetComponent<Unit>();
                            if (u.mConditions.isDied)
                                continue;
                            u.mField.TargetedMagicHostile(true);
                        }
                    }

                    if (mSkillNerfTarget == SkillTarget.All)
                    {                        
                        foreach(GameObject g in TargetALL)
                        {
                            Unit u = g.GetComponent<Unit>();
                            if (u.mConditions.isDied)
                                continue;
                            u.mField.TargetedMagicHostile(true);
                        }
                    }

                    if(mSkillBuffTarget == SkillTarget.All)
                    {
                        foreach (GameObject g in AllyALL)
                        {
                            Unit u = g.GetComponent<Unit>();
                            if (u.mConditions.isDied)
                                continue;
                            u.mField.TargetedFriendly(true);
                        }
                    }

                    if (Input.GetKeyDown(UIManager.Instance.mYesKeyCode))
                    {
                        isActive = true;
                        foreach (GameObject g in TargetALL)
                        {
                            Unit u = g.GetComponent<Unit>();
                            if (u.mConditions.isDied)
                                continue;
                            u.mField.TargetedMagicHostile(false);
                        }
                        foreach (GameObject g in AllyALL)
                        {
                            Unit u = g.GetComponent<Unit>();
                            if (u.mConditions.isDied)
                                continue;
                            u.mField.TargetedFriendly(false);
                        }
                        break;
                    }
                    if (Input.GetKeyDown(UIManager.Instance.mNoKeyCode))
                    {
                        isActive = false;
                        foreach (GameObject g in TargetALL)
                        {
                            Unit u = g.GetComponent<Unit>();
                            if (u.mConditions.isDied)
                                continue;
                            u.mField.TargetedMagicHostile(false);
                        }
                        foreach (GameObject g in AllyALL)
                        {
                            Unit u = g.GetComponent<Unit>();
                            if (u.mConditions.isDied)
                                continue;
                            u.mField.TargetedFriendly(false);
                        }
                        UIManager.Instance.ChangeOrderBarText("Waiting for Order...");
                        break;
                    }
                    yield return null;
                }
            }
            else
                isActive = true;

            if (isActive)
            {
                UIManager.Instance.DisplaySupportKey(false);
                UIManager.Instance.ChangeOrderBarText("<color=red>" + mName + "!</color>");
                
                bool hasState = mOwner.GetComponent<Animator>().HasState(0, Animator.StringToHash(mAnimationName));
                mOwner.mStatus.mMana -= mManaCost;
                mOwner.mirror?.Play((hasState) ? mAnimationName : "Attack");
                mOwner.mAnimator.Play((hasState) ? mAnimationName : "Attack");
                if (mProperty == SkillProperty.Friendly)
                    CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.ZoomCamera(mEffectTime / 2.0f,mOwner.transform.position));

                if (mActionTrigger != null)
                {
                    mActionTrigger?.Invoke();
                    yield return new WaitUntil(()=> mOwner.GetComponent<ActionTrigger>().isCompleted);
                    if (mOwner.mSkillClips.Count() > 0)
                        AudioManager.PlaySfx(mOwner.mSkillClips.ElementAt(UnityEngine.Random.Range(0, mOwner.mSkillClips.Count())).Clip);
                    DoBuff();
                    DoNerf();
                }
                else
                {
                    if (mOwner.mSkillClips.Count() > 0)
                        AudioManager.PlaySfx(mOwner.mSkillClips.ElementAt(UnityEngine.Random.Range(0, mOwner.mSkillClips.Count())).Clip);
                    yield return new WaitForSeconds(mEffectTime);
                    CommonState();
                }
                if (effect != null)
                {
                    GameObject go = Instantiate(effect
    , mOwner.transform.position + effect.transform.position, Quaternion.Euler((effect.transform.eulerAngles)));
                    Destroy(go, 3.0f);
                }
                yield return new WaitForSeconds(1.0f);
            }
            else
                BattleManager.Instance.Cancel();
        }
 
        isComplete = true;
        yield return null;
    }

    private void CommonState()
    {
        float newValue_M = mValue + mOwner.mStatus.mMagicPower + mOwner.mBonusStatus.mMagicPower;
        float newValue_P = mValue + mOwner.mStatus.mDamage + mOwner.mBonusStatus.mDamage;
        bool isHit = true;
        switch (mSkillType)
        {
            case SkillType.Attack:
                {
                    IEnumerable<GameObject> group = group = (mOwner.mFlag == Flag.Player) ? BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Enemy)
: BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Player);

                    foreach (GameObject unit in group)
                    {
                        var i = unit.GetComponent<Unit>();
                        isHit = i.TakeDamage((mDamageType == DamageType.Physical) ? (newValue_P) : (newValue_M), mDamageType);
                        if(isHit)
                        {
                            foreach (var nerf in mNerfList)
                                i.SetNerf(nerf.Initialize(mOwner, i));
                            if (effect != null)
                            {
                                GameObject go = Instantiate(effect
                , i.transform.position + effect.transform.position, Quaternion.identity);
                                Destroy(go, 3.0f);
                            }
                        }
                    }
                    DoBuff();
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
                    mOwner.TakeRecover(newValue_M);
                    DoBuff();
                    DoNerf();
                    if (effect != null)
                    {
                        GameObject go = Instantiate(effect
        , mOwner.transform.position + effect.transform.position, Quaternion.identity);
                        Destroy(go, 3.0f);
                    }
                    break;
                }
            default:
                break;
        }
    }
    private void DoBuff()
    {
        if (mSkillBuffTarget == SkillTarget.All)
        {
            IEnumerable<GameObject> group = (mOwner.mFlag == Flag.Enemy) ? BattleManager.Instance.mEnemies : PlayerController.Instance.mHeroes;
            foreach (var unit in group)
            {
                var i = unit.GetComponent<Unit>();
                if (i.mConditions.isDied)
                    continue;
                foreach (var buff in mBuffList)
                    i.SetBuff(buff.Initialize(mOwner, i));
            }
        }
        else if (mSkillBuffTarget == SkillTarget.Random)
        {
            IEnumerable<GameObject> group = (mOwner.mFlag == Flag.Enemy) ? BattleManager.Instance.mEnemies : PlayerController.Instance.mHeroes;
            foreach (var unit in group)
            {
                var i = unit.GetComponent<Unit>();
                if (i.mConditions.isDied)
                    continue;
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
            IEnumerable<GameObject> group = (mOwner.mFlag == Flag.Player) ? BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Enemy)
                    : BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Player);
            foreach (var unit in group)
            {
                var i = unit.GetComponent<Unit>();
                if (i.mConditions.isDied)
                    continue;
                foreach (var nerf in mNerfList)
                    i.SetNerf(nerf.Initialize(mOwner, i));
            }
        }
        else if (mSkillNerfTarget == SkillTarget.Random)
        {
            IEnumerable<GameObject> group = (mOwner.mFlag == Flag.Player) ? BattleManager.Instance.mEnemies : PlayerController.Instance.mHeroes;
            foreach (var unit in group)
            {
                var i = unit.GetComponent<Unit>();
                if (i.mConditions.isDied)
                    continue;
                if (UnityEngine.Random.Range(0, 2) == 1)
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
