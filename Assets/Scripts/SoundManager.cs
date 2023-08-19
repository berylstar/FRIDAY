using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager I;

    public AudioSource bgm;
    public AudioSource effect;

    private void Awake()
    {
        I = this;
    }

    public void PlayEffect(string path)
    {
        effect.clip = Resources.Load<AudioClip>(path);
        effect.Play();
    }
}