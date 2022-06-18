using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : NPC
{
    [SerializeField]
    List<NeedsInfo> m_NeedsList = new List<NeedsInfo>();

    protected override void Start()
    {
        base.Start();
        mProperty = ((Companion)mProperty != null) ? (Companion)mProperty
    : Resources.Load<Companion>("Prefabs/Items/Companions/" + mName);

        mProperty.Initialize(-1);
        Companion companion = (Companion)mProperty;
        companion.mTransform = transform;
    }

    public override IEnumerator Interact(Action Callback)
    {
        yield return StartCoroutine(base.Interact(Callback));
        yield return new WaitForSeconds(0.25f);
        if (isTrading)
            Destroy(gameObject, 0.5f);
        StopAllCoroutines();
    }

    public override IEnumerator Event()
    {
        mTrigger = null;
        UIManager.Instance.ChangeTwoButtons(UIManager.Instance.mStorage.YesButtonImage,
UIManager.Instance.mStorage.NoButtonImage);
        UIManager.Instance.AddListenerRightButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.AddListenerLeftButton(() => {
            if(PlayerController.Instance.mHeroes.Count >= 4)
            {
                Dialogue dialogue = new Dialogue();
                dialogue.Trigger = Dialogue.TriggerType.None;
                dialogue.Text = "Hmm. It seems your party is full now.";
                m_DialogueQueue.Enqueue(dialogue);
                mComplete = true;
            }
            else
            {
                foreach (var val in m_NeedsList)
                {
                    switch (val.Name)
                    {
                        case "Money":
                            {
                                if (val.Value <= PlayerController.Instance.mGold)
                                {
                                    PlayerController.Instance.mGold -= val.Value;
                                    val.onComplete = true;
                                }
                            }
                            break;
                        case "Item":
                            //TODO: Item quest
                            break;
                        default:
                            break;
                    }
                }

                isTrading = m_NeedsList.TrueForAll(t => t.onComplete == true);

                if (isTrading)
                {
                    foreach (var dialogue in m_DialogueYesCase)
                        m_DialogueQueue.Enqueue(dialogue);
                    mProperty.Apply();
                }
                else
                {
                    foreach (var dialogue in m_DialogueFailToTradeCase)
                        m_DialogueQueue.Enqueue(dialogue);
                }
                mComplete = true;
            }

        });
        UIManager.Instance.DisplayEKeyInDialogue(false);
        UIManager.Instance.DisplayButtonsInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }
}
