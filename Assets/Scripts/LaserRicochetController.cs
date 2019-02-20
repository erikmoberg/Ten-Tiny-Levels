using UnityEngine;
using System.Collections;

public class LaserRicochetController : MonoBehaviour {
    
    private float alpha = 1;
    
    private SpriteRenderer spriteRenderer;
    

    public void Start () 
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, 3);
	}
	
	public void Update () 
    {
        this.alpha -= Time.deltaTime * 1f;
        var newColor = new Color(255, 255, 255, Mathf.Max(0, this.alpha));
        this.spriteRenderer.color = newColor;
	}
}
