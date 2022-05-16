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
        agent.mConditions.isPicked = true;
        agent.mOrder = Order.Standby;
        agent.BuffAndNerfTick();
        agent.mField.GetComponent<Field>().Picked(true);
        IsSucceeded = false;
        IsSearched = false;
        randomNumber = -1;
    }

    public override void Execute(Unit agent)
    {
        if (agent.mAiBuild.type == AIType.Auto)
        {
            IsSucceeded = (IsSucceeded == false) ? Find(agent) : true;
            if(!IsSearched)
            {
                agent.StartCoroutine(WaitforSecond(agent));
                IsSearched = true;
            }
        }

    }

    public override void Exit(Unit agent)
    {
        randomNumber = -1;
    }

    private IEnumerator WaitforSecond(Unit agent)
    {
        yield return new WaitForSeconds(1.0f);
        int percent = Mathf.RoundToInt((100 * agent.mStatus.mHealth) / agent.mStatus.mMaxHealth);
        int targetPercent = Mathf.RoundToInt((100 * agent.mTarget.mStatus.mHealth) / agent.mTarget.mStatus.mMaxHealth);
        randomNumber = (agent.mAiBuild.property == AIProperty.Offensive) ?
            UnityEngine.Random.Range(30, 50) : randomNumber = UnityEngine.Random.Range(50, 70);

        bool condition1 = percent > randomNumber;
        bool condition2 = agent.mStatus.mHealth > agent.mTarget.mStatus.mHealth;
        bool condition3 = targetPercent <= BattleManager.Instance.mPercentageHP;

        agent.mTarget?.mSelected.SetActive(true);

        agent.mAiBuild.stateMachine.ChangeState((condition1 || condition2 || condition3) ? "Attack" : "Defend");
    }

    private bool Find(Unit agent)
    {
        List<GameObject> list = new List<GameObject>((agent.mFlag == Flag.Enemy) ? PlayerController.Instance.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()
            : GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()); 
        if (list.Count == 0) return false;
        agent.mTarget = list[Random.Range(0, list.Count)].GetComponent<Unit>();
        return !agent.mTarget.mConditions.isDied;
    }
}
