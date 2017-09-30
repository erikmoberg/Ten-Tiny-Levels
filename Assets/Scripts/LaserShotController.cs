using UnityEngine;
using System.Collections;

public class LaserShotController : MonoBehaviour {

    private float alpha = 1;

    private float baseDamage = 15f;

    public Vector3 FromPosition
    {
        get;
        set;
    }

    public Vector3 ToPosition
    {
        get;
        set;
    }

    public void Start () 
    {
        Destroy(gameObject, 3);

        this.DealDamage();
	}
	
	public void Update () 
    {
        this.alpha -= Time.deltaTime * 1f;
        var newColor = new Color(255, 0, 0, Mathf.Max(0, this.alpha));
        GetComponent<LineRenderer>().startColor = newColor;
        GetComponent<LineRenderer>().endColor = newColor;
	}

    void DealDamage()
    {
        var overlaps = Physics2D.LinecastAll(this.FromPosition, this.ToPosition, LayerMask.GetMask(LayerNames.Badguy, LayerNames.Player));
        foreach (var overlap in overlaps)
        {
            var damageable = overlap.collider.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.AddDamage((int)Random.Range(this.baseDamage * 0.8f, this.baseDamage * 1.2f), this.FromPosition.x > this.ToPosition.x);
            }
        }
    }
}
