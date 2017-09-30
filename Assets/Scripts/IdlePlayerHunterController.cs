using System.Collections;
using UnityEngine;

public class IdlePlayerHunterController : MonoBehaviour
{
    [HideInInspector]
    public Vector2 EndLocation { get; set; }

    public GameObject Explosion;

    private float speed = 1.5f;
    private bool isClose = false;

    public void Awake()
    {
        SfxHelper.PlayFromResourceAtCamera(ResourceNames.IdlePlayerHunterAudioClip);
    }

    void Update()
    {
        this.transform.position = Vector2.Lerp(this.transform.position, this.EndLocation, this.speed * Time.deltaTime);
        var distanceToEndPosition = Vector2.Distance(this.transform.position, this.EndLocation);
        if (!this.isClose && distanceToEndPosition < 5)
        {
            this.isClose = true;
            StartCoroutine(Blink());
        }

        if (distanceToEndPosition < 2)
        {
            Instantiate(this.Explosion, transform.position, Quaternion.Euler(new Vector3(0, 0)));
            GameObject.Destroy(this.gameObject);
        }
    }

    private IEnumerator Blink()
    {
        var renderer = this.GetComponent<SpriteRenderer>();
        var originalColor = renderer.color;
        var blinkColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        while (true)
        {
            renderer.color = renderer.color.a == 0 ? originalColor : blinkColor;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
