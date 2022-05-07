using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderBar : MonoBehaviour
{

    //private List<Image> queueImages = new List<Image>();
    private Dictionary<Unit, GameObject> queueImages = new Dictionary<Unit, GameObject>();
    private Image image;

    private Vector3 EndPosition = new Vector3(-80.0f, 0.0f);
    private GameObject key = null;

    void Start()
    {
        image = transform.GetComponent<Image>();
        
        BattleManager.Instance.onEnqueuingOrderEvent += EnqueueOrder;
        BattleManager.Instance.onMovingOrderEvent += MoveOrder;
        BattleManager.Instance.onDequeuingOrderEvent += DequeueOrder;
        BattleManager.Instance.onFinishOrderEvent += Clear;
        gameObject.SetActive(false);
    }

    private void EnqueueOrder()
    {
        queueImages.Clear();
        Queue<Unit> unitList = BattleManager.Instance.mOrders;
        int i = 0;
        foreach(Unit unit in unitList)
        {
            GameObject go = new GameObject("Sprite");
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(0.0f + 40.0f * i, 5.0f);
            go.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
            go.transform.localRotation = Quaternion.Euler(0, 180, 0);
            Sprite sprite = go.AddComponent<Image>().sprite = unit.mSetting.BasicSprite;
            queueImages.Add(unit, go);
            i++;
        }
    }

    private void Update()
    {
        if(key != null)
        {
            key.transform.localPosition += new Vector3(-image.rectTransform.rect.width * Time.deltaTime, 0.0f, 0.0f);
            foreach(var unit in queueImages)
            {
                if (unit.Value == key)
                    continue;
                if(unit.Value.transform.localPosition.x > -260)
                    unit.Value.transform.localPosition += new Vector3(-50.0f * unit.Key.mStatus.mAgility * Time.deltaTime, 0.0f, 0.0f);
            }
            if (key.transform.localPosition.x <= -320)
                key = null;
        }
    }

    private void MoveOrder()
    {
        foreach (var unit in queueImages)
        {
            if(unit.Key.mConditions.isPicked)
            {
                key = unit.Value;
                break;
            }
        }
    }

    public void DequeueOrder(Unit unit)
    {
        GameObject obj;
        queueImages.TryGetValue(unit, out obj);
        if (queueImages.ContainsKey(unit) && unit.mConditions.isDied)
        {
            Destroy(obj);
            queueImages.Remove(unit);
        }
        else if (queueImages.ContainsKey(unit))
        {
            Destroy(obj);
            queueImages.Remove(unit);
        }
    }

    public void Clear()
    {
        foreach(var unit in queueImages)
        {
            Destroy(unit.Value);
        }    
        queueImages.Clear();
    }
}
