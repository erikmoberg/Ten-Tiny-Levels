using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    private bool canProceed = false;
    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Text highscoreText;
    private int count = 0;

    void Start()
    {
        this.scoreText.text = "SCORE: " + GameState.Score;

        var highScore = SettingsRepository.GetHighScore(GameState.GameMode);
        if (GameState.Score > highScore)
        {
            this.highscoreText.enabled = true;
            SettingsRepository.SetHighScore(GameState.GameMode, GameState.Score);
            StartCoroutine(FireFireworks());
        }

        StartCoroutine(SetProceed());
    }

    IEnumerator FireFireworks()
    {
        if (count++ > 10)
        {
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        var fireworksInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.Fireworks), this.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject;
        fireworksInstance.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-2000, 2000), UnityEngine.Random.Range(10000, 20000)));
        StartCoroutine(FireFireworks());
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
        PlayerSettingsRepository.PlayerOneSettings.SelectedWeapon = MenuController.defaultWeapon;
        PlayerSettingsRepository.PlayerTwoSettings.SelectedWeapon = MenuController.defaultWeapon;
        GameState.CurrentLevel = 0;
        GameState.Score = 0;
        SceneManager.LoadScene(LevelRepository.AllLevels[GameState.CurrentLevel].SceneName);
    }
}
