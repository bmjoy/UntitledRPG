using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/TargetAbility")]
public class TargetAbility : Skill_Setting
{
    private Unit mTarget;
    private GameObject mProjectile;
    public bool IsShootType = false;

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
                mOwner.mStatus.mMana -= mManaCost;
                if (IsShootType && isActive)
                {
                    Shoot();
                    yield return new WaitUntil(() => mProjectile.GetComponent<Projectile>().isCollide == true);
                }
                else if (!IsShootType && isActive)
                {
                    mOwner.mAiBuild.actionEvent = ActionEvent.MagicWalk;
                    yield return new WaitUntil(() => mOwner.mAiBuild.actionEvent == ActionEvent.Busy);
                    yield return new WaitForSeconds(0.5f);
                    mOwner.PlayAnimation("Skill");
                    Melee();
                    yield return new WaitForSeconds(1.0f);
                    mOwner.mAiBuild.actionEvent = ActionEvent.BackWalk;
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
                            mTarget.SetBuff(mBuff.Initialize(mOwner));
                        }
                        break;
                    case SkillType.AttackNerf:
                        {
                            mTarget.TakeDamage(mValue, DamageType.Magical);
                            mTarget.SetNerf(mNerf.Initialize(mTarget));
                        }
                        break;
                    case SkillType.Buff:
                        {
                            mOwner.SetBuff(mBuff.Initialize(mTarget));
                        }
                        break;
                    case SkillType.BuffNerf:
                        {
                            mOwner.SetBuff(mBuff.Initialize(mTarget));
                            mOwner.SetNerf(mNerf.Initialize(mTarget));
                        }
                        break;
                    case SkillType.Nerf:
                        {
                            mOwner.SetNerf(mNerf.Initialize(mTarget));
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
                            mTarget.SetBuff(mBuff.Initialize(mTarget));
                            break;
                        }

                    case SkillType.HealNerf:
                        {
                            mTarget.TakeRecover(mValue);
                            mTarget.SetNerf(mNerf.Initialize(mTarget));
                            break;
                        }
                    case SkillType.Summon:
                        break;
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

    private void Shoot()
    {
        mOwner.PlayAnimation("Skill");
        Vector3 dir = (mTarget.transform.position - mOwner.transform.position).normalized;
        mProjectile = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mName), mOwner.transform.position + dir * 1.5f, Quaternion.identity);
        mProjectile.transform.LookAt(dir);
        mProjectile.GetComponent<Projectile>().Initialize(mTarget, dir, DamageType.Magical, null);
    }

    private void Melee()
    {
        Vector3 dir = (mTarget.transform.position - mOwner.transform.position).normalized;
        mProjectile = Instantiate(Resources.Load<GameObject>("Prefabs/Skills/" + mName), mOwner.transform.position + dir * mStartPosition.x, Quaternion.identity);
        mProjectile.transform.position += new Vector3(0.0f, mOwner.transform.GetComponent<BoxCollider>().size.y + mStartPosition.y);
        mProjectile.GetComponent<SpriteRenderer>().sortingOrder = mOwner.mSpriteRenderer.sortingOrder;
        mProjectile.GetComponent<SpriteRenderer>().flipX = (mTarget.mFlag != Flag.Player);
        Destroy(mProjectile.gameObject, 1.0f);
    }

    private void Raycasting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (mProperty == SkillProperty.Friendly)
        {
            if (Physics.Raycast(ray, out hit, 100, (mOwner.GetComponent<Unit>().mFlag == Flag.Player) ? LayerMask.GetMask("Ally") 
                : LayerMask.GetMask("Enemy")))
            {
                if (Input.GetMouseButtonDown(0) && hit.transform.GetComponent<Unit>().mConditions.isDied == false)
                    mTarget = hit.transform.GetComponent<Unit>();
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 100, (mOwner.GetComponent<Unit>().mFlag == Flag.Player) ? LayerMask.GetMask("Enemy")
                : LayerMask.GetMask("Ally")))
            {
                if (Input.GetMouseButtonDown(0) && hit.transform.GetComponent<Unit>().mConditions.isDied == false)
                    mTarget = hit.transform.GetComponent<Unit>();
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
