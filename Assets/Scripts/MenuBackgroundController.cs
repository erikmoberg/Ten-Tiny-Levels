using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackgroundController : MonoBehaviour {
    private float verticalPositionTarget;
    private float startTime;
    private Vector3 startMarkerPosition;
    private Vector3 endMarkerPosition;
    private float secondsToLerp;
    private Image image;

    // Use this for initialization
    void Start () {
        this.image = this.gameObject.GetComponent<Image>();
        this.verticalPositionTarget = this.image.rectTransform.rect.height / 4;
        this.secondsToLerp = 10;
        this.startTime = Time.time;
        this.startMarkerPosition = new Vector2(0, this.verticalPositionTarget);
        this.endMarkerPosition = this.transform.position; 
        var v = this.transform.localScale;
        v.Set(2, 2, 2);
        this.transform.localScale = v;
        this.NextImage();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {    
        var timeSinceStarted = Time.time - startTime;
        var percentageComplete = timeSinceStarted / secondsToLerp;
        this.SetTransparency(percentageComplete);
        transform.position = Vector3.Lerp(startMarkerPosition, endMarkerPosition, percentageComplete);
        
        if (this.transform.position.y == this.endMarkerPosition.y)
        {
            this.NextImage();
        }
    }

    private void SetTransparency(float percentageComplete)
    {
        var fadeChoppines = 20;
        var fadeLimit = 0.15f;

        percentageComplete = Mathf.Round(percentageComplete * fadeChoppines) / fadeChoppines;
        var c = this.image.color;
        c.a = Mathf.Clamp(percentageComplete < fadeLimit ? percentageComplete * (1 / fadeLimit) : percentageComplete > (1 - fadeLimit) ? (1 - percentageComplete) * (1 / fadeLimit) : 1, 0, 1);
        this.image.color = c;
    }

    private void NextImage()
    {
        transform.position = new Vector2(0, 0);
        this.startTime = Time.time;
        this.image.sprite = Resources.Load<Sprite>(LevelRepository.NextRandomized().BackgroundImage);
    }
}
