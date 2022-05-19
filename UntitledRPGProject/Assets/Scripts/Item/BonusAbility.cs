using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class BonusAbility
{
    public enum AbilityType
    {
        Health,
        Damage,
        MagicPower,
        MagicResistance,
        Agility,
        Armor,
        Magic,
        Mana
    }

    public AbilityType Type;
    public float Value;
    public Skill_Setting Skill;
}

[CustomPropertyDrawer(typeof(BonusAbility))]
public class MyActionPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var effectRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var secondRect = new Rect(position.x, position.y + 20f, position.width, EditorGUIUtility.singleLineHeight);

        var type = property.FindPropertyRelative("Type");
        var skill = property.FindPropertyRelative("Skill");
        var value = property.FindPropertyRelative("Value");
        type.intValue = EditorGUI.Popup(effectRect, "Type", type.intValue, type.enumNames);

        switch ((BonusAbility.AbilityType)type.intValue)
        {
            case BonusAbility.AbilityType.Agility:
                value.floatValue = EditorGUI.FloatField(secondRect, "Value", value.floatValue);
                break;
            case BonusAbility.AbilityType.Damage:
                value.floatValue = EditorGUI.FloatField(secondRect, "Value", value.floatValue);
                break;
            case BonusAbility.AbilityType.Mana:
                value.floatValue = EditorGUI.FloatField(secondRect, "Value", value.floatValue);
                break;
            case BonusAbility.AbilityType.Health:
                value.floatValue = EditorGUI.FloatField(secondRect, "Value", value.floatValue);
                break;
            case BonusAbility.AbilityType.MagicResistance:
                value.floatValue = EditorGUI.FloatField(secondRect, "Value", value.floatValue);
                break;
            case BonusAbility.AbilityType.Armor:
                value.floatValue = EditorGUI.FloatField(secondRect, "Value", value.floatValue);
                break;
            case BonusAbility.AbilityType.MagicPower:
                value.floatValue = EditorGUI.FloatField(secondRect, "Value", value.floatValue);
                break;
            case BonusAbility.AbilityType.Magic:
                EditorGUI.ObjectField(secondRect,skill);
                break;
            default:
                break;
        }

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    //This will need to be adjusted based on what you are displaying
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (20 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 2);
    }
}