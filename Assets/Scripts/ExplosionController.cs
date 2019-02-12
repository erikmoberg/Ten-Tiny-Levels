using UnityEngine;
using System.Collections;
using System;

public class ExplosionController : MonoBehaviour 
{
    private SpriteRenderer spriteRenderer;

    public ParticleSystem Particles;

    float baseDamage = 20f;

    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        SfxHelper.PlayFromResourceAtCamera(ResourceNames.ExplosionAudioClip);
        StartCoroutine(DeactivateDamage());
        StartCoroutine(DestroySelf());
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        var damageable = col.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.AddDamage((int)UnityEngine.Random.Range(this.baseDamage - this.baseDamage * 0.8f, this.baseDamage * 1.2f), GetComponent<CircleCollider2D>().bounds.center.x > col.bounds.center.x);
        }
    }

    IEnumerator DeactivateDamage()
    {
        yield return new WaitForSeconds(0.3f);
        GetComponent<Collider2D>().enabled = false;
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.8f);
        GameObject.Destroy(gameObject);
    }
}
