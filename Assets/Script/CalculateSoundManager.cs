using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public class CalculateSoundManager : MonoBehaviour
{
    public static CalculateSoundManager instance { get; private set; }
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] AudioSource bgmSound;
    [SerializeField] AudioSource sfxSound;
        private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySound(CalculateSoundName audioClipName, float volume)
    {
         var audioClip = audioClips.Find(x => x.name == audioClipName.ToString());
        if (audioClip)
        {
            sfxSound.clip = audioClip;
            sfxSound.volume = volume;
            sfxSound.Play();
        }
    }
    public void ChangeBGMSound(CalculateSoundName audioClipName, float volume)
    {
         var audioClip = audioClips.Find(x => x.name == audioClipName.ToString());
        if (audioClip)
        {
            bgmSound.clip = audioClip;
            bgmSound.volume = volume;
            bgmSound.Play();
        }
    }
}
public enum CalculateSoundName
{
    WIN,
    LOSE,
    CLICK,
}
