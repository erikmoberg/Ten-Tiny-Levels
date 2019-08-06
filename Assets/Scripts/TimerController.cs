using System.Collections;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public UnityEngine.UI.Text counterText;
    private float timeSeconds = MagicNumbers.TimerSeconds;
    private float timeLeft;
    private bool timerIsActive = true;
    private bool hasFinishedLevel = false;
    private bool isBlinking = false;
    private float blinkTime = 0.1f;

    public void CheckIfFinishedLevel()
    {
        if (this.hasFinishedLevel)
        {
            return;
        }

        if (GameObject.FindGameObjectsWithTag(TagNames.Badguy).Length > 0)
        {
            this.timerIsActive = true;
        }
        else
        {
            // all bad guys are gone
            SfxHelper.PlayFromResourceAtCamera(ResourceNames.FinishedLevelAudioClip);

            this.hasFinishedLevel = true;
            this.isBlinking = false;
            this.timerIsActive = false;
            var bonus = Mathf.Max(0, (int)((timeLeft - 5) / 5));
            this.GetComponent<Canvas>().sortingLayerName = LayerNames.Foreground;
            this.counterText.color = new Color(255, 255, 255);
            if (bonus > 0)
            {
                this.counterText.text += "\r\nTIME BONUS: " + bonus;
                GameState.Score += bonus;
            }
            else
            {
                this.counterText.text += "\r\nNO TIME BONUS";
            }
        }
    }

    void Awake ()
    {
        this.timeLeft = this.timeSeconds;
        this.UpdateText();

        if (GameState.GameMode == GameMode.TwoPlayerDeathmatch)
        {
            this.counterText.enabled = false;
            this.enabled = false;
        }
        else
        {
            this.counterText.enabled = true;
            this.enabled = true;
        }

        this.timerIsActive = false;
        StartCoroutine(this.WaitUntilStart());
    }

    private IEnumerator WaitUntilStart()
    {
        yield return new WaitForSeconds(LevelIntroController.GetWaitTimeUntilSpawnSeconds());
        this.timerIsActive = true;
    }

    void Update()
    {
        if (!this.timerIsActive)
        {
            return;
        }

        this.CheckIfFinishedLevel();

        if (!this.timerIsActive)
        {
            return;
        }

        if (timeLeft < 10)
        {
            this.counterText.color = new Color(255, 0, 0);
        }

        if (timeLeft < 5 && !this.isBlinking)
        {
            this.StartCoroutine(StartBlink());
            this.StartCoroutine(StartTickingSound());
        }

        if (timeLeft > 0)
        {
            if (this.timerIsActive)
            {
                timeLeft -= Time.deltaTime;
            }

            this.UpdateText();
        }
        else
        {
            this.counterText.text = "TIME'S UP!";

            var toDestroy = GameObject.FindGameObjectsWithTag(TagNames.Player);
            foreach (var t in toDestroy)
            {
                var player = t.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.NumberOfLives = 0;
                    player.AddDamage(int.MaxValue, false);
                }
                else
                {
                    var pod = t.GetComponent<PlayerPodController>();
                    if (pod != null)
                    {
                        pod.KillCharacter();
                    }
                }
            }
        }
    }

    private void UpdateText()
    {
        this.counterText.text = (Mathf.Max(timeLeft * 100, 0)).ToString("00:00");
    }

    private IEnumerator StartBlink()
    {
        this.isBlinking = true;
        while (this.isBlinking)
        {
            yield return new WaitForSeconds(this.blinkTime);
            this.counterText.enabled = !this.counterText.enabled;
        }

        this.counterText.enabled = true;
    }

    private IEnumerator StartTickingSound()
    {
        while (this.isBlinking && !(this.timeLeft <= 0))
        {
            SfxHelper.PlayFromResourceAtCamera(ResourceNames.TimerTickAudioClip);
            yield return new WaitForSeconds(1f);
        }
    }
}
