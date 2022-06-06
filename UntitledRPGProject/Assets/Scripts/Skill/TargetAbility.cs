using System;
using System.Collections;
using System.Collections.Generic;
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
            if (mOwner.mAiBuild.type == AIType.Manual)
            {
                UIManager.Instance.ChangeText_Skill(UIManager.Instance.mTextForTarget);
                UIManager.Instance.DisplayAskingSkill(true);
                mTarget = null;

                foreach (GameObject unit in (mProperty == SkillProperty.Friendly) ? PlayerController.Instance.mHeroes : BattleManager.Instance.mEnemies)
                {
                    if (!unit.GetComponent<Unit>().mConditions.isDied)
                        unit.GetComponent<Unit>().mCanvas.transform.Find("Arrow").gameObject.SetActive(true);
                }
                while (true)
                {
                    Raycasting();
                    if(Input.GetMouseButtonDown(0) && mTarget)
                    {
                        break;
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        mTarget = null;
                        isActive = false;
                        break;
                    }
                    yield return null;
                }

                foreach (GameObject unit in (mProperty == SkillProperty.Friendly) ? PlayerController.Instance.mHeroes : BattleManager.Instance.mEnemies)
                {
                    unit.GetComponent<Unit>().mCanvas.transform.Find("Arrow").gameObject.SetActive(false);
                }

                UIManager.Instance.ChangeText_Skill(UIManager.Instance.mTextForAccpet);
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
                mOwner.mTarget = mTarget;
                
                bool hasState = mOwner.GetComponent<Animator>().HasState(0, Animator.StringToHash(mAnimationName));
                mOwner.mMagicDistance = mRange;
                mOwner.mAiBuild.actionEvent = ActionEvent.MagicWalk;
                if(mProperty == SkillProperty.Friendly)
                    CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.ZoomCamera(mEffectTime / 2.0f, Vector3.Lerp(mOwner.transform.position, mTarget.transform.position, 0.5f)));
                mOwner.StartCoroutine(Effect());
                yield return new WaitUntil(() => mOwner.mAiBuild.actionEvent == ActionEvent.Busy);
                mOwner.mStatus.mMana -= mManaCost;

                if (mShootType == SKillShootType.Range)
                {
                    mOwner.PlayAnimation((hasState) ? mAnimationName : "Attack");
                    yield return new WaitForSeconds(mEffectTime);
                    if(mOwner.mSkillClips.Count > 0)
                        AudioManager.PlaySfx(mOwner.mSkillClips[UnityEngine.Random.Range(0, mOwner.mSkillClips.Count - 1)].Clip, 1.0f);
                    Shoot();
                    yield return new WaitUntil(() => mProjectile.GetComponent<Projectile>().isCollide == true);
                }
                else if (mShootType == SKillShootType.Melee)
                {
                    
                    yield return new WaitForSeconds(mEffectTime);
                    mOwner.PlayAnimation((hasState) ? mAnimationName : "Attack");

                    bool projectile = Melee();
                    if (mOwner.mSkillClips.Count > 0 && projectile)
                        AudioManager.PlaySfx(mOwner.mSkillClips[UnityEngine.Random.Range(0, mOwner.mSkillClips.Count - 1)].Clip, 1.0f);
                    yield return new WaitForSeconds(mOwner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + mEffectTime);
                        if (mOwner.mSkillClips.Count > 0 && !projectile)
                            AudioManager.PlaySfx(mOwner.mSkillClips[UnityEngine.Random.Range(0, mOwner.mSkillClips.Count - 1)].Clip, 1.0f);
                    CommonState();
                    yield return new WaitForSeconds(0.15f);
                }
                else if(mShootType == SKillShootType.Instant)
                {
                    mOwner.PlayAnimation((hasState) ? mAnimationName : "Attack");
                    if (mOwner.mSkillClips.Count > 0)
                        AudioManager.PlaySfx(mOwner.mSkillClips[UnityEngine.Random.Range(0, mOwner.mSkillClips.Count - 1)].Clip, 1.0f);
                    yield return new WaitForSeconds(mEffectTime);
                    CommonState();
                }    
                mOwner.mAiBuild.actionEvent = ActionEvent.BackWalk;

                if(Resources.Load<GameObject>("Prefabs/Effects/" + mName + "_Effect") != null)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + mName + "_Effect")
    , mTarget.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
                    Destroy(go, go.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                }
            }
            else
                BattleManager.Instance.Cancel();
        }
        if(mOwner.mAiBuild.actionEvent == ActionEvent.BackWalk)
        {
            yield return new WaitUntil(() => mOwner.mAiBuild.actionEvent == ActionEvent.Busy);
        }
        isComplete = true;
        yield return null;
    }

    private void CommonState()
    {
        float newValue = mValue + mOwner.mStatus.mMagicPower + mOwner.mBonusStatus.mMagicPower;
        switch (mSkillType)
        {
            case SkillType.Attack:
                {
                    mTarget.TakeDamage(newValue, DamageType.Magical);

                    foreach (var buff in mBuffList)
                        mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
                    foreach (var nerf in mNerfList)
                        mTarget.SetNerf(nerf.Initialize(mOwner, mTarget));
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
        mProjectile = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mName), mOwner.transform.position + dir * mStartPosition.x, Quaternion.identity);
        mProjectile.GetComponent<Projectile>().mDamage = newValue;
        mProjectile.transform.LookAt(dir);

        mProjectile.GetComponent<Projectile>().Initialize(mTarget,
    () => {
        mTarget.TakeDamage(newValue, DamageType.Magical);
        foreach (var buff in mBuffList)
            mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
        foreach (var nerf in mNerfList)
            mOwner.SetNerf(nerf.Initialize(mOwner, mTarget));
    });

    }

    private bool Melee()
    {
        Vector3 dir = (mTarget.transform.position - mOwner.transform.position).normalized;
        if(Resources.Load<GameObject>("Prefabs/Skills/" + mName))
        {
            mProjectile = Instantiate(Resources.Load<GameObject>("Prefabs/Skills/" + mName), mOwner.transform.position + dir * mStartPosition.x, Quaternion.identity);
            mProjectile.transform.localPosition += new Vector3(3.0f, mOwner.transform.GetComponent<BoxCollider>().size.y + mStartPosition.y);
            mProjectile.GetComponent<SpriteRenderer>().sortingOrder = mOwner.GetComponent<SpriteRenderer>().sortingOrder;
            mProjectile.GetComponent<SpriteRenderer>().flipX = (mTarget.mFlag != Flag.Player);
            Destroy(mProjectile.gameObject, mOwner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 0.3f);
            return true;
        }
        return false;
    }

    private void Raycasting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (mProperty == SkillProperty.Friendly)
        {
            if (Physics.Raycast(ray, out hit, 500, (mOwner.GetComponent<Unit>().mFlag == Flag.Player) ? LayerMask.GetMask("Ally") 
                : LayerMask.GetMask("Enemy")))
            {
                mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                mTarget?.mSelected.SetActive(true);
            }
            else
                mTarget?.mSelected.SetActive(false);
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 500, (mOwner.GetComponent<Unit>().mFlag == Flag.Player) ? LayerMask.GetMask("Enemy")
                : LayerMask.GetMask("Ally")))
            {
                mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                mTarget?.mSelected.SetActive(true);
            }
            else
                mTarget?.mSelected.SetActive(false);
        }

    }

    public override void Initialize(Unit owner)
    {
        mOwner = owner;
        if (mValue <= 0.0f)
            mValue = owner.mStatus.mMagicPower;
    }

    public override IEnumerator Effect()
    {
        var colorParameter = new UnityEngine.Rendering.VolumeParameter<Color>();
        switch (mElement)
        {
            case SkillElement.Normal:
                break;
            case SkillElement.Holy:
                colorParameter.value = Color.white;
                break;
            case SkillElement.Shadow:
                colorParameter.value = Color.green;
                break;
            case SkillElement.Water:
                colorParameter.value = Color.cyan;
                break;
            case SkillElement.Fire:
                colorParameter.value = Color.red;
                break;
            case SkillElement.Undead:
                colorParameter.value = Color.black;
                break;
        }
        if (mElement == SkillElement.Normal)
        {
            yield return null;
        }
        else
        {
            CameraSwitcher.Instance.mBloom.tint.SetValue(colorParameter);

            yield return new WaitUntil(() => mOwner.mAiBuild.actionEvent == ActionEvent.Busy);
            while (CameraSwitcher.Instance.mBloom.intensity.value < 2.0f)
            {
                CameraSwitcher.Instance.mBloom.intensity.value += Time.deltaTime * 2.0f;
                yield return null;
            }
            yield return new WaitUntil(() => mOwner.mAiBuild.actionEvent != ActionEvent.Busy);

            while (CameraSwitcher.Instance.mBloom.intensity.value != 0.0f)
            {
                CameraSwitcher.Instance.mBloom.intensity.value -= Time.deltaTime * 2.0f;
                yield return null;
            }
        }

    }
}
