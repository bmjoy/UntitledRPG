public struct BonusStatus
{
    public float mHealth;
    public float mMana;
    public float mDamage;
    public float mArmor;
    public float mMagic_Resistance;
    public float mDefend;
    public float mAgility;
    public float mMagicPower;

    public BonusStatus(bool initialize = true)
    {
        mHealth = mMana =
            mDamage = mArmor =
            mMagic_Resistance = mMagicPower =
            mDefend = mAgility = 0.0f;
    }

}