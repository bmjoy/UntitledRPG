using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Abilities/SummonAbility")]
public class SummonAbility : Skill_Setting
{
    [SerializeField]
    EnemyUnit mSummonUnit = EnemyUnit.Ghoul;
    [HideInInspector]
    public int mTurn = 0;
    public Field mSelectedField = null;
    [SerializeField]
    public int mSkillUsageEveryTurn = 3;

    public override void Activate(MonoBehaviour parent)
    {
        isActive = false;
        parent.StopAllCoroutines();
        if (mOwner.mFlag == Flag.Enemy)
        {
            for (int i = 0; i < BattleManager.Instance.mCurrentField.transform.Find("EnemyFields").childCount; ++i)
            {
                var obj = BattleManager.Instance.mCurrentField.transform.Find("EnemyFields").GetChild(i).gameObject.GetComponent<Field>();
                if (obj.GetComponent<Field>().IsExist == false)
                {
                    mSelectedField = obj;
                    mSelectedField.TargetedMagicHostile(true);
                    break;
                }
            }
        }
        else if (mOwner.mFlag == Flag.Player)
        {
            for (int i = 0; i < BattleManager.Instance.mCurrentField.transform.Find("PlayerFields").childCount; i++)
            {
                var obj = BattleManager.Instance.mCurrentField.transform.Find("PlayerFields").GetChild(i).gameObject.GetComponent<Field>();
                if (obj.GetComponent<Field>().IsExist == false)
                {
                    mSelectedField = obj;
                    mSelectedField.TargetedMagicHostile(true);
                    break;
                }
            }
        }
        else
            mSelectedField = null;
        if (mOwner.mAiBuild.type == AIBuild.AIType.Auto)
        {
            if (mSelectedField && mSkillUsageEveryTurn * 4 != mTurn)
            {
                mTurn++;
            }
            if (mTurn % 3 == 0 && mSelectedField)
                parent.StartCoroutine(WaitforDecision());
            else
            {
                mSelectedField?.TargetedMagicHostile(false);
                BattleManager.Instance.Defend();
            }
        }
        else
        {
            if(mSelectedField)
                parent.StartCoroutine(WaitforDecision());
            else
            {
                mSelectedField?.TargetedMagicHostile(false);
                BattleManager.Instance.Defend();
            }
        }
    }

    public override void Initialize(Unit owner)
    {
        mOwner = owner;
        mTurn = 2;
    }

    public override IEnumerator WaitforDecision()
    {
        if (mOwner.mStatus.mMana < mManaCost)
            BattleManager.Instance.Cancel();
        else
        {
            if (mOwner.mAiBuild.type == AIBuild.AIType.Manual)
            {
                UIManager.Instance.ChangeOrderBarText(UIManager.Instance.mStorage.mTextForAccpet);
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
                bool hasState = mOwner.mAnimator.HasState(0, Animator.StringToHash("Skill"));
                mOwner.mAnimator.Play((hasState) ? "Skill" : "Attack");
                CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.ZoomCamera(mEffectTime / 2.0f, Vector3.Lerp(mOwner.transform.position, mSelectedField.transform.position, 0.5f)));

                mActionTrigger?.Invoke();
                
                if (ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + mName + "_Effect") != null)
                {
                    GameObject go = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + mName + "_Effect")
    , mSelectedField.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
                    Destroy(go, mEffectTime);
                }


                if (mSelectedField)
                {
                    GameObject unit = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Units/Enemys/" + mSummonUnit.ToString() + "_Unit"), mSelectedField.transform.position, Quaternion.identity, mOwner.transform.parent);
                    unit.GetComponent<Animator>().SetTrigger("Spawn");
                    unit.GetComponent<Unit>().mField = mSelectedField.GetComponent<Field>();
                    unit.GetComponent<Unit>().mField.mUnit = unit.GetComponent<Unit>();
                    unit.GetComponent<Unit>().mOrder = Order.Standby;
                    unit.GetComponent<Unit>().mFlag = mOwner.mFlag;
                    BattleManager.Instance.mOrders.Enqueue(unit.GetComponent<Unit>());
                    BattleManager.Instance.mUnits.Add(unit);
                    if(mOwner.mFlag == Flag.Enemy)
                        BattleManager.Instance.mEnemies.Add(unit);
                    UIManager.Instance.mOrderBar.EnqueueSignleOrder(unit.GetComponent<Unit>());
                }
                if (mOwner.mSkillClips.Count() > 0)
                    AudioManager.PlaySfx(mOwner.mSkillClips.ElementAt(UnityEngine.Random.Range(0, mOwner.mSkillClips.Count())).Clip, 1.0f);
                yield return new WaitForSeconds(mEffectTime);
                mOwner.mStatus.mMana -= mManaCost;
                mSelectedField.TargetedMagicHostile(false);
                mSelectedField = null;
            }
            else
                BattleManager.Instance.Cancel();
        }

        isComplete = true;
        yield return null;
    }

}
