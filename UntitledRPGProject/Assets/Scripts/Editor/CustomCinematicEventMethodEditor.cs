using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(CinematicEventMethod))]
public class CustomCinematicEventMethodEditor : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var effectRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var secondRect = new Rect(position.x, position.y + 20f, position.width, EditorGUIUtility.singleLineHeight);
        var thirdRect = new Rect(position.x, position.y + 40f, position.width, EditorGUIUtility.singleLineHeight);
        var forthRect = new Rect(position.x, position.y + 60f, position.width, EditorGUIUtility.singleLineHeight);
        var fifthRect = new Rect(position.x, position.y + 80f, position.width, EditorGUIUtility.singleLineHeight);
        var sixthRect = new Rect(position.x, position.y + 100f, position.width, EditorGUIUtility.singleLineHeight);
        var seventhRect = new Rect(position.x, position.y + 120f, position.width, EditorGUIUtility.singleLineHeight);


        var type = property.FindPropertyRelative("Type");
        var dialogue = property.FindPropertyRelative("Dialogue");
        var animationName = property.FindPropertyRelative("AnimationName");
        var time = property.FindPropertyRelative("Time");
        var pos = property.FindPropertyRelative("Position");
        var target = property.FindPropertyRelative("Target");
        var clip = property.FindPropertyRelative("Clip");
        var effect = property.FindPropertyRelative("Effect");
        var maxZoom = property.FindPropertyRelative("MaxZoom");
        var speed = property.FindPropertyRelative("Speed");
        type.intValue = EditorGUI.Popup(effectRect, "Type", type.intValue, type.enumNames);

        switch ((CinematicEventMethod.CinematicEventType)type.intValue)
        {
            case CinematicEventMethod.CinematicEventType.None:
                time.floatValue = EditorGUI.FloatField(secondRect, "Time", time.floatValue);
                EditorGUI.ObjectField(thirdRect, clip);
                break;            
            case CinematicEventMethod.CinematicEventType.Animation:
                EditorGUI.ObjectField(secondRect, target);
                animationName.stringValue = EditorGUI.TextField(thirdRect, "AnimationName", animationName.stringValue);
                time.floatValue = EditorGUI.FloatField(forthRect, "Time", time.floatValue);
                EditorGUI.ObjectField(fifthRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.FadeIn:
                time.floatValue = EditorGUI.FloatField(secondRect, "Time", time.floatValue);
                EditorGUI.ObjectField(thirdRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.FadeOut:
                time.floatValue = EditorGUI.FloatField(secondRect, "Time", time.floatValue);
                EditorGUI.ObjectField(thirdRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.Dialogue:
                EditorGUI.ObjectField(secondRect, target);
                dialogue.stringValue = EditorGUI.TextField(thirdRect, "Dialogue", dialogue.stringValue);
                time.floatValue = EditorGUI.FloatField(forthRect, "Time", time.floatValue);
                EditorGUI.ObjectField(fifthRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.Move:
                EditorGUI.ObjectField(secondRect, pos);
                EditorGUI.ObjectField(thirdRect, target);
                speed.floatValue = EditorGUI.FloatField(forthRect, "Speed", speed.floatValue);
                time.floatValue = EditorGUI.FloatField(fifthRect, "Time", time.floatValue);
                EditorGUI.ObjectField(fifthRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.MoveAndDialogue:
                EditorGUI.ObjectField(secondRect, pos);
                EditorGUI.ObjectField(thirdRect, target);
                dialogue.stringValue = EditorGUI.TextField(forthRect, "Dialogue", dialogue.stringValue);
                speed.floatValue = EditorGUI.FloatField(fifthRect, "Speed", speed.floatValue);
                time.floatValue = EditorGUI.FloatField(sixthRect, "Time", time.floatValue);
                EditorGUI.ObjectField(seventhRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.Teleport:
                EditorGUI.ObjectField(secondRect, pos);
                EditorGUI.ObjectField(thirdRect, target);
                time.floatValue = EditorGUI.FloatField(forthRect, "Time", time.floatValue);
                EditorGUI.ObjectField(fifthRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.PlayCameraShake:
                time.floatValue = EditorGUI.FloatField(secondRect, "Time", time.floatValue);
                EditorGUI.ObjectField(thirdRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.StopCameraShake:
                time.floatValue = EditorGUI.FloatField(secondRect, "Time", time.floatValue);
                EditorGUI.ObjectField(thirdRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.CameraZoom:
                EditorGUI.ObjectField(secondRect, target);
                maxZoom.floatValue = EditorGUI.FloatField(thirdRect, "MaxZoom", maxZoom.floatValue);
                time.floatValue = EditorGUI.FloatField(forthRect, "Time", time.floatValue);
                EditorGUI.ObjectField(fifthRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.PlayMusic:
                EditorGUI.ObjectField(secondRect, clip);
                break;
            case CinematicEventMethod.CinematicEventType.TargetEffect:
                EditorGUI.ObjectField(secondRect, effect);
                EditorGUI.ObjectField(thirdRect, target);
                time.floatValue = EditorGUI.FloatField(forthRect, "Time", time.floatValue);
                break;
            case CinematicEventMethod.CinematicEventType.PositionEffect:
                EditorGUI.ObjectField(secondRect, effect);
                EditorGUI.ObjectField(thirdRect, pos);
                time.floatValue = EditorGUI.FloatField(forthRect, "Time", time.floatValue);
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
        return (140 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 2);
    }
}
