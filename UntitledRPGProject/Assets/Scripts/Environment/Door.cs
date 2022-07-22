using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableEnvironment, ILockable
{
    private bool isLock = false;
    private bool isOpen = false;
    private MeshRenderer mRenderer;
    private Animator mDoorAnimator;
    private string mOpenDirection = string.Empty;
    public GameObject mGate;

    public GateDetector[] mDetectors = new GateDetector[2];

    private void Start()
    {
        Transform doorObject = transform.Find("DoorObject");
        for (int i = 0; i < mDetectors.Length; ++i)
        {
            mDetectors[i].Initialize(this);
        }
        mDoorAnimator = doorObject.GetComponent<Animator>();
        doorObject.GetComponent<MeshRenderer>().sortingOrder = 20;
        mRenderer = mGate.GetComponent<MeshRenderer>();
        mRenderer.sortingOrder = 20;
        mOpenDirection = "IsFrontOpen";
    }

    public override void Initialize(int id)
    {
        base.Initialize(id);
        Canvas_Initialize();
        //isLock = (UnityEngine.Random.Range(0, 100) <= 40) ? true : false;
    }

    public void SetDirection(string str)
    {
        mOpenDirection = str;
    }

    public override IEnumerator Interact(Action action = null)
    {
        action?.Invoke();
        if(isOpen)
        {
            yield return null;
        }
        else
        {
            Key key = PlayerController.Instance.mInventory.Get("Key") as Key;
            foreach (Dialogue dialogue in mDialogue)
            {
                m_DialogueQueue.Enqueue(dialogue);
            }
            if (IsLocked())
            {
                if (key != null)
                {
                    key.End();
                    UnLock();
                    Open();
                }
                else
                {
                    UIManager.Instance.DisplayDialogueBox(true);
                    yield return new WaitForSeconds(0.5f);
                    UIManager.Instance.DisplaySupportKey(true, false, false);
                    UIManager.Instance.ChangeSupportText(new string[3]{
            "Continue",
            string.Empty,
            string.Empty});
                    UIManager.Instance.ChangeDialogueText("Jimmy" + ": " + m_DialogueFailCase.Text);
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    UIManager.Instance.ChangeDialogueText("");
                    UIManager.Instance.DisplayDialogueBox(false);
                }
            }
            else
                Open();

        }
        yield break;
    }

    private void Open()
    {
        isOpen = true;
        _Completed = true;
        if(mOpenDirection == string.Empty)
        {
            mOpenDirection = "IsFrontOpen";
        }
        mDoorAnimator.SetBool(mOpenDirection, true);
        AudioManager.PlaySfx(mSFX);
    }
    public bool IsLocked()
    {
        return isLock;
    }

    public void UnLock()
    {
        isLock = true;
    }
    public override void Reset()
    {
        _Completed = false;
        isOpen = false;
    }

}
