using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundClip
{
    public enum SoundType
    {
        Attack,
        Death,
        Run,
        Skill
    }

    public SoundType Type;
    public AudioClip Clip;
}
