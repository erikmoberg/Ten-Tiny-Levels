using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseButtonController : MonoBehaviour {

    private GameObject pauseMenu;

	void Start () 
    {
        this.pauseMenu = GameObject.FindGameObjectWithTag(TagNames.PauseMenu);
        if (pauseMenu != null)
        {
            this.pauseMenu.SetActive(false);
        }
	}

    public void HandlePress()
    {
        if (Time.timeScale == 1)
        {
            this.PauseGame();
        }
        else
        {
            this.ResumeGame();
        }
    }

    public void PauseGame()
    {
        this.SetImage("play");
        Time.timeScale = 0;
        this.pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        this.SetImage("pause");
        Time.timeScale = 1;
        this.pauseMenu.SetActive(false);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneNames.Menu);
    }

    void SetImage(string spriteResource)
    {
        GameObject.Find("Pause Button").GetComponent<Image>().sprite = Resources.Load<Sprite>(spriteResource);
    }
}
