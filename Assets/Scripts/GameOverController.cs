using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    private bool canProceed = false;

    void Start()
    {
        StartCoroutine(SetProceed());
    }

    IEnumerator SetProceed()
    {
        yield return new WaitForSeconds(0.5f);
        this.canProceed = true;
    }

    public void GoToMenu()
    {
        if (!this.canProceed)
        {
            return;
        }

        SceneManager.LoadScene(SceneNames.Menu);
    }

    public void Restart()
    {
        if (!this.canProceed)
        {
            return;
        }

        PlayerSettingsRepository.PlayerOneSettings.LivesLeft = DifficultyRepository.GetNumberOfLives();
        PlayerSettingsRepository.PlayerTwoSettings.LivesLeft = DifficultyRepository.GetNumberOfLives();
        GameState.CurrentLevel = 0;
        SceneManager.LoadScene(LevelRepository.AllLevels[GameState.CurrentLevel].SceneName);
    }
}
