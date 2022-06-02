using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : Item
{
    public bool IsEquipped { get; set; } = false;
    [HideInInspector]
    public List<BonusAbility> Bonus = new List<BonusAbility>();
    public override void Initialize(int id)
    {
        base.Initialize(id);
        var obj = (EquipmentInfo)Info;
        IsEquipped = false;
        Bonus.Clear();
        foreach(BonusAbility bonus in obj.mBonusAbilities)
            Bonus.Add(bonus);
    }

    public virtual void ChangeOwner(Unit gameObject)
    {
        IsEquipped = false;
        mOwner = gameObject;
    }

    public override void Apply()
    {
        if (!IsEquipped)
        {
            IsEquipped = true;
            foreach (var info in Bonus)
            {
                switch (info.Type)
                {
                    case BonusAbility.AbilityType.Health:
                        mOwner.mBonusStatus.mHealth += info.Value;
                        break;
                    case BonusAbility.AbilityType.MagicResistance:
                        mOwner.mBonusStatus.mMagic_Resistance += info.Value;
                        break;
                    case BonusAbility.AbilityType.Magic:
                        break;
                    case BonusAbility.AbilityType.Agility:
                        mOwner.mBonusStatus.mAgility += info.Value;
                        break;
                    case BonusAbility.AbilityType.MagicPower:
                        mOwner.mBonusStatus.mMagicPower += info.Value;
                        break;
                    case BonusAbility.AbilityType.Damage:
                        mOwner.mBonusStatus.mDamage += info.Value;
                        break;
                    case BonusAbility.AbilityType.Armor:
                        mOwner.mBonusStatus.mArmor += info.Value;
                        break;
                    case BonusAbility.AbilityType.Mana:
                        mOwner.mBonusStatus.mMana += info.Value;
                        break;
                }
            }
        }
    }

    public override void End()
    {
        if (IsEquipped)
        {
            IsEquipped = false;
            foreach (var info in Bonus)
            {
                switch (info.Type)
                {
                    case BonusAbility.AbilityType.Health:
                        mOwner.mBonusStatus.mHealth -= info.Value;
                        break;
                    case BonusAbility.AbilityType.MagicResistance:
                        mOwner.mBonusStatus.mMagic_Resistance -= info.Value;
                        break;
                    case BonusAbility.AbilityType.Magic:
                        break;
                    case BonusAbility.AbilityType.Agility:
                        mOwner.mBonusStatus.mAgility -= info.Value;
                        break;
                    case BonusAbility.AbilityType.MagicPower:
                        mOwner.mBonusStatus.mMagicPower -= info.Value;
                        break;
                    case BonusAbility.AbilityType.Damage:
                        mOwner.mBonusStatus.mDamage -= info.Value;
                        break;
                    case BonusAbility.AbilityType.Armor:
                        mOwner.mBonusStatus.mArmor -= info.Value;
                        break;
                    case BonusAbility.AbilityType.Mana:
                        mOwner.mBonusStatus.mMana -= info.Value;
                        break;
                }
            }
        }
        Bonus.Clear();
        ChangeOwner(null);
    }
}
