using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Standby : State
{
    private int randomNumber;
    private bool IsSucceeded = false;
    private bool IsSearched = false;
    public override void Enter(Unit agent)
    {
        agent.mConditions.isDefend = false;
        agent.mOrder = Order.Standby;
        agent.GetComponent<BuffAndNerfEntity>().Tick();
        agent.mField.GetComponent<Field>().Picked(true);
        IsSucceeded = false;
        IsSearched = false;
        randomNumber = -1;
    }

    public override void Execute(Unit agent)
    {
        if (agent.mAiBuild.type == AIType.Auto)
        {
            IsSucceeded = Find(agent);
            if(IsSucceeded && !IsSearched)
            {
                agent.StartCoroutine(WaitforSecond(agent));
                IsSearched = true;
            }
        }
    }

    public override void Exit(Unit agent)
    {
        randomNumber = -1;
        agent.mAiBuild.stateMachine.mPreferredTarget = null;
        agent.mTarget?.mSelected.SetActive(false);
    }

    private IEnumerator WaitforSecond(Unit agent)
    {
        yield return new WaitForSeconds(0.5f);
        int percent = Mathf.RoundToInt((100 * agent.mStatus.mHealth) / agent.mStatus.mMaxHealth);
        int targetPercent = Mathf.RoundToInt((100 * agent.mTarget.mStatus.mHealth) / agent.mTarget.mStatus.mMaxHealth);
        randomNumber = (agent.mAiBuild.property == AIProperty.Offensive) ?
            UnityEngine.Random.Range(10, 20) : randomNumber = UnityEngine.Random.Range(40, 60);

        bool condition1 = percent > randomNumber;
        bool condition2 = percent > targetPercent;
        bool condition3 = agent.mBuffNerfController.GetBuffCount() > 0;

        string behavior = ((condition1 || condition2 || condition3) && agent.mType != AttackType.None) ? "Attack" : "Defend";

        agent.mTarget?.mSelected.SetActive(true);

        if(agent.GetType() == typeof(Boss))
        {
            Boss boss = (Boss)agent;
            var database = boss.GetComponent<Boss_Skill_DataBase>();
            if (boss.mHealthTriggerPercentage.Length > 1)
            {
                if (boss.mActionTriggerComponent._isUltimate)
                {
                    database.ChangeSkill(database.mUltimateSkillIndex);
                    behavior = "Magic";
                }
                else
                {
                    for (int i = 0; i < boss.mHealthTriggerPercentage.Length; ++i)
                    {
                        if (boss.HalfHealthEvent(boss.mHealthTriggerPercentage[i]))
                        {
                            database.ChangeSkill(i);
                            behavior = (database.Mana <= boss.mStatus.mMana) ? "Magic" : "Attack";
                        }
                    }
                    if (condition3)
                        behavior = "Attack";
                }
            }
            else
            {
                if (database.Type == SkillType.Buff && !condition3)
                {
                    if (boss.HalfHealthEvent(boss.mHealthTriggerPercentage[0]))
                        behavior = (database.Mana <= boss.mStatus.mMana) ? "Magic" : "Attack";
                    else
                        behavior = (UnityEngine.Random.Range(0, 100) >= 50) ? "Defend" : "Attack";
                }
            }

        }
        else
        {
            if (agent.mSkillDataBase != null)
            {
                behavior = ((agent.mSkillDataBase.mSkill.GetType() == typeof(SummonAbility))
|| (agent.mStatus.mMana >= agent.mSkillDataBase.Mana && UnityEngine.Random.Range(0, 100) >= 50)) ? "Magic" : behavior;
            }

        }
        if(agent.mAiBuild.stateMachine.mPreferredTarget)
            behavior = (agent.mType != AttackType.None) ? "Attack" : "Defend";
        agent.mAiBuild.stateMachine.ChangeState(behavior);
    }

    private bool Find(Unit agent)
    {
        List<GameObject> list = new List<GameObject>((agent.mFlag == Flag.Enemy) ? PlayerController.Instance.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()
            : GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()); 
        if (list.Count == 0) return false;
        agent.mTarget = (agent.mAiBuild.stateMachine.mPreferredTarget) ? agent.mAiBuild.stateMachine.mPreferredTarget
            : list[Random.Range(0, list.Count)].GetComponent<Unit>();
        return !agent.mTarget.mConditions.isDied;
    }
}
