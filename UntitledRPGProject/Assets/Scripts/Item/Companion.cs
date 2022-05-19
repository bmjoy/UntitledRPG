using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : Item
{
    [HideInInspector]
    public Transform mTransform;
    [HideInInspector]
    public bool isSuccess = false;
    public override void Apply()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/" + Name), PlayerController.Instance.transform.position, Quaternion.identity);
        go.transform.SetParent(PlayerController.Instance.transform);
        go.GetComponent<Unit>().ResetUnit();
        go.SetActive(false);
        PlayerController.Instance.mHeroes.Add(go);
        isSuccess = true;
    }

    public override void End()
    {
    }
}
