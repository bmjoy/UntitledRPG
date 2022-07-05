using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : NPC
{
    SlotManager mSlotManager;
    bool isBuy = false;
    [SerializeField]
    private Dialogue m_DialogueBuyCase;
    [SerializeField]
    private Dialogue m_DialogueSellCase;
    [SerializeField]
    private Dialogue m_DialogueAskAgainCase;

    protected override void Start()
    {
        base.Start();
        mSlotManager = GetComponent<SlotManager>();
        Destroy(mInteraction.GetComponent<Billboard>());
        m_DialogueAskAgainCase.Trigger = Dialogue.TriggerType.Event;
    }

    protected override void Update()
    {
        base.Update();
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
        UIManager.Instance.DisplayEKeyInDialogue(false);
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
        UIManager.Instance.AddListenerBackButton(() => {
            m_DialogueQueue.Enqueue(m_DialogueAskAgainCase);
            mComplete = true;
        });
        UIManager.Instance.DisplayExitButtonInDialogue(true);
        UIManager.Instance.DisplayBackButtonInDialogue(true);
        if (isBuy)
            mSlotManager.StartBuyTrade();
        else
            mSlotManager.StartSellTrade();
        yield return new WaitUntil(() => mComplete);
        mSlotManager.EndTrade();
        UIManager.Instance.DisplayExitButtonInDialogue(false);
        UIManager.Instance.DisplayBackButtonInDialogue(false);
    }
}
