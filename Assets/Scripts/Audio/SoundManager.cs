using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum Sound
    {
        Sound1,
        Navigate,
        TrainStationAmbient,
        TrainComing,
        TrainLeaving,
        Typing,
        QuietClick,
        HeartBeat,
        Pulsating_1,
        Pulsating_2,
        ShopBackground,
        Cut
    }

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    private static GameObject ambientSoundGameObject;

    private static GameObject audioHolder;

    private static Dictionary<Sound, AudioSource> loopedSounds = new Dictionary<Sound, AudioSource>();

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.Sound1] = 0f;
        audioHolder = new GameObject("All Sounds");
    }

    public static void PlayLoopedSound(Sound sound, float volume = 1)
    {
        if (!CanPlaySound(sound))
            return;

        AudioSource audioSource;
        if (loopedSounds.ContainsKey(sound))
        {
            audioSource = loopedSounds[sound];
            if (audioSource.isPlaying)
                return;
        }
        else
        {
            GameObject loopedSoundGameObject = new GameObject("Looped Sound");
            loopedSoundGameObject.transform.parent = audioHolder.transform;
            audioSource = loopedSoundGameObject.AddComponent<AudioSource>();
            loopedSounds[sound] = audioSource;
        }

        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.clip = GetAudioClip(sound);
        audioSource.Play();
    }

    public static void StopLoopedSound(Sound sound)
    {
        if (loopedSounds.ContainsKey(sound))
        {
            AudioSource audioSource = loopedSounds[sound];
            audioSource.Stop();
            Object.Destroy(audioSource.gameObject);  // Optionally, you can destroy the GameObject
            loopedSounds.Remove(sound);
        }
    }

    // 3D Sounds
    public static void PlaySound3DStatic(Sound sound, Vector3 position, float volume = 1)
    {
        if (!CanPlaySound(sound))
            return;

        GameObject soundGameObject = new GameObject("Sound");
        soundGameObject.transform.parent = audioHolder.transform;
        soundGameObject.transform.position = position;
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.volume = volume;
        audioSource.clip = GetAudioClip(sound);
        audioSource.Play();
        // todo: create a pool
        Object.Destroy(soundGameObject, audioSource.clip.length);
    }

    public static void PlaySound3D(Sound sound, Transform parent, bool looped = false, float volume = 1, float dopplerEffect = 1, float spread = 0, float minDist = 1, float maxDist = 500)
    {
        GameObject soundGameObject = new GameObject("Sound");
        soundGameObject.transform.parent = parent;
        soundGameObject.transform.localPosition = Vector3.zero;
        //Time.timeScale = 0f;
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.loop = looped;
        audioSource.maxDistance = 3;
        audioSource.volume = volume;
        audioSource.dopplerLevel = dopplerEffect;
        audioSource.spread = spread;
        audioSource.minDistance = minDist;
        audioSource.maxDistance = maxDist;
        audioSource.clip = GetAudioClip(sound);
        audioSource.Play();
    }

    public static void PlayAmbientSound(Sound sound, bool additive = false, float volume = 1)
    {
        AudioSource audioSource;

        if (additive)
        {
            if (ambientSoundGameObject != null)
            {
                audioSource = ambientSoundGameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.clip = GetAudioClip(sound);
                audioSource.volume = volume;
                audioSource.Play();

                return;
            }
        }
        ambientSoundGameObject = new GameObject("AmbientSounds");
        ambientSoundGameObject.transform.parent = audioHolder.transform;
        audioSource = ambientSoundGameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = GetAudioClip(sound);
        audioSource.volume = volume;
        audioSource.Play();
    }

    public static void PlaySound2D(Sound sound, float volume = 1)
    {
        if (!CanPlaySound(sound))
            return;
        if (oneShotGameObject == null)
        {
            oneShotGameObject = new GameObject("One Shot Sound");
            if (oneShotGameObject == null)
                return;
            oneShotGameObject.transform.parent = audioHolder.transform;
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }
        oneShotAudioSource.volume = volume;

        oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
    }

    public static void PlaySound2DRandomChanges(Sound sound, float rndMinVol, float rndMaxVol, float rndMinPitch, float rndMaxPitch)
    {
        if (!CanPlaySound(sound))
            return;
        if (oneShotGameObject == null)
        {
            oneShotGameObject = new GameObject("One Shot Sound");
            oneShotGameObject.transform.parent = audioHolder.transform;
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }
        oneShotAudioSource.volume = Random.Range(rndMinVol, rndMaxVol);
        oneShotAudioSource.pitch = Random.Range(rndMinPitch, rndMaxPitch);

        oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
    }

    // Put a sound in here, if it cannot overlap and Initialize it
    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;
            case Sound.Sound1:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .05f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                else
                    return false;
                }
                else
                {

                    return true;
                }
                break;
        }
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (var audioClip in SoundsHolder.Sounds.soundAudioClip)
        {
            if (audioClip.sound == sound)
                return audioClip.audioClip;
        }

        Debug.LogError("Add " + sound + " sound to the Sounds Scriptable Object");
        return null;
    }
}
