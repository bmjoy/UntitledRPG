using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Standby : State
{
    private int randomNumber;
    public override void Enter(Unit agent)
    {
        List<GameObject> list = new List<GameObject>();
        list = (agent.mFlag == Flag.Enemy) ? GameManager.Instance.mPlayer.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()
            : GameManager.Instance.mEnemyProwler.EnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList();
        agent.mTarget = list[Random.Range(0, list.Count)].GetComponent<Unit>();
        randomNumber = -1;
    }

    public override void Execute(Unit agent)
    {
        if(agent.mConditions.isPicked && !isAct)
        {
            isAct = true;
            agent.StartCoroutine(WaitforSecond(agent));
        }
    }

    public override void Exit(Unit agent)
    {
        randomNumber = -1;
        isAct = false;
    }

    private IEnumerator WaitforSecond(Unit agent)
    {
        yield return new WaitForSeconds(2.0f);
        int percent = (int)Mathf.RoundToInt((100 * agent.mStatus.mHealth) / agent.mStatus.mMaxHealth);
        randomNumber = (agent.mAiBuild.property == AIProperty.Offensive) ?
            UnityEngine.Random.Range(30, 50) : randomNumber = UnityEngine.Random.Range(50, 70);
        if (percent > randomNumber)
            agent.mAiBuild.stateMachine.ChangeState("Attack");
        else
            agent.mAiBuild.stateMachine.ChangeState("Defend");
    }
}
