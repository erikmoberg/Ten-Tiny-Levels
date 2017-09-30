using UnityEngine;

public static class SfxHelper
{
    public static void PlaySound(AudioSource audioSource)
    {
        if (SettingsRepository.SfxEnabled)
        {
            audioSource.Play();
        }
    }

    public static void PlayClipAtCamera(AudioSource audioSource)
    {
        SfxHelper.PlayClipAtCamera(audioSource.clip);
    }

    public static void PlayClipAtCamera(AudioClip audioClip)
    {
        if (SettingsRepository.SfxEnabled)
        {
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
        }
    }

    public static void PlayFromResourceAtCamera(string resourceName)
    {
        SfxHelper.PlayClipAtCamera(Resources.Load<AudioClip>(resourceName));
    }
}