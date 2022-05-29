using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Abilities/SummonAbility")]
public class SummonAbility : Skill_Setting
{
    [SerializeField]
    EnemyUnit mSummonUnit = EnemyUnit.Ghoul;
    [HideInInspector]
    public int mTurn = 0;
    public GameObject mSelectedField = null;
    [SerializeField]
    public int mSkillUsageEveryTurn = 3;

    public override void Activate(MonoBehaviour parent)
    {
        isActive = false;
        parent.StopAllCoroutines();

        if (mOwner.mFlag == Flag.Enemy)
        {
            for (int i = 0; i < BattleManager.Instance.mCurrentField.transform.Find("EnemyFields").childCount; i++)
            {
                var obj = BattleManager.Instance.mCurrentField.transform.Find("EnemyFields").GetChild(i).gameObject;
                if (obj.GetComponent<Field>().IsExist == false)
                {
                    mSelectedField = obj;
                    break;
                }
            }
        }
        else if (mOwner.mFlag == Flag.Player)
        {
            for (int i = 0; i < BattleManager.Instance.mCurrentField.transform.Find("PlayerFields").childCount; i++)
            {
                var obj = BattleManager.Instance.mCurrentField.transform.Find("PlayerFields").GetChild(i).gameObject;
                if (obj.GetComponent<Field>().IsExist == false)
                {
                    mSelectedField = obj;
                    break;
                }
            }
        }
        else
            mSelectedField = null;
        if (mOwner.mAiBuild.type == AIType.Auto)
        {
            if (mSelectedField && mSkillUsageEveryTurn * 4 != mTurn)
                mTurn++;
            if (mTurn % 3 == 0 && mSelectedField)
                parent.StartCoroutine(WaitforDecision());
            else
                BattleManager.Instance.Defend();
        }
        else
        {
            if(mSelectedField)
                parent.StartCoroutine(WaitforDecision());
            else
                BattleManager.Instance.Defend();
        }
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
                colorParameter.value = Color.magenta;
                break;
        }
        if (mElement == SkillElement.Normal)
        {
            yield return null;
        }
        else
        {
            CameraSwitcher.Instance.mBloom.tint.SetValue(colorParameter);
            while (CameraSwitcher.Instance.mBloom.intensity.value < 2.0f)
            {
                CameraSwitcher.Instance.mBloom.intensity.value += Time.deltaTime * 2.0f;
                yield return null;
            }
            yield return new WaitForSeconds(mEffectTime);

            while (CameraSwitcher.Instance.mBloom.intensity.value != 0.0f)
            {
                CameraSwitcher.Instance.mBloom.intensity.value -= Time.deltaTime * 2.0f;
                yield return null;
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
            if (mOwner.mAiBuild.type == AIType.Manual)
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
                mOwner.StartCoroutine(Effect());
                bool hasState = mOwner.GetComponent<Animator>().HasState(0, Animator.StringToHash("Skill"));
                mOwner.PlayAnimation((hasState) ? "Skill" : "Attack");
                CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.ZoomCamera(mEffectTime / 2.0f, Vector3.Lerp(mOwner.transform.position, mSelectedField.transform.position, 0.5f)));

                mActionTrigger?.Invoke();
                
                if (Resources.Load<GameObject>("Prefabs/Effects/" + mName + "_Effect") != null)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + mName + "_Effect")
    , mSelectedField.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
                    Destroy(go, go.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                }
                yield return new WaitForSeconds(mEffectTime);
                mOwner.mStatus.mMana -= mManaCost;

                if (mSelectedField)
                {
                    GameObject unit = Instantiate(Resources.Load<GameObject>("Prefabs/Units/Enemys/" + mSummonUnit.ToString() + "_Unit"), mSelectedField.transform.position, Quaternion.identity);
                    unit.GetComponent<Animator>().SetTrigger("Spawn");
                    unit.GetComponent<Unit>().mField = mSelectedField;
                    unit.GetComponent<Unit>().ResetUnit();
                    unit.GetComponent<Unit>().mOrder = Order.Standby;
                    unit.GetComponent<Unit>().mFlag = mOwner.mFlag;
                    unit.transform.SetParent(mOwner.transform.parent);
                    BattleManager.Instance.mOrders.Enqueue(unit.GetComponent<Unit>());
                    BattleManager.Instance.mUnits.Add(unit);
                    UIManager.Instance.mOrderbar.GetComponent<OrderBar>().EnqueueSignleOrder(unit.GetComponent<Unit>());
                }

            }
            else
                BattleManager.Instance.Cancel();
        }

        isComplete = true;
        yield return null;
    }

}
