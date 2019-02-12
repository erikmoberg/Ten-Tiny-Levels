using System;
using UnityEngine;

public static class SettingsRepository
{
    public static bool MusicEnabled
    {
        get
        {
            return PlayerPrefs.GetInt("MusicEnabled") == 1;
        }
        set
        {
            PlayerPrefs.SetInt("MusicEnabled", value ? 1 : 0);
            if (MusicEnabledChanged != null)
            {
                MusicEnabledChanged(MusicEnabled, new EventArgs());
            }
        }
    }

    public static bool SfxEnabled
    {
        get
        {
            return PlayerPrefs.GetInt("SfxEnabled") == 1;
        }
        set
        {
            PlayerPrefs.SetInt("SfxEnabled", value ? 1 : 0);
        }
    }

    public static bool HasInitialized
    {
        get
        {
            return PlayerPrefs.GetInt("HasInitialized") == 1;
        }
        set
        {
            PlayerPrefs.SetInt("HasInitialized", value ? 1 : 0);
        }
    }

    public static int GetHighScore(GameMode gameMode)
    {
        return PlayerPrefs.GetInt("HighScore" + gameMode);
    }

    public static void SetHighScore(GameMode gameMode, int score)
    {
        PlayerPrefs.SetInt("HighScore" + gameMode, score);
    }

    public static event EventHandler MusicEnabledChanged;
}
