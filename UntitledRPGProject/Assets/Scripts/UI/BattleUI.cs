using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField]
    private Image mAttackButton;
    [SerializeField]
    private Image mMagicButton;
    [SerializeField]
    private Image mDefendButton;
    [SerializeField]
    private GameObject mHereIcon;
    private Action[] Actions = new Action[3];
    private int currentIndex = 0;

    private void Start()
    {
        mAttackButton = (mAttackButton == null) ? transform.Find("Attack").GetComponent<Image>() : mAttackButton;
        mMagicButton = (mMagicButton == null) ? transform.Find("Magic").GetComponent<Image>() : mMagicButton;
        mDefendButton = (mDefendButton == null) ? transform.Find("Defend").GetComponent<Image>() : mDefendButton;

        mAttackButton.color = new Color(1, 1, 1);
        mMagicButton.color = new Color(1, 1, 1);
        mDefendButton.color = new Color(1, 1, 1);
        mHereIcon.transform.SetParent(mAttackButton.gameObject.transform);
        currentIndex = 0;
    }

    private void Update()
    {
        if(gameObject.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                }
            }
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentIndex < 2)
                {
                    currentIndex++;
                }
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                Actions[currentIndex]?.Invoke();
            }

            switch (currentIndex)
            {
                case 0:
                    {
                        mAttackButton.color = Color.Lerp(mAttackButton.color, new Color(0, 1, 0), Time.deltaTime * 2.0f);
                        mMagicButton.color = Color.Lerp(mMagicButton.color, new Color(1, 1, 1), Time.deltaTime * 2.0f);
                        mDefendButton.color = Color.Lerp(mDefendButton.color, new Color(1, 1, 1), Time.deltaTime * 2.0f);
                        mHereIcon.transform.SetParent(mAttackButton.gameObject.transform);
                        mHereIcon.transform.position = mAttackButton.transform.position + new Vector3(15,0,0);
                    }
                    break;
                case 1:
                    {
                        mAttackButton.color = Color.Lerp(mAttackButton.color, new Color(1, 1, 1), Time.deltaTime * 2.0f);
                        mMagicButton.color = Color.Lerp(mMagicButton.color, new Color(0, 1, 0), Time.deltaTime * 2.0f);
                        mDefendButton.color = Color.Lerp(mDefendButton.color, new Color(1, 1, 1), Time.deltaTime * 2.0f);
                        mHereIcon.transform.SetParent(mMagicButton.gameObject.transform);
                        mHereIcon.transform.position = mMagicButton.transform.position + new Vector3(15, 0, 0);
                    }
                    break;
                case 2:
                    {
                        mAttackButton.color = Color.Lerp(mAttackButton.color, new Color(1, 1, 1), Time.deltaTime * 2.0f);
                        mMagicButton.color = Color.Lerp(mMagicButton.color, new Color(1, 1, 1), Time.deltaTime * 2.0f);
                        mDefendButton.color = Color.Lerp(mDefendButton.color, new Color(0, 1, 0), Time.deltaTime * 2.0f);
                        mHereIcon.transform.SetParent(mDefendButton.gameObject.transform);
                        mHereIcon.transform.position = mDefendButton.transform.position + new Vector3(15, 0, 0);
                    }
                    break;
            }

        }
    }

    private void OnEnable()
    {
        if (Actions[0] == null)
            Actions[0] += BattleManager.Instance.Attack;
        if(Actions[1] == null)
            Actions[1] += BattleManager.Instance.Magic;
        if(Actions[2] == null)
            Actions[2] += BattleManager.Instance.Defend;
        currentIndex = 0;
        mHereIcon.transform.SetParent(mAttackButton.gameObject.transform);
    }
    private void OnDisable()
    {
        if(Actions[0] != null)
            Actions[0] -= BattleManager.Instance.Attack;
        if(Actions[1] != null)
            Actions[1] -= BattleManager.Instance.Magic;
        if(Actions[2] != null)
            Actions[2] -= BattleManager.Instance.Defend;
    }
}
