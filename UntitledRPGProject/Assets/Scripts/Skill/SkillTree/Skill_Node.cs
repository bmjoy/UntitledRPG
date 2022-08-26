using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_Node : MonoBehaviour
{
    public string _Name = string.Empty;
    [TextArea]
    public string _Description = string.Empty;
    public int _Cost = 0;
    public Image _Sprite;
    public Image _LineSprite;
    public Button _Button;
    public SkillTree_BounsAbility _BonusAbility;
    public List<Skill_Node> _Parents;

    private bool _isUnlocked = false;

    public void Initialize()
    {
        SkillTreeManager.Instance.skill_Nodes.Add(this);
        _Sprite = GetComponent<Image>();
        _Button = GetComponent<Button>();
        _Button.onClick.RemoveAllListeners();
        _Button.onClick.AddListener(() => UIManager.Instance.mSkillTreeScreen.Display(this));
        if (_LineSprite != null)
            _LineSprite.fillAmount = 0;
    }

    private void Update()
    {
        if(_isUnlocked)
        {
            _Sprite.color = Color.Lerp(_Sprite.color, new Color(0.0f, 1.0f, 0.0f), Time.deltaTime);
            if(_LineSprite != null)
                _LineSprite.fillAmount = Mathf.Lerp(_LineSprite.fillAmount, 1.0f, Time.deltaTime);
        }
    }

    public void Unlock()
    {
        if (_isUnlocked || PlayerController.Instance.mSoul < _Cost)
            return;
        _isUnlocked = true;
        
        if (_LineSprite != null)
            _LineSprite.color = new Color(1.0f, 1.0f, 1.0f);
        PlayerController.Instance.mSoul -= _Cost;
        AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mSKillTreeObtainedSFX);
        GameObject fireworks = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/UpgradeEffect"), transform.position, Quaternion.identity, UIManager.Instance.mAdditionalCanvas.transform);
        SkillTreeManager.Instance.mTotalBounsAbilities.Add(_BonusAbility);
        Destroy(fireworks, 3.0f);
    }

    public void Unlock_Free()
    {
        if (_isUnlocked)
            return;
        _isUnlocked = true;

        if (_LineSprite != null)
            _LineSprite.color = new Color(1.0f, 1.0f, 1.0f);
        SkillTreeManager.Instance.mTotalBounsAbilities.Add(_BonusAbility);
    }

    public bool IsUnlocked()
    {
        return _isUnlocked;
    }

    public bool IsChildrenUnlocked()
    {
        if (_Parents.Count == 0)
            return true;

        for (int i = 0; i < _Parents.Count; ++i)
            if (!_Parents[i].IsUnlocked())
                return false;
        return true;
    }

    public void ResetNode()
    {
        _isUnlocked = false;
        _Button.onClick.RemoveAllListeners();
        _Button.onClick.AddListener(() => UIManager.Instance.mSkillTreeScreen.Display(this));
        _Sprite.color = new Color(1.0f, 1.0f, 1.0f);
        if (_LineSprite != null)
            _LineSprite.fillAmount = 0;
    }

    private void OnApplicationQuit()
    {
        _Button.onClick.RemoveAllListeners();
    }
}
