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
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Units/Allys/" + Name), PlayerController.Instance.transform.position, Quaternion.identity, PlayerController.Instance.transform);
        go.GetComponent<Unit>().ResetUnit();
        go.SetActive(false);
        PlayerController.Instance.mHeroes.Add(go);
        GameManager.Instance.AssignCharacter(Name);
        isSuccess = true;
    }

    public override void End()
    {
    }
}
