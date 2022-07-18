using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : Unit
{
    [HideInInspector]
    public BigHealthBar mMyHealthBar;
    public WeaponType mWeaponType;

    private SkillTreeBonus mPreservedSkillTreeBonus = new SkillTreeBonus();
    protected override void Start()
    {
        base.Start();
        GameObject[] agent = GameObject.FindGameObjectsWithTag("Player");
        if (agent.Length > 1)
        {
            for (int i = 0; i < agent.Length; i++)
                Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
        }
        mAiBuild.type = AIType.Manual;
        mTarget = null;
    }

    protected override void Update()
    {
        base.Update();
        if(mMyHealthBar != null)
        {
            mMyHealthBar.mCurrentHealth = mStatus.mHealth;
            mMyHealthBar.mMaxHealth = mStatus.mMaxHealth;
            mMyHealthBar.mCurrentMana = mStatus.mMana;
            mMyHealthBar.mMaxMana = mStatus.mMaxMana;
        }
    }
    private bool isFinish = false;
    protected override IEnumerator BattleState(DamageType type)
    {
        if (mStatus.mDamage + mBonusStatus.mDamage > mTarget.mStatus.mHealth || GameManager.Instance.mFinisherChance >= Mathf.RoundToInt((100 * mTarget.mStatus.mHealth) / mTarget.mStatus.mMaxHealth))
        {
            mProjectileName += "_Finisher";
            mStatus.mDamage *= 1.2f;
            isFinish = true;
        }
        else
        {
            mProjectileName = mSetting.Name;
            isFinish = false;
        }

        var doubleAttack = SkillTreeManager._Instance.mTotalBounsAbilities.Find(s => s.Type == SkillTree_BounsAbility.SkillTreeAbilityType.DoubleAttack);

        if (doubleAttack != null && Random.Range(1, 100) >= doubleAttack.Value)
        {
            if (mType == AttackType.Melee)
            {
                mAiBuild.SetActionEvent(ActionEvent.AttackWalk);
                yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
                mirror?.Play("Attack");
                if (mTarget)
                {
                    if (mActionTrigger != null)
                    {
                        mActionTrigger?.Invoke();
                        yield return new WaitUntil(() => GetComponent<ActionTrigger>().isCompleted);
                    }
                    else
                    {
                        mAnimator.Play(MyAttackAnim[Random.Range(0, MyAttackAnim.Count)]);
                        if (mAttackClips.Count > 0)
                            AudioManager.PlaySfx(mAttackClips[Random.Range(0, mAttackClips.Count)].Clip, 0.6f);
                        yield return new WaitForSeconds(mAttackTime);

                        mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                    }
                    StartCoroutine(CounterState(mTarget.mStatus.mDamage));
                }
            }
            else if (mType == AttackType.Range)
            {
                mAiBuild.SetActionEvent(ActionEvent.Busy);
                mAnimator.Play("Attack");
                mirror?.Play("Attack");
                if (mAttackClips.Count > 0)
                    AudioManager.PlaySfx(mAttackClips[Random.Range(0, mAttackClips.Count)].Clip, 0.6f);
                yield return new WaitForSeconds(mAttackTime);
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mProjectileName), transform.Find("Fire").position, Quaternion.identity);
                if (go.GetComponent<SpriteRenderer>())
                    go.GetComponent<SpriteRenderer>().flipX = transform.GetComponent<SpriteRenderer>().flipX;
                Bullet bullet = go.GetComponent<Bullet>();
                bullet.Initialize(mTarget.transform, mStatus.mDamage + mBonusStatus.mDamage);

                yield return new WaitUntil(() => bullet.isDamaged == true);
            }
            else if (mType == AttackType.Instant)
            {
                mirror?.Play("Attack");
                mAiBuild.SetActionEvent(ActionEvent.Busy);
                mAnimator.Play("Attack");
                if (mAttackClips.Count > 0)
                    AudioManager.PlaySfx(mAttackClips[Random.Range(0, mAttackClips.Count)].Clip, 0.6f);
                yield return new WaitForSeconds(mAttackTime);
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mProjectileName), mTarget.transform.position + new Vector3(0.0f, 0.1f, 0.0f), Quaternion.identity);
                mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                Destroy(go, 2.0f);
                yield return new WaitUntil(() => go == null);
            }
            yield return new WaitForSeconds(0.8f);
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mDoubleAttackSFX);
            GameObject effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/DoubleAttackEffect");
            GameObject d_effect = Instantiate(effectPrefab, transform.position + new Vector3(0.0f, GetComponent<BoxCollider>().size.y / 2.0f, 0.0f), Quaternion.Euler(effectPrefab.transform.eulerAngles));
            Destroy(d_effect, 1.0f);
            yield return StartCoroutine(base.BattleState(type));
        }
        else
            yield return StartCoroutine(base.BattleState(type));
    }

    public override void TurnEnded()
    {
        base.TurnEnded();
        if(isFinish)
            mStatus.mDamage /= 1.2f;
        isFinish = false;
        mProjectileName = mSetting.Name;
    }

    public override bool TakeDamage(float dmg, DamageType type)
    {
        bool isHit = true;
        GameObject effectPrefab = null;
        GameObject effect = null;

        if (mPreservedSkillTreeBonus.IsShield && bonusShield > 0.0f)
        {
            isHit = !DodgeState();

            if(isHit)
            {
                bonusShield -= dmg;

                GameObject damage = Instantiate(mCanvas.transform.Find("DamageValue").gameObject
        , mCanvas.transform.position - new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity, mCanvas.transform);
                damage.SetActive(true);
                damage.GetComponent<TextMeshProUGUI>().text = "<color=blue>" + dmg.ToString() + "</color>";
                Destroy(damage, 1.1f);
                if (bonusShield < 0.0f)
                {
                    AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mShieldExplosionSFX);
                    effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ShieldExplosion");
                    effect = Instantiate(effectPrefab, transform.position + new Vector3(0.0f, GetComponent<BoxCollider>().size.y / 2.0f, 0.0f), Quaternion.Euler(effectPrefab.transform.eulerAngles));
                    Destroy(effect, 1.0f);                   
                    Destroy(mPreservedSkillTreeBonus.mShield);
                    mPreservedSkillTreeBonus.mShield = null;
                }
                else
                {
                    AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mShieldHitSFX);
                    effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ShieldHit");
                    effect = Instantiate(effectPrefab, transform.position + new Vector3(0.0f, GetComponent<BoxCollider>().size.y / 2.0f, 0.0f), Quaternion.Euler(effectPrefab.transform.eulerAngles));
                    Destroy(effect, 1.0f);
                }
            }
            else
            {
                mAiBuild.actionEvent = ActionEvent.DodgeWalk;
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mDodgeSFX);
            }
        }
        else
            isHit = base.TakeDamage(dmg, type);

        mMyHealthBar.mCurrentHealth = (mStatus.mHealth > 0.0f) ? mStatus.mHealth : 0.0f;
        if(isHit && mMyHealthBar.mCurrentHealth > 0.0f)
        {
            mMyHealthBar.StartCoroutine(mMyHealthBar.PlayBleed());
        }

        if (mConditions.isDied)
        {
            mPreservedSkillTreeBonus = new SkillTreeBonus();
            bonusHP = bonusMP = bonusAP = bonusMAP =
    bonusHPRE = bonusMPRE = bonusShield = bonusDmg = bonusMagicDmg = 0.0f;
            if (mPreservedSkillTreeBonus.mShield)
            {
                Destroy(mPreservedSkillTreeBonus.mShield);
                mPreservedSkillTreeBonus.mShield = null;
            }
        }

        return isHit;
    }

    public override void TakeRecover(float val)
    {
        base.TakeRecover(val);
        if(mMyHealthBar)
            mMyHealthBar.mNextHealth = mStatus.mHealth;
    }

    public override void TakeRecoverMana(float val)
    {
        base.TakeRecoverMana(val);
        if (mMyHealthBar)
            mMyHealthBar.mCurrentMana = mStatus.mMana;
    }

    float bonusHP = 0.0f;
    float bonusMP = 0.0f;
    float bonusAP = 0.0f;
    float bonusMAP = 0.0f;
    float bonusHPRE = 0.0f;
    float bonusMPRE = 0.0f;
    float bonusDmg = 0.0f;
    float bonusMagicDmg = 0.0f;
    float bonusShield = 0.0f;

    public override void EnableUnit(int index)
    {
        base.EnableUnit(index);

        mStatus.mMaxHealth += bonusHP;
        mStatus.mHealth += bonusHP;
        mStatus.mMaxMana += bonusMP;
        mStatus.mMana += bonusMP;
        mStatus.mArmor += bonusAP;
        mStatus.mMagic_Resistance += bonusMAP;   
        mStatus.mDamage += bonusDmg;
        mStatus.mMagicPower += bonusMagicDmg;
    }

    public override void DisableUnit(Vector3 pos)
    {
        base.DisableUnit(pos);

        mStatus.mMaxHealth -= bonusHP;
        mStatus.mHealth -= bonusHP;
        mStatus.mMaxMana -= bonusMP;
        mStatus.mMana -= bonusMP;
        mStatus.mArmor -= bonusAP;
        mStatus.mMagic_Resistance -= bonusMAP;
        mStatus.mDamage -= bonusDmg;
        mStatus.mMagicPower -= bonusMagicDmg;

        bonusHP = bonusMP = bonusAP = bonusMAP = 
            bonusHPRE = bonusMPRE = bonusShield = bonusDmg = bonusMagicDmg = 0.0f;
        if (mPreservedSkillTreeBonus.mShield)
        {
            Destroy(mPreservedSkillTreeBonus.mShield);
            mPreservedSkillTreeBonus.mShield = null;
        }
    }

    public void ApplySkillBonus(SkillTreeBonus bonus)
    {
        mPreservedSkillTreeBonus = bonus;
        if (mPreservedSkillTreeBonus.mHealth > 0.0f)
            bonusHP = Mathf.RoundToInt(mStatus.mMaxHealth * mPreservedSkillTreeBonus.mHealth / 100.0f);
        if (mPreservedSkillTreeBonus.mMana > 0.0f)
            bonusMP = Mathf.RoundToInt(mStatus.mMaxMana * mPreservedSkillTreeBonus.mMana / 100.0f);
        if (mPreservedSkillTreeBonus.mArmor > 0.0f)
        {
            bonusAP = (mStatus.mArmor > 0.0f) ? Mathf.RoundToInt(mStatus.mArmor * mPreservedSkillTreeBonus.mArmor / 100.0f)
                : mPreservedSkillTreeBonus.mArmor / 10.0f;
            bonusMAP = (mStatus.mMagic_Resistance > 0.0f) ? Mathf.RoundToInt(mStatus.mMagic_Resistance * mPreservedSkillTreeBonus.mArmor / 100.0f)
                : mPreservedSkillTreeBonus.mArmor / 10.0f;
        }
        if(mPreservedSkillTreeBonus.mDamage > 0.0f)
        {
            bonusDmg = Mathf.RoundToInt(mStatus.mDamage * mPreservedSkillTreeBonus.mDamage / 100.0f);
            bonusMagicDmg = Mathf.RoundToInt(mStatus.mMagicPower * mPreservedSkillTreeBonus.mDamage / 100.0f);
        }
        if(mPreservedSkillTreeBonus.IsHPRegeneration)
        {
            bonusHPRE = Mathf.RoundToInt(mStatus.mMaxHealth * mPreservedSkillTreeBonus.mHPRegeneration / 100.0f);
        }        
        if(mPreservedSkillTreeBonus.IsMPRegeneration)
        {
            bonusMPRE = Mathf.RoundToInt(mStatus.mMaxMana * mPreservedSkillTreeBonus.mMPRegeneration / 100.0f);
        }
        if(mPreservedSkillTreeBonus.IsShield)
        {
            bonusShield = Mathf.RoundToInt(mStatus.mMaxMana * mPreservedSkillTreeBonus.mShieldValue / 100.0f);
            GameObject ShieldPrefab = Resources.Load<GameObject>("Prefabs/Effects/Shield");
            mPreservedSkillTreeBonus.mShield = Instantiate(ShieldPrefab
, transform.position + new Vector3(0.0f, 0.2f, 0.0f), Quaternion.Euler(ShieldPrefab.transform.eulerAngles),transform);
        }
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
        if(mPreservedSkillTreeBonus.IsHPRegeneration)
        {
            TakeRecover(bonusHPRE);
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mHealingSFX, 0.3f);
            GameObject recover = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Healing_Effect")
, transform.position + new Vector3(0.0f, 0.1f, 0.0f), Quaternion.identity);
            Destroy(recover, 1.5f);
        }
        if(mPreservedSkillTreeBonus.IsMPRegeneration)
        {
            TakeRecoverMana(bonusHPRE);
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mHealingSFX, 0.3f);
            GameObject recover = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MP_Healing_Effect")
, transform.position + new Vector3(0.0f, 0.1f, 0.0f), Quaternion.identity);
            Destroy(recover, 1.5f);
        }
    }
}
