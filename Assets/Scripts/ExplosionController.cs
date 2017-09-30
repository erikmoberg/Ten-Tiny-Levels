using UnityEngine;
using System.Collections;
using System;

public class ExplosionController : MonoBehaviour 
{
    private SpriteRenderer spriteRenderer;

    public ParticleSystem Particles;

    float baseDamage = 20f;

    private ParticleSystem particlesInstance;

    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        //this.particlesInstance = Instantiate (this.Particles, transform.position, Quaternion.Euler (new Vector3 (0, 0)));
        SfxHelper.PlayFromResourceAtCamera(ResourceNames.ExplosionAudioClip);
        StartCoroutine(DeactivateDamage());
        StartCoroutine(DestroySelf());
    }

    //void Update () 
    //{
    //    this.spriteRenderer.color = new Color(
    //            this.spriteRenderer.color.r, 
    //            this.spriteRenderer.color.g, 
    //            this.spriteRenderer.color.b, 
    //            Mathf.Clamp(this.spriteRenderer.color.a - Time.deltaTime, 0, 1));

    //    if (this.spriteRenderer.color.a == 0)
    //    {
    //        GameObject.Destroy(this.particlesInstance.gameObject);
    //        GameObject.Destroy(gameObject);
    //    }
    //}

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
        //GameObject.Destroy(this.particlesInstance.gameObject);
        GameObject.Destroy(gameObject);
    }
}
