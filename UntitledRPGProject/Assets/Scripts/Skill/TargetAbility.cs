using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/TargetAbility")]
public class TargetAbility : DamagableAbility
{
    private Unit mTarget;
    private GameObject mProjectile;
    public SKillShootType mShootType = SKillShootType.Melee;
    [SerializeField]
    private float mRange = 4.0f;
    [SerializeField]
    private Vector2 mStartPosition;

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
            Field unit = null;
            if (mOwner.mAiBuild.type == AIBuild.AIType.Manual)
            {
                UIManager.Instance.ChangeOrderBarText(UIManager.Instance.mStorage.mTextForTarget);
                DisplayArrow(true);
                mTarget = null;
                unit = RandomTargeting(ref unit);
                if(mProperty == SkillProperty.Friendly)
                    BattleManager.Instance.mSpellChanning = true;
                while (true)
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        mTarget = null;
                        unit.TargetedMagicHostile(false);
                        unit.TargetedFriendly(false);
                        unit = (mProperty == SkillProperty.Hostile) ? BattleManager.enemyFieldParent.GetChild(0).GetComponent<Field>()
                            : BattleManager.playerFieldParent.GetChild(1).GetComponent<Field>();
                        if (unit.IsExist)
                        {
                            mTarget = unit.mUnit;
                            unit.TargetedMagicHostile(mProperty == SkillProperty.Hostile);
                            unit.TargetedFriendly(mProperty == SkillProperty.Friendly);
                        }
                        else
                            unit = RandomTargeting(ref unit);
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        unit.TargetedMagicHostile(false);
                        unit.TargetedFriendly(false);
                        mTarget = null;
                        unit = (mProperty == SkillProperty.Hostile) ? BattleManager.enemyFieldParent.GetChild(1).GetComponent<Field>()
    : BattleManager.playerFieldParent.GetChild(0).GetComponent<Field>();
                        if (unit.IsExist)
                        {
                            mTarget = unit.mUnit;
                            unit.TargetedMagicHostile(mProperty == SkillProperty.Hostile);
                            unit.TargetedFriendly(mProperty == SkillProperty.Friendly);
                        }
                        else
                            unit = RandomTargeting(ref unit);
                    }
                    else if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        unit.TargetedMagicHostile(false);
                        unit.TargetedFriendly(false);
                        mTarget = null;
                        unit = (mProperty == SkillProperty.Hostile) ? BattleManager.enemyFieldParent.GetChild(2).GetComponent<Field>()
    : BattleManager.playerFieldParent.GetChild(2).GetComponent<Field>();
                        if (unit.IsExist)
                        {
                            mTarget = unit.mUnit;
                            unit.TargetedMagicHostile(mProperty == SkillProperty.Hostile);
                            unit.TargetedFriendly(mProperty == SkillProperty.Friendly);
                        }
                        else
                            unit = RandomTargeting(ref unit);
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        unit.TargetedMagicHostile(false);
                        unit.TargetedFriendly(false);
                        mTarget = null;
                        unit = (mProperty == SkillProperty.Hostile) ? BattleManager.enemyFieldParent.GetChild(3).GetComponent<Field>()
     : BattleManager.playerFieldParent.GetChild(3).GetComponent<Field>();
                        if (unit.IsExist)
                        {
                            mTarget = unit.mUnit;
                            unit.TargetedMagicHostile(mProperty == SkillProperty.Hostile);
                            unit.TargetedFriendly(mProperty == SkillProperty.Friendly);
                        }
                        else
                            unit = RandomTargeting(ref unit);
                    }
                    else { }

                    if (Input.GetKeyDown(UIManager.Instance.mYesKeyCode) && mTarget)
                    {
                        isActive = true;
                        break;
                    }

                    if(Input.GetKeyDown(UIManager.Instance.mNoKeyCode))
                    {
                        unit.TargetedMagicHostile(false);
                        unit.TargetedFriendly(false);
                        isActive = false;
                        unit = null;
                        mTarget = null;
                        BattleManager.Instance.mSpellChanning = false;
                        UIManager.Instance.ChangeOrderBarText("Waiting for Order...");
                        break;
                    }

                    yield return null;
                }

                DisplayArrow(false);

                UIManager.Instance.ChangeOrderBarText(UIManager.Instance.mStorage.mTextForAccpet);
            }
            else
            {
                isActive = true;
                mTarget = mOwner.mTarget;
                unit = mTarget.mField;
            }

            if (isActive)
            {
                UIManager.Instance.DisplaySupportKey(false);
                BattleManager.Instance.mSpellChanning = false;
                unit.TargetedMagicHostile(false);
                unit.TargetedFriendly(false);
                if(mTarget.GetType() == typeof(Player) && mProperty == SkillProperty.Friendly)
                {
                    Player playerunit = (Player)mTarget;
                    playerunit.mMyHealthBar.isTargetted = false;
                }

                UIManager.Instance.ChangeOrderBarText("<color=red>"+ mName + "!</color>");
                mOwner.mTarget = mTarget;
                mTarget?.mSelected.SetActive(false);
                bool hasState = mOwner.mAnimator.HasState(0, Animator.StringToHash(mAnimationName));
                mOwner.mMagicDistance = mRange;
                mOwner.mAiBuild.SetActionEvent(AIBuild.ActionEvent.MagicWalk);
                if(mProperty == SkillProperty.Friendly)
                    CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.ZoomCamera(mEffectTime / 2.0f, Vector3.Lerp(mOwner.transform.position, mTarget.transform.position, 0.5f)));
                yield return new WaitUntil(() => mOwner.mAiBuild.actionEvent == AIBuild.ActionEvent.Busy);
                mOwner.mStatus.mMana -= mManaCost;

                if (mShootType == SKillShootType.Range)
                {
                    mOwner.mirror?.Play((hasState) ? mAnimationName : "Attack");
                    mOwner.mAnimator.Play((hasState) ? mAnimationName : "Attack");
                    yield return new WaitForSeconds(mEffectTime);
                    if(mOwner.mSkillClips.Count() > 0)
                        AudioManager.PlaySfx(mOwner.mSkillClips.ElementAt(UnityEngine.Random.Range(0, mOwner.mSkillClips.Count())).Clip, 1.0f);
                    Shoot();
                    yield return new WaitUntil(() => mProjectile.GetComponent<Projectile>().isCollide == true);
                }
                else if (mShootType == SKillShootType.Melee)
                {
                    yield return new WaitForSeconds(mEffectTime);
                    mOwner.mirror?.Play((hasState) ? mAnimationName : "Attack");
                    if(mOwner.GetComponent<ActionTrigger>())
                    {
                        mActionTrigger?.Invoke();
                        yield return new WaitUntil(() => mOwner.GetComponent<ActionTrigger>().isCompleted);
                    }
                    else
                    {
                        mOwner.mAnimator.Play((hasState) ? mAnimationName : "Attack");
                        yield return new WaitForSeconds(mOwner.mAnimator.GetCurrentAnimatorStateInfo(0).length + mEffectTime);
                        if (mOwner.mSkillClips.Count() > 0)
                            AudioManager.PlaySfx(mOwner.mSkillClips.ElementAt(UnityEngine.Random.Range(0, mOwner.mSkillClips.Count())).Clip, 1.0f);
                        CommonState();
                    }
                    yield return new WaitForSeconds(0.2f);
                }
                else if(mShootType == SKillShootType.Instant)
                {
                    mOwner.mirror?.Play((hasState) ? mAnimationName : "Attack");
                    mOwner.mAnimator.Play((hasState) ? mAnimationName : "Attack");
                    if (mOwner.mSkillClips.Count() > 0)
                        AudioManager.PlaySfx(mOwner.mSkillClips.ElementAt(UnityEngine.Random.Range(0, mOwner.mSkillClips.Count())).Clip, 1.0f);
                    yield return new WaitForSeconds(mEffectTime);
                    CommonState();
                }

                GameObject effect = ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + mName + "_Effect");
                if (effect != null)
                {
                    GameObject go = Instantiate(effect
    , mTarget.transform.position + effect.transform.position,Quaternion.Euler(effect.transform.eulerAngles));
                    Destroy(go, 1.1f);
                }
                yield return new WaitForSeconds(0.5f);
                mOwner.mAiBuild.SetActionEvent(AIBuild.ActionEvent.BackWalk);
            }
            else
                BattleManager.Instance.Cancel();
        }
        if(mOwner.mAiBuild.actionEvent == AIBuild.ActionEvent.BackWalk)
            yield return new WaitUntil(() => mOwner.mAiBuild.actionEvent == AIBuild.ActionEvent.Busy);
        isComplete = true;
        yield return null;
    }
    private Field RandomTargeting(ref Field unit)
    {
        if(mProperty == SkillProperty.Friendly)
        {
            for (int i = 0; i < BattleManager.playerFieldParent.childCount; ++i)
            {
                unit = BattleManager.playerFieldParent.GetChild(i).GetComponent<Field>();
                if (unit.IsExist)
                {
                    mTarget = unit.mUnit;
                    unit.TargetedFriendly(true);
                    return unit;
                }
            }
        }
        else
        {
            for (int i = 0; i < BattleManager.enemyFieldParent.childCount; ++i)
            {
                unit = BattleManager.enemyFieldParent.GetChild(i).GetComponent<Field>();
                if (unit.IsExist)
                {
                    mTarget = unit.mUnit;
                    unit.TargetedMagicHostile(true);
                    return unit;
                }
            }
        }
        return unit;
    }
    private void DisplayArrow(bool display)
    {
        int count = 0;
        if (mOwner.mFlag == Flag.Player)
        {
            count = (mProperty == SkillProperty.Friendly) ? PlayerController.Instance.mHeroes.Count : BattleManager.Instance.mEnemies.Count;
        }
        else if(mOwner.mFlag == Flag.Enemy)
        {
            count = (mProperty == SkillProperty.Hostile) ? PlayerController.Instance.mHeroes.Count : BattleManager.Instance.mEnemies.Count;
        }
        else
        {
            count = 0;
        }
        for (int i = 0; i < count; ++i)
        {
            if (mOwner.mFlag == Flag.Player)
            {
                if(mProperty == SkillProperty.Hostile)
                {
                    Unit e = BattleManager.Instance.mEnemies[i].GetComponent<Unit>();
                    if (!e.mConditions.isDied)
                        e.mArrow.SetActive(display);
                }
                else
                {
                    Unit e = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
                    if (!e.mConditions.isDied)
                        e.mArrow.SetActive(display);
                }
            }
            else if (mOwner.mFlag == Flag.Enemy)
            {
                if (mProperty == SkillProperty.Friendly)
                {
                    Unit e = BattleManager.Instance.mEnemies[i].GetComponent<Unit>();
                    if (!e.mConditions.isDied)
                        e.mArrow.SetActive(display);
                }
                else
                {
                    Unit e = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
                    if (!e.mConditions.isDied)
                        e.mArrow.SetActive(display);
                }
            }
        }
    }

    private void CommonState()
    {
        float newValue = mValue + mOwner.mStatus.mMagicPower + mOwner.mBonusStatus.mMagicPower;
        bool isHit = true;
        switch (mSkillType)
        {
            case SkillType.Attack:
                {
                    isHit = mTarget.TakeDamage(newValue, DamageType.Magical);
                    foreach (var buff in mBuffList)
                        mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
                    if (isHit)
                    {
                        foreach (var nerf in mNerfList)
                            mTarget.SetNerf(nerf.Initialize(mOwner, mTarget));
                    }

                }
                break;
            case SkillType.Buff:
                foreach (var buff in mBuffList)
                    mOwner.SetBuff(buff.Initialize(mOwner, mTarget));
                break;
            case SkillType.BuffNerf:
                {
                    foreach (var buff in mBuffList)
                        mTarget.SetBuff(buff.Initialize(mOwner, mTarget));
                    foreach (var nerf in mNerfList)
                        mTarget.SetNerf(nerf.Initialize(mOwner, mTarget));
                }
                break;
            case SkillType.Nerf:
                foreach (var nerf in mNerfList)
                {
                    mOwner.SetNerf(nerf.Initialize(mOwner, mTarget));
                }
                break;
            case SkillType.Heal:
                {
                    mTarget.TakeRecover(newValue);
                    foreach (var buff in mBuffList)
                        mOwner.SetBuff(buff.Initialize(mOwner, mTarget));
                    foreach (var nerf in mNerfList)
                        mOwner.SetNerf(nerf.Initialize(mOwner, mTarget));
                }
                    break;
        }
    }

    private void Shoot()
    {
        float newValue = mValue + mOwner.mStatus.mMagicPower + mOwner.mBonusStatus.mMagicPower;
        Vector3 dir = (mTarget.transform.position - mOwner.transform.position).normalized;
        mProjectile = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Bullets/" + mName), mOwner.transform.position + dir * mStartPosition.x, Quaternion.identity);
        mProjectile.GetComponent<Projectile>().mDamage = newValue;
        mProjectile.transform.LookAt(dir);

        mProjectile.GetComponent<Projectile>().Initialize(mTarget,
    () => {
        bool isHit = true;
        isHit = mTarget.TakeDamage(newValue, DamageType.Magical);
        foreach (var buff in mBuffList)
            mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
        if (isHit)
        {
            foreach (var nerf in mNerfList)
                mOwner.SetNerf(nerf.Initialize(mOwner, mTarget));
        }
    });

    }
    float maxDist = 0.0f;
    private void Raycasting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (mProperty == SkillProperty.Friendly)
        {                   
            BattleManager.Instance.mSpellChanning = true;

            if (Physics.Raycast(ray, out hit, 100, (mOwner.GetComponent<Unit>().mFlag == Flag.Player) ? LayerMask.GetMask("Ally") 
                : LayerMask.GetMask("Enemy")))
            {
                if (maxDist < hit.distance)
                {
                    mTarget?.mField.TargetedFriendly(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    maxDist = hit.distance;
                }

                if (mTarget.gameObject != hit.collider.gameObject)
                {
                    mTarget?.mField.TargetedFriendly(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    maxDist = hit.distance;
                }

            }
            else if(BattleManager.Instance.mCurrentUnit.mTarget)
            {
                if(mTarget != BattleManager.Instance.mCurrentUnit.mTarget)
                {
                    mTarget?.mField.TargetedFriendly(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (BattleManager.Instance.mCurrentUnit.mTarget.mConditions.isDied == false) ? BattleManager.Instance.mCurrentUnit.mTarget : null;
                    mTarget?.mSelected.SetActive(true);
                }
            }
            else
            {
                maxDist = 0.0f;
                mTarget?.mField.TargetedFriendly(false);
                mTarget?.mSelected.SetActive(false);
                mTarget = null;
            }
            mTarget?.mField.TargetedFriendly(true);
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 100, (mOwner.GetComponent<Unit>().mFlag == Flag.Player) ? LayerMask.GetMask("Enemy")
                : LayerMask.GetMask("Ally")))
            {
                if (maxDist < hit.distance)
                {
                    mTarget?.mField.TargetedMagicHostile(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    maxDist = hit.distance;
                }

                if (mTarget && mTarget.gameObject != hit.collider.gameObject)
                {
                    mTarget?.mField.TargetedMagicHostile(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    maxDist = hit.distance;
                }

                mTarget?.mField.TargetedMagicHostile(true);
            }
            else
            {
                maxDist = 0.0f;
                mTarget?.mField.TargetedMagicHostile(false);
                mTarget?.mSelected.SetActive(false);
                mTarget = null;
            }
        }

    }

    public override void Initialize(Unit owner)
    {
        mOwner = owner;
        if (mValue <= 0.0f)
            mValue = owner.mStatus.mMagicPower;
    }
}
