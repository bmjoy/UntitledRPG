using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monk : NPC
{
    protected override void Start()
    {
        base.Start();
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
        UIManager.Instance.DisplaySupportKey(true, true, false);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Yes",
            "No",
            string.Empty});
        PlayerController.Instance.GetComponent<InteractSystem>().mRightAction += (() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            UIManager.Instance.DisplaySupportKey();
            UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty, string.Empty});
            mComplete = true;
        });
        PlayerController.Instance.GetComponent<InteractSystem>().mLeftAction += (() => {
            foreach (var dialogue in m_DialogueYesCase)
                m_DialogueQueue.Enqueue(dialogue);
            isTrading = true;
            mComplete = true;
        });
        yield return new WaitUntil(() => mComplete);
        PlayerController.Instance.GetComponent<InteractSystem>().ResetActions();
    }

    public override IEnumerator Trade()
    {
        UIManager.Instance.DisplaySupportKey(true,false ,true);
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Learn",
            string.Empty,
            "Exit"});
        mTrigger = null;
        PlayerController.Instance.GetComponent<InteractSystem>().mExitAction += (() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });

        UIManager.Instance.DisplaySkillTreeScreen(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplaySkillTreeScreen(false);
        isTrading = false;
        PlayerController.Instance.GetComponent<InteractSystem>().ResetActions();
        UIManager.Instance.DisplaySupportKey();
        UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty,
            string.Empty});
    }
}
