using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private GameObject mBonusValuesGroup;
    private GameObject mTextAreaGroup;
    private GameObject mButtonGroup;
    private GameObject mEquipmentImageGroup;
    private GameObject mItemsGroup;
    private GameObject mMoneyGroup;
    public GameObject mPickIcon;

    private Image mCurrentUnit;
    private Animator mCurrentUnitAnimator;

    public Sprite mCharacterEmptyImage;
    public Sprite mEquipmentEmptyImage;
    public Sprite mCenterCharacterEmptyImage;
    private bool mInitialized = false;
    //private bool mTransfer = false;
    public int mIndex = 0;

    //private int mSelectedItemIndex = 0;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        mBonusValuesGroup = transform.Find("Bonus").gameObject;
        mTextAreaGroup = transform.Find("TextArea").gameObject;
        mButtonGroup = transform.Find("Button").gameObject;
        mEquipmentImageGroup = transform.Find("Equipment").gameObject;
        mMoneyGroup = transform.Find("Money").gameObject;
        mCurrentUnit = transform.Find("CurrentUnit").GetComponent<Image>();
        mItemsGroup = transform.Find("ItemScroll").transform.Find("Items").gameObject;
        //mSelectedItemIndex = 0;
        mPickIcon.SetActive(false);
        mInitialized = true;
    }

    private void Update()
    {
        //if (items.Count > 0)
        //{
        //    if (Input.GetKeyDown(KeyCode.LeftArrow) && mSelectedItemIndex > 0)
        //    {
        //        mSelectedItemIndex--;
        //    }
        //    if (Input.GetKeyDown(KeyCode.RightArrow) && mSelectedItemIndex < items.Count - 1)
        //    {
        //        mSelectedItemIndex++;
        //    }
        //    if (Input.GetKeyDown(UIManager.Instance.mYesKeyCode))
        //    {
        //        if (items[mSelectedItemIndex].GetComponent<ItemUI>().mType == ItemUI.ItemUIType.Equip)
        //        {
        //            items[mSelectedItemIndex].GetComponent<ItemUI>().Activate();
        //            items.Remove(items[mSelectedItemIndex]);
        //        }
        //        mPickIcon.transform.SetParent(items[mSelectedItemIndex].transform);
        //        mPickIcon.transform.position = items[mSelectedItemIndex].transform.position + new Vector3(0, -1, 0);
        //    }
        //}
        //else
        //{
        //    mPickIcon.transform.SetParent(transform);
        //    mPickIcon.transform.position = transform.position;
        //}
    }

    private void LateUpdate()
    {
        if(mInitialized && gameObject.activeInHierarchy)
        {
            PrintStatus(mIndex);
            PrintStatusBonus(mIndex);
        }

    }

    public void Clear()
    {
        foreach(Transform transform in mTextAreaGroup.transform)
            transform.GetComponent<TextMeshProUGUI>().text = default;
        foreach (Transform transform in mBonusValuesGroup.transform)
            transform.GetComponent<TextMeshProUGUI>().text = default;
        foreach(Transform transform in mEquipmentImageGroup.transform)
            transform.GetComponent<Image>().sprite = mEquipmentEmptyImage;
        foreach(Transform transform in mButtonGroup.transform)
            transform.GetComponent<Button>().image.sprite = mCharacterEmptyImage;
        foreach (var item in items)
            Destroy(item.gameObject);
        items.Clear();
        foreach (Transform item in mItemsGroup.transform)
            Destroy(item.gameObject);
    }

    public void Active(bool active)
    {
        if (mInitialized == false) return;
        mBonusValuesGroup.SetActive(active);
        mTextAreaGroup.SetActive(active);
        mButtonGroup.SetActive(active);
        mEquipmentImageGroup.SetActive(active);
        mIndex = 0;
        if (active == true)
        {
            transform.gameObject.SetActive(true);
            Display(0);
            _Initialized = false;
        }
        else
        {
            if(transform.gameObject.activeSelf)
                StartCoroutine(WaitToClose());
        }    
    }
    private IEnumerator WaitToClose()
    {
        //mPickIcon.transform.SetParent(transform);
        //mPickIcon.transform.position = transform.position;
        GetComponent<Animator>().SetTrigger("Outro");
        yield return new WaitForSeconds(1.0f);
        Clear();
        transform.gameObject.SetActive(false);
    }

    public void Display(int num)
    {
        if(PlayerController.Instance.mHeroes.Count <= num) return;

        mIndex = num;
        PrintStatus(num);
        PrintStatusBonus(num);
        ChangeHeroSprite(num);
        InventorySetup();
    }

    private void ChangeHeroSprite(int num)
    {
        if (num >= PlayerController.Instance.mHeroes.Count) return;

        var unit = PlayerController.Instance.mHeroes[num].GetComponent<Player>();
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/UI/" + unit.mSetting.Name + "_UI"), transform.position, Quaternion.identity);
        mCurrentUnit = transform.Find("CurrentUnit").GetComponent<Image>();
        mCurrentUnit.sprite = mCenterCharacterEmptyImage;
        mCurrentUnitAnimator = transform.Find("CurrentUnit").GetComponent<Animator>();
        mCurrentUnitAnimator.runtimeAnimatorController = null;
        mCurrentUnit.sprite = go.GetComponent<Image>().sprite;
        mCurrentUnitAnimator.runtimeAnimatorController = go.transform.GetComponent<Animator>().runtimeAnimatorController;
        Destroy(go);
        InventoryUpdate();
    }

    private void PrintStatus(int num)
    {
        if (num >= PlayerController.Instance.mHeroes.Count) return;
        var unit = PlayerController.Instance.mHeroes[num].GetComponent<Player>();

        mTextAreaGroup.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = unit.mSetting.Name;
        mTextAreaGroup.transform.Find("Level").GetComponent<TextMeshProUGUI>().text = unit.mStatus.mLevel.ToString();
        mTextAreaGroup.transform.Find("Health").GetComponent<TextMeshProUGUI>().text = unit.mStatus.mHealth.ToString() + "/" + unit.mStatus.mMaxHealth.ToString();
        mTextAreaGroup.transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = unit.mStatus.mMana.ToString() + "/" + unit.mStatus.mMaxMana.ToString();
        mTextAreaGroup.transform.Find("Damage").GetComponent<TextMeshProUGUI>().text = unit.mStatus.mDamage.ToString();
        mTextAreaGroup.transform.Find("Armor").GetComponent<TextMeshProUGUI>().text = unit.mStatus.mArmor.ToString();
        mTextAreaGroup.transform.Find("MagicPower").GetComponent<TextMeshProUGUI>().text = unit.mStatus.mMagicPower.ToString();
        mTextAreaGroup.transform.Find("MagicResistance").GetComponent<TextMeshProUGUI>().text = unit.mStatus.mMagic_Resistance.ToString();
        mTextAreaGroup.transform.Find("Agility").GetComponent<TextMeshProUGUI>().text = unit.mStatus.mAgility.ToString();
    }  
    private void PrintStatusBonus(int num)
    {
        if (num >= PlayerController.Instance.mHeroes.Count) return;
        var unit = PlayerController.Instance.mHeroes[num].GetComponent<Player>();

        mBonusValuesGroup.transform.Find("Health").GetComponent<TextMeshProUGUI>().text = "+" + unit.mBonusStatus.mHealth.ToString();
        mBonusValuesGroup.transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = "+" + unit.mBonusStatus.mMana.ToString();
        mBonusValuesGroup.transform.Find("Damage").GetComponent<TextMeshProUGUI>().text = "+" + unit.mBonusStatus.mDamage.ToString();
        mBonusValuesGroup.transform.Find("Armor").GetComponent<TextMeshProUGUI>().text = "+" + unit.mBonusStatus.mArmor.ToString();
        mBonusValuesGroup.transform.Find("MagicPower").GetComponent<TextMeshProUGUI>().text = "+" + unit.mBonusStatus.mMagicPower.ToString();
        mBonusValuesGroup.transform.Find("MagicResistance").GetComponent<TextMeshProUGUI>().text = "+" + unit.mBonusStatus.mMagic_Resistance.ToString();
        mBonusValuesGroup.transform.Find("Agility").GetComponent<TextMeshProUGUI>().text = "+" + unit.mBonusStatus.mAgility.ToString();

        mEquipmentImageGroup.transform.Find("Weapon").GetComponent<Image>().sprite = (unit.GetComponent<InventroySystem>().mInventoryInfo.Weapon) ?
            unit.GetComponent<InventroySystem>().mInventoryInfo.Weapon.Info.mSprite : mEquipmentEmptyImage;
        mEquipmentImageGroup.transform.Find("Body").GetComponent<Image>().sprite = (unit.GetComponent<InventroySystem>().mInventoryInfo.Body) ?
            unit.GetComponent<InventroySystem>().mInventoryInfo.Body.Info.mSprite : mEquipmentEmptyImage;
        mEquipmentImageGroup.transform.Find("Leg").GetComponent<Image>().sprite = (unit.GetComponent<InventroySystem>().mInventoryInfo.Leg) ?
    unit.GetComponent<InventroySystem>().mInventoryInfo.Leg.Info.mSprite : mEquipmentEmptyImage;
        mEquipmentImageGroup.transform.Find("Head").GetComponent<Image>().sprite = (unit.GetComponent<InventroySystem>().mInventoryInfo.Head) ?
    unit.GetComponent<InventroySystem>().mInventoryInfo.Head.Info.mSprite : mEquipmentEmptyImage;
        mEquipmentImageGroup.transform.Find("Arm").GetComponent<Image>().sprite = (unit.GetComponent<InventroySystem>().mInventoryInfo.Arm) ?
    unit.GetComponent<InventroySystem>().mInventoryInfo.Arm.Info.mSprite : mEquipmentEmptyImage;
    }

    bool _Initialized = false;
    List<GameObject> items = new List<GameObject>();
    private void InventorySetup()
    {
        if (_Initialized) return;
        items.Clear();
        PlayerController.Instance.mInventory.myInventory.Sort((a, b) => a.Value.Value.CompareTo(b.Value.Value));
        foreach (var item in PlayerController.Instance.mInventory.myInventory)
        {
            if (item.Value.GetType().IsSubclassOf(typeof(Expendables)))
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Item"), mItemsGroup.transform.position, Quaternion.identity, mItemsGroup.transform);
                go.GetComponent<ItemUI>().Initialize(item.Key, item.Value, ItemUI.ItemUIType.Non_Activate);
                items.Add(go);
            }
            else
            {
                EquipmentItem equipment = (EquipmentItem)item.Value;
                if (equipment.IsEquipped) continue;
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Item"), mItemsGroup.transform.position, Quaternion.identity, mItemsGroup.transform);
                go.GetComponent<ItemUI>().Initialize(item.Key, item.Value, ItemUI.ItemUIType.Equip);
                items.Add(go);
                string part = string.Empty;
                if (item.Value is Weapon) part = "Weapon";
                else if (item.Value is Armor)
                {
                    var i = (Armor)item.Value;
                    switch (i.armorType)
                    {
                        case ArmorType.Bracer: part = "Arm"; break;
                        case ArmorType.BodyArmor: part = "Body"; break;
                        case ArmorType.LegArmor: part = "Leg"; break;
                        case ArmorType.Helmet: part = "Head"; break;
                    }
                }
                else continue;
                if (part != string.Empty)
                    mEquipmentImageGroup.transform.Find(part).GetComponent<ItemUI>().Initialize(item.Key, item.Value, ItemUI.ItemUIType.Unequip);
            }

        }
        _Initialized = true;
    }

    public void InventoryUpdate()
    {
        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }
        items.Clear();
        foreach (Transform item in mItemsGroup.transform)
        {
            Destroy(item.gameObject);
        }
        _Initialized = false;
        InventorySetup();
        mMoneyGroup.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = PlayerController.Instance.mGold.ToString();
        //mSelectedItemIndex = 0;
    }


    private void OnEnable()
    {
        if (mInitialized == false)
            return;
        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
            mButtonGroup.transform.GetChild(i).GetComponent<Button>().image.sprite = PlayerController.Instance.mHeroes[i].GetComponent<Unit>().mSetting.BasicSprite;
        //mSelectedItemIndex = 0;
    }

    private void OnDisable()
    {
        mIndex = 0;
        //mSelectedItemIndex = 0;
        if (mItemsGroup == null)
            return;
        foreach (Transform obj in mItemsGroup.transform)
        {
            Destroy(obj.gameObject);
        }
    }
}
