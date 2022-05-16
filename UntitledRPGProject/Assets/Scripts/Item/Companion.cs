using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Companion")]
public class Companion : Item
{
    public Companion(string name, int val, int amount) : base(name, val, amount)
    {
    }

    public override void Apply()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/" + mName), PlayerController.Instance.transform.position, Quaternion.identity);
        go.transform.SetParent(PlayerController.Instance.transform);
        go.GetComponent<Unit>().ResetUnit();
        go.SetActive(false);
        PlayerController.Instance.mHeroes.Add(go);
    }

    public override void End()
    {
        GameObject.Destroy(transform.gameObject);
    }
}
