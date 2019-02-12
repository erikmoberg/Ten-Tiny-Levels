using UnityEngine;
using System.Collections;

public class GrenadeShellController : MonoBehaviour {

    public SpriteRenderer Explosion;
    private Vector3 applyForceTo = new Vector3(10, 0, 0);
    private float baseDamage = 10;
    private new Rigidbody2D rigidbody;

    public void Awake()
    {
        this.rigidbody = this.GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        var force = new Vector2(0, this.rigidbody.velocity.x * 0.01f);
        rigidbody.AddForceAtPosition(force, this.applyForceTo);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.enabled && collision.relativeVelocity.magnitude > 10)
        {
            SfxHelper.PlaySound(GetComponent<AudioSource>());
        }
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        var damageable = col.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Explosion because hit target
            Instantiate(this.Explosion, transform.position, Quaternion.Euler(new Vector3(0, 0)));
            damageable.AddDamage((int)Random.Range(this.baseDamage - this.baseDamage * 0.8f, this.baseDamage * 1.2f), GetComponent<Rigidbody2D>().velocity.x < 0);
            Destroy(gameObject);
        }
    }

    public void SetTimeToLive(float timeInSeconds)
    {
        StartCoroutine(Remove(timeInSeconds));
    }

    IEnumerator Remove(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        Instantiate (this.Explosion, transform.position, Quaternion.Euler (new Vector3 (0, 0)));
        Destroy(this.gameObject);
    }
}
