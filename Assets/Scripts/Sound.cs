using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f,1f)]
    public float volume;
    [Range(.5f,2f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}
