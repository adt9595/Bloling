using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public void play(AudioSource a, AudioClip clip) {
        a.PlayOneShot(clip);
    }
}
