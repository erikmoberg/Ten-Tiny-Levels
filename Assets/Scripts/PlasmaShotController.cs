using UnityEngine;
using System.Collections;

public class PlasmaShotController : MonoBehaviour {

    public SpriteRenderer Richochet;
    private float baseDamage = 10;

    private Vector2 forceVector;

    public void Start()
    {
        var multiplyBy = Random.Range(1f, -1f);
        this.forceVector = new Vector2(0, 50 * multiplyBy);
    }

    public void Update()
    {
        GetComponent<Rigidbody2D>().AddForce(this.forceVector);
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
        SfxHelper.PlayClipAtCamera(GetComponent<AudioSource>());
        Instantiate (this.Richochet, transform.position, Quaternion.Euler (new Vector3 (0, 0)));
        Destroy(this.gameObject);
    }
}
