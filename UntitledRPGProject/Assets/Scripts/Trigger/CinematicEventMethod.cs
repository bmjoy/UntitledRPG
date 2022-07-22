using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CinematicEventMethod
{
    public enum CinematicEventType
    {
        None,
        Animation,
        FadeIn,
        FadeOut,
        Move,
        MoveAndDialogue,
        Dialogue,
        Teleport,
        PlayCameraShake,
        StopCameraShake,
        CameraZoom,
        PlayMusic,
        TargetEffect,
        PositionEffect
    }

    public CinematicEventType Type;
    public string Dialogue = string.Empty;
    public string AnimationName = string.Empty;
    public float Time = 0.0f;
    public float Speed = 0.0f;
    public float MaxZoom = 0.0f;
    public Transform Position = null;
    public GameObject Target = null;
    public AudioClip Clip = null;
    public GameObject Effect = null;
}
