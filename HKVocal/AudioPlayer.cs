using System;
using UnityEngine;

namespace HKVocals;

public static class AudioPlayer
{
    public static bool TryPlayAudioFor(string convName, float removeTime = 0f, AudioSource asrc = null)
    {
        HKVocals.DoLogDebug($"Intentando reproducir audio para: {convName}");
        if (HasAudioFor(convName))
        {
            AudioClip clip = GetAudioClip(convName);
            if (clip == null) return false;

            if (removeTime != 0f)
            {
                PlayAudioWithTrim(clip, removeTime);
                return true;
            }

            PlayAudio(clip, asrc);
            return true;
        }
        else
        {
            HKVocals.DoLogDebug($"El audio no existe para: {convName}");
            return false;
        }
    }

    private static AudioClip GetAudioClip(string convoName) => GetAudioFor(convoName.ToLower());

    private static void PlayAudioWithTrim(AudioClip clip, float removeTime)
    {
        int remove = (int)(clip.frequency * removeTime);
        int size = clip.samples - remove;
        if (size <= 0) return;

        float[] samples = new float[size * clip.channels];
        clip.GetData(samples, remove);

        AudioClip newclip = AudioClip.Create(clip.name, size, clip.channels, clip.frequency, false);
        newclip.SetData(samples, 0);

        PlayAudio(newclip);
    }

    private static void PlayAudio(AudioClip clip, AudioSource asrc = null)
    {
        if (HKVocals.instance == null || clip == null) return;

        if (asrc == null)
        {
            asrc = HKVocals.instance.audioSource;
        }
        if (asrc == null)
        {
            HKVocals.instance.CreateAudioSource();
            asrc = HKVocals.instance.audioSource;
        }

        if (asrc == null) return;

        CheckForAchivements(clip.name);

        asrc.Stop();
        asrc.spatialBlend = 0f; // Modode audio 2D (sin atenuación por distancia)
        asrc.volume = 1f;
        asrc.mute = false;

        // Asignación de posición segura (evita excepciones durante transiciones de escena/menús)
        if (HeroController.instance != null && HeroController.instance.gameObject.activeInHierarchy)
        {
            asrc.transform.position = HeroController.instance.transform.position;
        }
        else if (Camera.main != null)
        {
            asrc.transform.position = Camera.main.transform.position;
        }
        else
        {
            asrc.transform.localPosition = new Vector3(15f, 10f, 1f);
        }

        // Asegurar la presencia de un AudioListener en escenas sin jugador activo
        if (UnityEngine.Object.FindObjectOfType<AudioListener>() == null && Camera.main != null)
        {
            Camera.main.gameObject.AddComponent<AudioListener>();
        }

        asrc.SetMixerGroup();
        MixerLoader.SetMixerVolume();

        asrc.clip = clip;
        asrc.Play();
    }

    private static void CheckForAchivements(string clip)
    {
        if (HKVocals._globalSettings == null) return;

        HKVocals._globalSettings.FinishedNPCDialoge?.RemoveAll(v => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
        HKVocals._globalSettings.FinishedDNailDialoge?.RemoveAll(v => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
        HKVocals._globalSettings.FinishedLoreTabletDialoge?.RemoveAll(v => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
        HKVocals._globalSettings.FinishedUIDialoge?.RemoveAll(v => v.Equals(clip, StringComparison.OrdinalIgnoreCase));

        if (GameManager.instance == null) return;

        if (!GameManager.instance.IsAchievementAwarded("Consideration") && HKVocals._globalSettings.FinishedNPCDialoge?.Count == 0)
            GameManager.instance.AwardAchievement("Consideration");

        if (!GameManager.instance.IsAchievementAwarded("Ambition") && HKVocals._globalSettings.FinishedDNailDialoge?.Count == 0)
            GameManager.instance.AwardAchievement("Ambition");

        if (!GameManager.instance.IsAchievementAwarded("Chronology") && HKVocals._globalSettings.FinishedLoreTabletDialoge?.Count == 0)
            GameManager.instance.AwardAchievement("Chronology");

        if (!GameManager.instance.IsAchievementAwarded("Acquisition") && HKVocals._globalSettings.FinishedUIDialoge?.Count == 0)
            GameManager.instance.AwardAchievement("Acquisition");

        if (
            GameManager.instance.IsAchievementAwarded("Consideration") &&
            GameManager.instance.IsAchievementAwarded("Ambition") &&
            GameManager.instance.IsAchievementAwarded("Chronology") &&
            GameManager.instance.IsAchievementAwarded("Acquisition") &&
            !GameManager.instance.IsAchievementAwarded("CompendiumVocalization")
        ) GameManager.instance.AwardAchievement("CompendiumVocalization");
    }

    public static bool IsPlaying() => HKVocals.instance?.audioSource != null && HKVocals.instance.audioSource.isPlaying;

    public static void StopPlaying()
    {
        if (HKVocals.instance?.audioSource != null)
        {
            HKVocals.instance.audioSource.Stop();
        }
    }

    public static bool HasAudioFor(string convName) => EmbeddedAudioLoader.HasAudioFor(convName);
    public static AudioClip GetAudioFor(string convName) => EmbeddedAudioLoader.GetAudioFor(convName);
}