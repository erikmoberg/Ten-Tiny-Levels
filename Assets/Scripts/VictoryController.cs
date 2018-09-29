using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryController : MonoBehaviour
{
    private bool canProceed = false;
    private int count = 0;
    public UnityEngine.UI.Text difficultyText;
    public UnityEngine.UI.Text nextDifficultyHeaderText;
    public UnityEngine.UI.Text nextDifficultyText;
    public GameObject restartButton;

    void Start()
    {
        difficultyText.text = GameState.Difficulty.ToString().ToUpperInvariant();
        var nextDifficulty = (int)GameState.Difficulty + 1;
        if (!Enum.IsDefined(typeof(Difficulty), nextDifficulty))
        {
            // finished on max level
            this.restartButton.SetActive(false);
            nextDifficultyHeaderText.text = "CONGRATULATIONS";
            nextDifficultyText.text = "A WINNER IS YOU";
        }
        else
        {
            nextDifficultyText.text = ((Difficulty)nextDifficulty).ToString().ToUpperInvariant();
        }

        StartCoroutine(FireFireworks());
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
        GameState.Difficulty = GameState.Difficulty + 1;
        GameState.CurrentLevel = 0;
        SceneManager.LoadScene(LevelRepository.AllLevels[GameState.CurrentLevel].SceneName);
    }
}
