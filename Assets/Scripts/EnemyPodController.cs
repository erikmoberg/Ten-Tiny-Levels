using UnityEngine;
using System.Collections;

public class EnemyPodController : MonoBehaviour
{

    private float completedTime = 0;
    private float timeToLive;
    private string weapon = "Shotgun";
    private ParticleSystem particles;

    // set to false to prevent player from capturing the object right away, such as jump on head
    private bool canTouch = false;
    private bool isAlive = true;

    void Start () 
    {
        this.timeToLive = DifficultyRepository.GetPodTimeToLiveSeconds();
        StartCoroutine(SpawnBadguys());
        StartCoroutine(Blink());
        this.particles = GetComponent<ParticleSystem>();
        StartCoroutine(SetCanTouch());
    }
	
	void Update () 
    {
        if(!this.isAlive)
        {
            return;
        }

        this.completedTime += Time.deltaTime;
         
        if (Random.Range(0f, 1f) > 0.8f)
        {
            var main1 = particles.main;
            main1.startColor = new Color(1, 0, 0);
            return;
        }

        var colorComponent = Mathf.Clamp(this.completedTime / this.timeToLive, 0, 1);
        var main = particles.main;
        main.startColor = new Color(1, 1 - colorComponent, 1 - colorComponent);
	}

    public void SetWeapon(string weapon)
    {
        this.weapon = weapon;
    }

    IEnumerator SpawnBadguys()
    {
        yield return new WaitForSeconds(this.timeToLive);
        if (this.isAlive)
        {
            var instance = Instantiate(Resources.Load<GameObject>(ResourceNames.Badguy), this.transform.position, new Quaternion()) as GameObject;
            var badguy = instance.GetComponent<BadguyController>();
            badguy.WeaponName = this.weapon;
            badguy.CanJump = true;
            badguy.IsAggressive = true;
            badguy.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 45), ForceMode2D.Impulse);
            this.isAlive = false;
            Destroy(this.gameObject);
        }
    }

    IEnumerator Blink()
    {
        yield return new WaitForSeconds(this.timeToLive - 1.5f);
        var spriteRenderer = this.GetComponent<SpriteRenderer>();
        while (this.isAlive)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator SetCanTouch()
    {
        var layer = this.gameObject.layer;
        this.gameObject.layer = LayerMask.NameToLayer(LayerNames.OnlyLevel);
        yield return new WaitForSeconds(0.5f);
        this.gameObject.layer = layer;
        this.canTouch = true;
    }

    public void Touch()
    {
        StartCoroutine(this.DoTouch());
    }

    private IEnumerator DoTouch()
    {
        if (this.canTouch && this.isAlive)
        {
            SfxHelper.PlayFromResourceAtCamera(ResourceNames.TakeEnemyPodAudioClip);

            this.isAlive = false;
            this.gameObject.layer = LayerMask.NameToLayer(LayerNames.OnlyLevel);
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            var e = this.gameObject.GetComponent<ParticleSystem>().emission;
            e.enabled = false;

            var particlesInstance = Instantiate(Resources.Load<ParticleSystem>(ResourceNames.DeathParticleSystemBadguy), this.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as ParticleSystem;
            var main = particlesInstance.main;
            main.startColor = new ParticleSystem.MinMaxGradient(Color.red);
            for (var j = 0; j < 35; j++)
            {    
                particlesInstance.Emit(1);
            }

            Destroy(particlesInstance.gameObject, 10);

            yield return new WaitForSeconds(3f);
            Destroy(this.gameObject);
        }
    }
}
