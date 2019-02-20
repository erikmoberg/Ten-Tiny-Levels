using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class GeneralScript : MonoBehaviour {

    [HideInInspector]
    public static PlayerController Player1;

    [HideInInspector]
    public static PlayerController Player2;

    private bool isGameOver = false;

    public bool HasInitialized { get; set; }

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update () 
    {
        if (Player1 == null)
        {
            if (!this.HasInitialized)
            {
                return;
            }

            if (GameState.GameMode == GameMode.SinglePlayer)
            {
                StartCoroutine(GoToMenu());
            }
        }
        else if (!this.HasInitialized)
        {
            Player1.OnDeath += (sender, e) => StartCoroutine(HandlePlayerDeath());
            if (Player2 != null)
            {
                Player2.OnDeath += (sender, e) => StartCoroutine(HandlePlayerDeath());
            }

            this.HasInitialized = true;
        }

        if (PointingDeviceManager.Player1Data.swipeDirection.Tap || Input.GetButtonDown("Fire1")) 
        {
            Player1.TryRespawn();
        }

        if (PointingDeviceManager.Player2Data.swipeDirection.Tap || Input.GetButtonDown("Fire2"))
        {
            if (Player2)
            {
                Player2.TryRespawn();
            }
        }

        if (GameState.GameMode == GameMode.SinglePlayer)
        {
            if (Player1.NumberOfLives <= 0)
            {
                StartCoroutine(GoToGameOver());
            }
            else if (GameObject.FindGameObjectsWithTag(TagNames.Badguy).Length == 0)
            {
                StartCoroutine(GoToNextLevel());
            }
        }
        else if (GameState.GameMode == GameMode.TwoPlayerDeathmatch)
        {
            if (Player1.NumberOfLives == 0 || Player2.NumberOfLives == 0)
            {
                StartCoroutine(GoToDeathmatchEnd());
            }
        }
        else if (GameState.GameMode == GameMode.TwoPlayerCoop)
        {
            if (Player1.NumberOfLives <= 0 && Player2.NumberOfLives <= 0)
            {
                StartCoroutine(GoToGameOver());
            }
            else if (GameObject.FindGameObjectsWithTag(TagNames.Badguy).Length == 0)
            {
                StartCoroutine(GoToNextLevel());
            }
        }
	}

    public static PlayerController[] GetPlayers()
    {
        return new[] { GeneralScript.Player1, GeneralScript.Player2 };
    }

    IEnumerator HandlePlayerDeath()
    {
        if (GameState.GameMode == GameMode.TwoPlayerDeathmatch && Player1.NumberOfLives > 0 && Player2.NumberOfLives > 0)
        {
            yield return new WaitForSeconds(2f);
            
            // Randomize new weapons
            PlayerSettingsRepository.PlayerOneSettings.SelectedWeapon =
                PlayerSettingsRepository.PlayerTwoSettings.SelectedWeapon =
                Fireable.AllWeaponResourceNames[UnityEngine.Random.Range(0, Fireable.AllWeaponResourceNames.Length - 1)];

            SceneManager.LoadScene(LevelRepository.NextRandomized().SceneName);
        }
    }

    IEnumerator GoToNextLevel() 
    {
        var currentLevel = GameState.CurrentLevel;
        yield return new WaitForSeconds (3f);
        // in case triggered twice, do not advance twice
        if (currentLevel == GameState.CurrentLevel && !this.isGameOver)
        {
            if (GameState.CurrentLevel < LevelRepository.AllLevels.Length - 1)
            {
                GameState.CurrentLevel++;
            }
            else
            {
                GameState.CurrentLevel = 0;
                GameState.Difficulty += 1;
            }

            SceneManager.LoadScene(LevelRepository.AllLevels[GameState.CurrentLevel].SceneName);
        }
    }

    IEnumerator GoToMenu() 
    {
        yield return new WaitForSeconds (3f);
        SceneManager.LoadScene(SceneNames.Menu);
    }

    IEnumerator GoToGameOver()
    {
        this.isGameOver = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneNames.GameOver);
    }

    IEnumerator GoToDeathmatchEnd()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneNames.DeathmatchEnd);
    }
}
