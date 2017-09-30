using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuController : MonoBehaviour {

    public Button startButton;
    public GameObject MainMenu;
    public GameObject SinglePlayerMenu;
    public GameObject CoopMenu;
    public GameObject DeathmatchMenu;

    public UIScrollRectSnap weaponSelection;
    public UIScrollRectSnap difficultySelection;
    public UIScrollRectSnap levelSelection;
    public GridLayoutGroup weaponSelectionGridLayoutGroup;

    public UIScrollRectSnap coopWeaponSelection1;
    public UIScrollRectSnap coopWeaponSelection2;
    public UIScrollRectSnap coopdifficultySelection;

    public UIScrollRectSnap deathmatchWeaponSelection1;
    public UIScrollRectSnap deathmatchWeaponSelection2;

    // Use this for initialization
    void Start () 
    {
        this.InitializeSettings();

        if (this.weaponSelectionGridLayoutGroup != null)
        {
            var availableWeapons = new List<string>();
            for (var i = 0; i < this.weaponSelectionGridLayoutGroup.transform.childCount; i++)
            {
                availableWeapons.Add(this.weaponSelectionGridLayoutGroup.transform.GetChild(i).name);
            }

            GameState.AvailableWeapons = availableWeapons.ToArray();
        }

        if (this.levelSelection != null)
        {
            var index = 0;
            foreach (var actualLevel in LevelRepository.AllLevels)
            {
                var entry = this.levelSelection.GetChild(index++);
                var text = entry.GetComponentInChildren<Text>();
                text.text = actualLevel.Title.ToUpperInvariant();
            }
        }
    }

    private void InitializeSettings()
    {
        if (!SettingsRepository.HasInitialized)
        {
            SettingsRepository.MusicEnabled = true;
            SettingsRepository.SfxEnabled = true;
            SettingsRepository.HasInitialized = true;
        }

        var toggles = gameObject.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            if (toggle.name == "MusicToggle")
            {
                toggle.isOn = SettingsRepository.MusicEnabled;
            }

            if (toggle.name == "SfxToggle")
            {
                toggle.isOn = SettingsRepository.SfxEnabled;
            }
        }
    }

    public void StartSinglePlayerGame()
    {
        this.PlayButtonPressedAudio();
        PlayerSettingsRepository.PlayerOneSettings.SelectedWeapon = this.weaponSelection.currentOption;
        GameState.GameMode = GameMode.SinglePlayer;
        GameState.Difficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), this.difficultySelection.currentOption);
        PlayerSettingsRepository.PlayerOneSettings.LivesLeft = DifficultyRepository.GetNumberOfLives();
        var level = 1;
        int.TryParse(this.levelSelection.currentOption, out level);
        GameState.CurrentLevel = level - 1;
        SceneManager.LoadScene(LevelRepository.AllLevels[GameState.CurrentLevel].SceneName);
    }

    public void StartCoopGame()
    {
        this.PlayButtonPressedAudio();
        PlayerSettingsRepository.PlayerOneSettings.SelectedWeapon = this.coopWeaponSelection1.currentOption;
        PlayerSettingsRepository.PlayerTwoSettings.SelectedWeapon = this.coopWeaponSelection2.currentOption;
        GameState.GameMode = GameMode.TwoPlayerCoop;
        GameState.Difficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), this.coopdifficultySelection.currentOption);
        PlayerSettingsRepository.PlayerOneSettings.LivesLeft = DifficultyRepository.GetNumberOfLives();
        PlayerSettingsRepository.PlayerTwoSettings.LivesLeft = DifficultyRepository.GetNumberOfLives();
        GameState.CurrentLevel = 0;
        SceneManager.LoadScene(LevelRepository.AllLevels[GameState.CurrentLevel].SceneName);
    }

    public void StartTwoPlayersDeathmatchGame() 
    {
        this.PlayButtonPressedAudio();
        LevelRepository.Randomize();
        PlayerSettingsRepository.PlayerOneSettings.SelectedWeapon = this.deathmatchWeaponSelection1.currentOption;
        PlayerSettingsRepository.PlayerTwoSettings.SelectedWeapon = this.deathmatchWeaponSelection2.currentOption;
        GameState.GameMode = GameMode.TwoPlayerDeathmatch;
        PlayerSettingsRepository.PlayerOneSettings.LivesLeft = DifficultyRepository.GetNumberOfLives();
        PlayerSettingsRepository.PlayerTwoSettings.LivesLeft = DifficultyRepository.GetNumberOfLives();
        SceneManager.LoadScene(LevelRepository.NextRandomized().SceneName);
    }

    public void OpenCoopMenu()
    {
        this.PlayButtonPressedAudio();
        this.CoopMenu.SetActive(true);
        this.MainMenu.SetActive(false);
    }

    public void OpenSinglePlayerMenu()
    {
        this.PlayButtonPressedAudio();
        this.SinglePlayerMenu.SetActive(true);
        this.MainMenu.SetActive(false);
    }

    public void OpenDeathmatchMenu()
    {
        this.PlayButtonPressedAudio();
        this.DeathmatchMenu.SetActive(true);
        this.MainMenu.SetActive(false);
    }

    public void OpenTopMenu()
    {
        this.PlayButtonPressedAudio();
        this.SinglePlayerMenu.SetActive(false);
        this.CoopMenu.SetActive(false);
        this.DeathmatchMenu.SetActive(false);
        this.MainMenu.SetActive(true);
    }

    public void Quit() 
    {
        this.PlayButtonPressedAudio();
        Application.Quit();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(SceneNames.Menu);
    }

    public void MusicToggleChanged(bool isOn)
    {
        SettingsRepository.MusicEnabled = isOn;
    }

    public void SfxToggleChanged(bool isOn)
    {
        SettingsRepository.SfxEnabled = isOn;
    }

    private void PlayButtonPressedAudio()
    {
        SfxHelper.PlayFromResourceAtCamera(ResourceNames.ClickAudioClip);
    }
}
