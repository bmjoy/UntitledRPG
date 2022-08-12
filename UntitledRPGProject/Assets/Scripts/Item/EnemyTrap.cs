using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrap : Item
{
    [HideInInspector]
    public Transform mTransform;
    [HideInInspector]
    public bool isSuccess = false;
    public override void Apply()
    {
        if (isSuccess)
            return;
        EnemyTrapInfo info = (EnemyTrapInfo)Info;
        if (info.mEnemyUnits.Length == 0)
            return;
        Vector3 pos = mTransform.position - new Vector3(0, 2, 0);
        GameObject place =  new GameObject("Spawner");
        place.transform.position = pos;
        place.AddComponent<EnemySpawner>().mEnemyList = info.mEnemyUnits;
        place.GetComponent<EnemySpawner>().Spawn();
        isSuccess = true;
    }

    public override void End()
    {
    }
}
