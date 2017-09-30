using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMusicController : MonoBehaviour
{
    public AudioSource InGameMusic;

    public AudioSource MenuMusic;

    private static GameMusicController instance = null;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SettingsRepository.MusicEnabledChanged += (a, s) =>
        {
            this.SetMute(!SettingsRepository.MusicEnabled);
        };

        SceneManager.sceneLoaded += (scene, loadSceneMode) => 
        {
            if (scene.name == SceneNames.Menu)
            {
                this.Play(this.MenuMusic);
            }
            else
            {
                this.Play(this.InGameMusic);
            }
        };
        
        if (this.InGameMusic != null && !this.InGameMusic.isPlaying)
        {
            this.InGameMusic.Play();
        }
    }

    private void Play(AudioSource source)
    {
        var sources = this.GetComponents<AudioSource>();
        foreach(var s in sources)
        {
            if (s == source)
            {
                if (!s.isPlaying)
                {
                    s.Play();
                }
            }
            else
            {
                if (s.isPlaying)
                {
                    s.Stop();
                }
            }
        }
    }

    private void SetMute(bool mute)
    {
        var sources = this.GetComponents<AudioSource>();
        foreach (var s in sources)
        {
            s.mute = mute;
        }
    }
}