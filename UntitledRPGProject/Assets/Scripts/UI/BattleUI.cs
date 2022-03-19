using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField]
    private Button mAttackButton;
    [SerializeField]
    private Button mMagicButton;
    [SerializeField]
    private Button mDefendButton;

    private void OnEnable()
    {
        if(mAttackButton != null)
            mAttackButton.onClick.AddListener(BattleManager.Instance.Attack);
        if(mDefendButton != null)
            mDefendButton.onClick.AddListener(BattleManager.Instance.Defend);
    }

    private void OnDisable()
    {
        if (mAttackButton != null)
            mAttackButton.onClick.RemoveListener(BattleManager.Instance.Attack);
        if (mDefendButton != null)
            mDefendButton.onClick.RemoveListener(BattleManager.Instance.Defend);
    }
}
