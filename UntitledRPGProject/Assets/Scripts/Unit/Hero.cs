using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : NPC
{
    protected override void Start()
    {
        base.Start();
        mProperty = ((Companion)mProperty != null) ? (Companion)mProperty
    : Resources.Load<Companion>("Prefabs/Items/Companions/" + mName);
        mCanvas.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        mProperty.Initialize(-1);
        Companion companion = (Companion)mProperty;
        companion.mTransform = transform;
    }

    public override IEnumerator Interact(Action Callback)
    {
        yield return StartCoroutine(base.Interact(Callback));
        yield return new WaitForSeconds(0.25f);
        if (isTrading) 
        {
            mProperty.End();
            Destroy(gameObject, 0.2f); 
        }
    }

    public override IEnumerator Event()
    {
        UIManager.Instance.DisplaySupportKey(true, true, false);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Yes",
            "No",
            string.Empty});
        mTrigger = null;
        UIManager.Instance.DisplayMoneyBoxInDialogue(true, mProperty.Value);
        PlayerController.Instance.GetComponent<InteractSystem>().mRightAction += (() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        PlayerController.Instance.GetComponent<InteractSystem>().mLeftAction += (() => {
            if(PlayerController.Instance.mHeroes.Count >= 4)
            {
                m_DialogueQueue.Enqueue(new Dialogue("Hmm. It seems your party is full now.", Dialogue.TriggerType.None));
                mComplete = true;
            }
            else
            {
                if (mProperty.Value <= PlayerController.Instance.mGold)
                {
                    PlayerController.Instance.mGold -= mProperty.Value;
                    isTrading = true;
                }
                else
                    isTrading = false;

                if (isTrading)
                {
                    foreach (var dialogue in m_DialogueYesCase)
                        m_DialogueQueue.Enqueue(dialogue);
                    mProperty.Apply();
                }
                else
                    foreach (var dialogue in m_DialogueFailToTradeCase)
                        m_DialogueQueue.Enqueue(dialogue);
                mComplete = true;
            }

        });
        PlayerController.Instance.GetComponent<InteractSystem>().mExitAction += (() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayMoneyBoxInDialogue(false);
        PlayerController.Instance.GetComponent<InteractSystem>().ResetActions();
        UIManager.Instance.DisplaySupportKey();
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty,
            string.Empty});
    }
}
