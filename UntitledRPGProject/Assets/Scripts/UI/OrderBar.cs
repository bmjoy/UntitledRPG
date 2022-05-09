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

    public GameObject key = null;
    private float halfWidth = 0.0f;
    private float partialWidth = 0.0f;

    [SerializeField]
    private float Ypos = 5.0f;
    [SerializeField]
    private float mSpriteLocalScale = 0.25f;
    [SerializeField]
    private float mSpeed = 50.0f;
    [SerializeField]
    private float mMaximumSpeed = 200.0f;

    void Start()
    {
        image = transform.GetComponent<Image>();

        halfWidth = image.rectTransform.rect.width / 2.0f;
        partialWidth = image.rectTransform.rect.width / 8.0f;
        BattleManager.Instance.onEnqueuingSingleOrderEvent += EnqueueSignleOrder;
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
        foreach(Unit unit in unitList)
        {
            GameObject go = CreateObject();
            go.AddComponent<Image>().sprite = unit.mSetting.BasicSprite;
            queueImages.Add(unit, go);
        }
    }

    private void Update()
    {
        if(key != null)
        {
            foreach(var unit in queueImages)
            {
                if (unit.Value == key)
                {
                    key.transform.localPosition += new Vector3(-mMaximumSpeed * Time.deltaTime, 0.0f, 0.0f);
                    continue;
                }
                if(unit.Value.transform.localPosition.x > -halfWidth + partialWidth)
                    unit.Value.transform.localPosition += new Vector3(-mSpeed * Time.deltaTime, 0.0f, 0.0f);
            }
            if (key.transform.localPosition.x <= -halfWidth)
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

    public void EnqueueSignleOrder(Unit unit)
    {
        if(queueImages.ContainsKey(unit))
            return;

        GameObject go = CreateObject();
        go.AddComponent<Image>().sprite = unit.mSetting.BasicSprite;
        queueImages.Add(unit, go);
        key = null;
    }

    private GameObject CreateObject()
    {
        GameObject go = new GameObject("Sprite");
        go.transform.parent = transform;
        go.transform.localPosition = new Vector3(halfWidth, Ypos);
        go.transform.localScale = new Vector3(mSpriteLocalScale, mSpriteLocalScale, mSpriteLocalScale);
        go.transform.localRotation = Quaternion.Euler(0, 180, 0);
        return go;
    }

    public void DequeueOrder(Unit unit)
    {
        GameObject obj;
        queueImages.TryGetValue(unit, out obj);
        if (queueImages.ContainsKey(unit))
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
