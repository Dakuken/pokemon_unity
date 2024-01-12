using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> sfxList;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    
    [SerializeField] float fadeDuration = 0.75f;
    
    float originalMusicVolume;
    private Dictionary<AudioId, AudioData> sfxLookUp;
    
    public static AudioManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }
    
    private void Start()
    {
        originalMusicVolume = musicSource.volume;
        
        sfxLookUp = sfxList.ToDictionary(x => x.id);
    }
    
    public void PlaySFX(AudioClip sfxClip)
    {
        if(sfxClip == null) return;
        
        sfxSource.PlayOneShot(sfxClip);
    }

    public void PlaySFX(AudioId audioId)
    {
        if(!sfxLookUp.ContainsKey(audioId)) return;
        
        var audioData = sfxLookUp[audioId];
        PlaySFX(audioData.clip);
    }

    public void PlayMusic(AudioClip musicClip, bool loop = true, bool fade = false){
        
        if(musicClip == null) return;

        StartCoroutine(PlayMusicAsync(musicClip, loop, fade));
    }

    IEnumerator PlayMusicAsync(AudioClip musicClip, bool loop, bool fade)
    {
        if(fade)
            yield return musicSource.DOFade(0,fadeDuration).WaitForCompletion();
        
        musicSource.clip = musicClip;
        musicSource.loop = loop;
        musicSource.Play();
        
        if(fade)
            yield return musicSource.DOFade(originalMusicVolume, fadeDuration).WaitForCompletion();
    }
    

}

public enum AudioId{ UISelect, Hit, Faint, ExpGain}

[Serializable]
public class AudioData
{
    public AudioId id;
    public AudioClip clip;
}
