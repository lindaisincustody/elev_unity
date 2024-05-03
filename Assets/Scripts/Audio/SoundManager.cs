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
    }

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    private static GameObject ambientSoundGameObject;

    private static GameObject audioHolder;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.Sound1] = 0f;
        audioHolder = new GameObject("All Sounds");
    }

    // 3D Sounds
    public static void PlaySound3D(Sound sound, Vector3 position, float volume = 1)
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
