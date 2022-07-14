using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monk : NPC
{
    protected override void Start()
    {
        base.Start();
        Destroy(mInteraction.GetComponent<Billboard>());
        isTrading = false;
    }

    protected override void Update()
    {
        base.Update();
        if((isTrading && mComplete == false) && Input.GetKeyDown(KeyCode.Escape))
        {
            mComplete = true;
        }
    }

    public override IEnumerator Interact(Action Callback)
    {
        mAnimator.SetBool("Talk", true);
        yield return base.Interact(Callback);
        mAnimator.SetBool("Talk", false);
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
            foreach (var dialogue in m_DialogueYesCase)
                m_DialogueQueue.Enqueue(dialogue);
            isTrading = true;
            mComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        UIManager.Instance.DisplayEKeyInDialogue(false);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }

    public override IEnumerator Trade()
    {
        mTrigger = null;
        UIManager.Instance.AddListenerExitButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });

        UIManager.Instance.DisplaySkillTreeScreen(true);

        UIManager.Instance.DisplayExitButtonInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayExitButtonInDialogue(false);
        UIManager.Instance.DisplaySkillTreeScreen(false);
        isTrading = false;
    }
}
