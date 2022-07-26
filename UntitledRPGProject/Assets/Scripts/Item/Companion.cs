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
        GameObject go = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Units/Allys/" + Name), PlayerController.Instance.transform.position, Quaternion.identity, PlayerController.Instance.transform);
        
        go.GetComponent<Unit>().ResetUnit();
        go.SetActive(false);
        PlayerController.Instance.mHeroes.Add(go);
        GameManager.Instance.AssignCharacter(Name);
        isSuccess = true;
    }

    public override void End()
    {
        GameObject model = ResourceManager.GetResource<GameObject>("Prefabs/Effects/CompanionEffect");
        GameObject go = Instantiate(model, mTransform.position + new Vector3(0.0f,0.5f,0.0f), Quaternion.Euler(model.transform.eulerAngles));
        Destroy(go, 1.5f);
    }
}
