using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Dialogue
{
    public Dialogue(string text, TriggerType trigger)
    {
        Text = text;
        Trigger = trigger;
    }
    public enum TriggerType
    {
        None,
        Trade,
        Event,
        Fail,
        Success
    }
    [TextArea]
    public string Text = string.Empty;
    public TriggerType Trigger = TriggerType.None;
}