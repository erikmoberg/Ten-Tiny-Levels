using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour {

    public SpriteRenderer Explosion;
    private float baseDamage = 10;

    private Vector2 forceVector;

    float torque;

    public void Start()
    {
        var multiplyBy = Random.Range(1f, -1f);
        this.torque = 0.3f * multiplyBy;
        this.forceVector = new Vector2(3, this.torque);
    }

    public void Update()
    {
        GetComponent<Rigidbody2D>().AddRelativeForce(this.forceVector, ForceMode2D.Impulse);
        GetComponent<Rigidbody2D>().AddTorque(this.torque);
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        // Explosion
        Instantiate (this.Explosion, transform.position, Quaternion.Euler (new Vector3 (0, 0)));
        var damageable = col.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.AddDamage((int)Random.Range(this.baseDamage - this.baseDamage * 0.8f, this.baseDamage * 1.2f), GetComponent<Rigidbody2D>().velocity.x < 0);
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
        Instantiate (this.Explosion, transform.position, Quaternion.Euler (new Vector3 (0, 0)));
        Destroy(this.gameObject);
    }
}
