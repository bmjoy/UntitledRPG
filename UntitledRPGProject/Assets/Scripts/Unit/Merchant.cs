using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        m_DialogueAskAgainCase.Trigger = Dialogue.TriggerType.Event;
    }

    public override IEnumerator Event()
    {
        mTrigger = null;
        UIManager.Instance.DisplaySupportKey(true, true, true);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Buy",
            "Sell",
            "Exit"});
        PlayerController.Instance.GetComponent<InteractSystem>().mRightAction += (() => {
            m_DialogueQueue.Enqueue(m_DialogueSellCase);
            isBuy = false;
            UIManager.Instance.DisplaySupportKey();
            UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty,
            string.Empty});
            mComplete = true;
        });
        PlayerController.Instance.GetComponent<InteractSystem>().mLeftAction += (() => {
            m_DialogueQueue.Enqueue(m_DialogueBuyCase);
            isBuy = true;
            mComplete = true;
        });
        PlayerController.Instance.GetComponent<InteractSystem>().mExitAction += (() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            UIManager.Instance.DisplaySupportKey();
            UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty,
            string.Empty});
            mComplete = true;
        });
        yield return new WaitUntil(() => mComplete);
        PlayerController.Instance.GetComponent<InteractSystem>().ResetActions();
    }

    public override IEnumerator Trade()
    {
        PlayerController.Instance.GetComponent<InteractSystem>().mExitAction += (() => {
            m_DialogueQueue.Enqueue(m_DialogueAskAgainCase);
            mComplete = true;
        });
        if (isBuy)
        {
            UIManager.Instance.DisplaySupportKey(true,false ,true);
            UIManager.Instance.ChangeSupportText(new string[3]{
            "Purchase",
            string.Empty,
            "Back"});
            mSlotManager.StartBuyTrade();
        }
        else
        {
            UIManager.Instance.DisplaySupportKey(true, true, true);
            UIManager.Instance.ChangeSupportText(new string[3]{
            "Sell/Buy",
            "Transfer",
            "Back"});
            mSlotManager.StartSellTrade();
        }
        yield return new WaitUntil(() => mComplete);
        mSlotManager.EndTrade();
        PlayerController.Instance.GetComponent<InteractSystem>().ResetActions();
        UIManager.Instance.DisplaySupportKey();
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty,
            string.Empty});
    }
}
