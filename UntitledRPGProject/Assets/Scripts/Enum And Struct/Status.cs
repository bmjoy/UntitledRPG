public struct Status
{
    public int mLevel;
    public int mEXP;
    public int mGold;
    public float mMaxHealth;
    public float mHealth;
    public float mMana;
    public float mMaxMana;
    public float mDamage;
    public float mArmor;
    public float mMagic_Resistance;
    public float mDefend;
    public float mAgility;
    public float mMagicPower;
    public WeaponType mWeaponType;
    public Status(int level, int exp, int gold, float maxHp, float hp, float mana, float maxMp, float dmg, float am, float mr, float de, float ag, float mp, WeaponType wp = WeaponType.None)
    {
        mLevel = level;
        mEXP = exp;
        mGold = gold;
        mMaxHealth = maxHp;
        mHealth = hp;
        mMana = mana;
        mMaxMana = maxMp;
        mDamage = dmg;
        mArmor = am;
        mMagic_Resistance = mr;
        mDefend = de;
        mAgility = ag;
        mMagicPower = mp;
        mWeaponType = wp;
    }

    public Status(Unit_Setting setting)
    {
        mLevel = setting.Level;
        mEXP = setting.EXP;
        mGold = setting.Gold;
        mMaxHealth = setting.MaxHealth;
        mHealth = setting.MaxHealth;
        mMana = setting.MaxMana;
        mMaxMana = setting.MaxMana;
        mDamage = setting.Attack;
        mArmor = setting.Armor;
        mMagic_Resistance = setting.Magic_Resistance;
        mDefend = setting.Defend;
        mAgility = setting.Agility;
        mMagicPower = setting.MagicPower;
        mWeaponType = setting.WeaponType;
    }
}