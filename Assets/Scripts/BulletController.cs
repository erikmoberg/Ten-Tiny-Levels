using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public SpriteRenderer Richochet;
    public float baseDamage;

	void Start () 
	{
	}
	
	void Update () 
	{
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		Instantiate (this.Richochet, transform.position, Quaternion.Euler (new Vector3 (0, 0)));
		var damageable = col.GetComponent<IDamageable>();
		if (damageable != null)
		{
            damageable.AddDamage((int)Random.Range(this.baseDamage - this.baseDamage * 0.8f, this.baseDamage * 1.2f), GetComponent<Rigidbody2D>().velocity.x < 0);
		}
		else
		{
            SfxHelper.PlayClipAtCamera(GetComponent<AudioSource>());
        }

		Destroy (gameObject);
	}

    public void SetTimeToLive(float timeInSeconds)
    {
        StartCoroutine(Remove(timeInSeconds));
    }

    IEnumerator Remove(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        Instantiate (this.Richochet, transform.position, Quaternion.Euler (new Vector3 (0, 0)));
        Destroy(this.gameObject);
    }
}
