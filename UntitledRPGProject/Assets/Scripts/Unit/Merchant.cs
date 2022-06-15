using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : NPC
{
    SlotManager mSlotManager;
    bool isBuy = false;
    [SerializeField]
    private Dialogue m_DialogueBuyCase = new Dialogue();
    [SerializeField]
    private Dialogue m_DialogueSellCase = new Dialogue();

    protected override void Start()
    {
        base.Start();
        mSlotManager = GetComponent<SlotManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            mComplete = true;
    }

    public override IEnumerator Event()
    {
        mTrigger = null;
        UIManager.Instance.ChangeTwoButtons(UIManager.Instance.mStorage.BuyButtonImage,
            UIManager.Instance.mStorage.SellButtonImage);
        UIManager.Instance.AddListenerRightButton(() => {
            m_DialogueQueue.Enqueue(m_DialogueSellCase);
            isBuy = false;
            mComplete = true;
        });
        UIManager.Instance.AddListenerLeftButton(() => {
            m_DialogueQueue.Enqueue(m_DialogueBuyCase);
            isBuy = true;
            mComplete = true;
        });
        UIManager.Instance.AddListenerExitButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        UIManager.Instance.DisplayExitButtonInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
        UIManager.Instance.DisplayExitButtonInDialogue(false);
    }

    public override IEnumerator Trade()
    {
        UIManager.Instance.AddListenerExitButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.DisplayExitButtonInDialogue(true);
        if (isBuy)
        {
            mSlotManager.StartBuyTrade();
        }
        else
        {
            mSlotManager.StartSellTrade();
        }
        yield return new WaitUntil(() => mComplete);
        mSlotManager.EndTrade();
        UIManager.Instance.DisplayExitButtonInDialogue(false);
    }
}
