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
            go.transform.localPosition = new Vector3(0.0f + partialWidth * i, Ypos);
            go.transform.localScale = new Vector3(mSpriteLocalScale, mSpriteLocalScale, mSpriteLocalScale);
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
            foreach(var unit in queueImages)
            {
                if (unit.Value == key)
                {
                    key.transform.localPosition += new Vector3(-mMaximumSpeed * unit.Key.mStatus.mAgility * Time.deltaTime, 0.0f, 0.0f);
                    continue;
                }
                if(unit.Value.transform.localPosition.x > -halfWidth + partialWidth)
                    unit.Value.transform.localPosition += new Vector3(-mSpeed * unit.Key.mStatus.mAgility * Time.deltaTime, 0.0f, 0.0f);
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
