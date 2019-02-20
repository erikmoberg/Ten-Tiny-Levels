using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnemyPodController : MonoBehaviour
{

    private float completedTime = 0;
    private float timeToLive;
    private ParticleSystem particles;

    // set to false to prevent player from capturing the object right away, such as jump on head
    private bool canTouch = false;
    public bool isAlive = true;
    private string weapon;

    public bool IsAlive
    {
        get
        {
            return this.isAlive;
        }
    }

    void Start()
    {
        this.timeToLive = DifficultyRepository.GetPodTimeToLiveSeconds();
        StartCoroutine(SpawnBadguys());
        StartCoroutine(Blink());
        this.particles = GetComponent<ParticleSystem>();
        StartCoroutine(SetCanTouch());
        var multiplyBy = Random.Range(2f, 1f);
        var direction = Random.Range(0, 2) > 1 ? 1 : -1;
        var torque = 1000 * multiplyBy * direction;
        GetComponent<Rigidbody2D>().AddTorque(torque);
    }

    void Update()
    {
        if (!this.isAlive)
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

    public string Weapon
    {
        get
        {
            return this.weapon ?? "Shotgun";
        }
        set
        {
            this.weapon = value;
        }
    }

    IEnumerator SpawnBadguys()
    {
        yield return new WaitForSeconds(this.timeToLive);
        if (this.isAlive)
        {
            var instance = Instantiate(Resources.Load<GameObject>(ResourceNames.Badguy), this.transform.position, new Quaternion()) as GameObject;
            var badguy = instance.GetComponent<BadguyController>();
            badguy.WeaponName = this.Weapon;
            badguy.CanJump = true;
            badguy.IsAggressive = true;
            badguy.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 45), ForceMode2D.Impulse);
            this.isAlive = false;
            SfxHelper.PlayFromResourceAtCamera(ResourceNames.EnemyRespawnAudioClip);
            Destroy(this.gameObject);
        }
    }

    IEnumerator Blink()
    {
        yield return new WaitForSeconds(this.timeToLive - 2f);
        var spriteRenderer = this.GetComponent<SpriteRenderer>();
        while (this.isAlive)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator SetCanTouch()
    {
        //var layer = this.gameObject.layer;
        //this.gameObject.layer = LayerMask.NameToLayer(LayerNames.OnlyLevel);
        //yield return new WaitForSeconds(0.5f);
        //this.gameObject.layer = layer;
        yield return new WaitForSeconds(0f);
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
            DisplayScoreText();

            this.isAlive = false;
            this.gameObject.layer = LayerMask.NameToLayer(LayerNames.OnlyLevel);
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            var e = this.gameObject.GetComponent<ParticleSystem>().emission;
            e.enabled = false;

            var particlesInstance = Instantiate(Resources.Load<ParticleSystem>(ResourceNames.TouchPodParticleSystem), this.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as ParticleSystem;
            var main = particlesInstance.main;
            for (var j = 0; j < 25; j++)
            {
                main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow, Color.red);
                particlesInstance.Emit(1);
            }

            this.gameObject.tag = TagNames.Removing;

            Destroy(particlesInstance.gameObject, 10);

            yield return new WaitForSeconds(3f);
            Destroy(this.gameObject);
        }
    }

    private void DisplayScoreText()
    {
        var scoreText = Resources.Load<GameObject>("ScoreText");
        var damageTextGameObject = Instantiate(scoreText, this.transform.position, Quaternion.Euler(new Vector3())) as GameObject;
        damageTextGameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 20);
        var textComponent1 = damageTextGameObject.GetComponent<Text>();
        GameState.Score++;
        textComponent1.text = GameState.Score.ToString();
        Destroy(damageTextGameObject, 1);
    }
}
