using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio Library")]
public class AudioLibrary : ScriptableObject
{
    public List<AudioClip> clips;

    public void PlayRandomClip(AudioSource audio)
    {
        if (clips.Count <= 0) return;

        audio.clip = clips[Random.Range(0, clips.Count)];
        audio.Play();
    }
}
