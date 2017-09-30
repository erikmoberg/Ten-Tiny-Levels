﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;

public abstract class CharacterBase : MonoBehaviour, IDamageable 
{
    private Transform groundCheck;
    private float ownHeight;

    bool shouldFlipOnStart;

    Collider2D mainCollider;

    private float MaxJumpSpeed = 5000f;
    protected float MaxRunSpeed = 70f;
    protected bool isFacingRight = true;
	protected int startingHealth = 20;
    protected int health;
	protected abstract LayerMask ProjectileLayerMask { get; }
    public bool IsDying;
    protected Rigidbody2D rigidBody;
    protected Animator animator;

    protected Vector3 startPosition;

    protected int ownLayer;

    protected int platformLayer;

    int dropLayer;

    private bool isDropping;
    private float dropStartAt;
    private float ledgeThickness = 8;
    private int numberOfLives = 1;
    protected SpriteRenderer spriteRenderer;

    protected GameObject weapon;

    Collider2D[] isGroundedColliderResults = new Collider2D[10];

    protected bool CanRespawn;

    protected bool isStunned;

    [HideInInspector]
    public int NumberOfLives
    {
        get
        { 
            return this.numberOfLives;
        }
        set
        { 
            this.numberOfLives = value;
        }
    }

    public void Start()
    {
        this.health = this.startingHealth = DifficultyRepository.GetHealth(this.tag);
        this.startPosition = this.transform.position;
        this.rigidBody = this.GetComponent<Rigidbody2D> ();
        this.animator = GetComponent<Animator>();
        this.groundCheck = transform.Find("GroundCheck");
        this.ownLayer = this.gameObject.layer;
        this.platformLayer = LayerMask.NameToLayer(LayerNames.Platforms);
        this.dropLayer = LayerMask.NameToLayer(LayerNames.Drop);
        this.mainCollider = GetComponent<Collider2D>();
        this.ownHeight = this.mainCollider.bounds.size.y;
        this.spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (this.shouldFlipOnStart)
        {
            this.Flip();
        }
    }

    public void InstantiateWeapon(string weaponName)
    {
        var gunResource = Resources.Load<GameObject>(weaponName);
        var gunPosition = new Vector3(this.transform.position.x + gunResource.transform.position.x, this.transform.position.y + gunResource.transform.position.y, this.transform.position.z);
        this.weapon = Instantiate(gunResource, gunPosition, Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject;
        weapon.GetComponent<SpriteRenderer>().sortingLayerID = this.GetComponent<SpriteRenderer>().sortingLayerID;
        weapon.transform.parent = this.gameObject.transform;
    }

	public void Flip ()
	{ 
        if (this.rigidBody == null)
        {
            this.shouldFlipOnStart = true;
            return;
        }

		this.isFacingRight = !this.isFacingRight;
		var scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
	
    public event EventHandler OnDeath;

    public event EventHandler OnRespawn;

    protected abstract string DeathParticleSystemName { get; }
    public bool OriginalIsFacingRight { get; internal set; }
    public abstract string DeathAudioClipName { get; }
    public abstract string HitAudioClipName { get; }
    public abstract bool ShouldPlayMovementSounds { get; }

    public virtual void AddDamage (int damage, bool fromRight)
	{
        if (damage != int.MaxValue)
        {
            var damageText = Resources.Load<GameObject>("DamageText");
            var damageTextGameObject = Instantiate(damageText, this.transform.position, Quaternion.Euler(new Vector3())) as GameObject;
            damageTextGameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(UnityEngine.Random.Range(-40, 40), UnityEngine.Random.Range(30, 60));
            var textComponent1 = damageTextGameObject.GetComponent<Text>();
            var critical = false;
            if (critical)
            {
                damage *= 2;
                textComponent1.color = new Color(255, 0, 0);
            }
            else
            {
                textComponent1.color = new Color(128, 255, 0);
            }

            textComponent1.text = damage.ToString();
            Destroy(damageTextGameObject, 5);
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(this.Stun(damage, fromRight));
            }
        }

		this.health -= damage;

        if (this.health <= 0 && !this.IsDying)
        {
            this.IsDying = true;
            this.EmitDeathParticles();
            
            this.NumberOfLives--;
            this.gameObject.SetActive(false);
            if (this.OnDeath != null)
            {
                this.OnDeath(this, new EventArgs());
            }
            
            SfxHelper.PlayFromResourceAtCamera(this.DeathAudioClipName);
            if (this.NumberOfLives <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            SfxHelper.PlayFromResourceAtCamera(this.HitAudioClipName);
        }
	}

    public void EmitDeathParticles()
    {
        var particlesInstance = Instantiate(Resources.Load<ParticleSystem>(this.DeathParticleSystemName), this.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as ParticleSystem;

        var spriteRect = this.spriteRenderer.sprite.rect;
        var xOffset = (int)spriteRect.x;
        var width = spriteRect.width;
        var height = spriteRect.height;
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var pixelColor = this.spriteRenderer.sprite.texture.GetPixel(xOffset + i, j);
                var main = particlesInstance.main;
                main.startColor = pixelColor;
                if (main.startColor.color.a != 0 && i % 2 == 0)
                {
                    particlesInstance.Emit(1);
                }
            }
        }

        Destroy(particlesInstance.gameObject, 10);
    }

    IEnumerator Stun(int damage, bool fromRight)
    {
        this.isStunned = true;
        var horizontalForce = 200;
        var verticalForce = 100;
        this.rigidBody.AddForce(new Vector2(fromRight ? -horizontalForce : horizontalForce, verticalForce));
        yield return new WaitForSeconds(0.5f);
        this.isStunned = false;
    }

    public void TryRespawn()
    {
        if (!this.CanRespawn)
        {
            return;
        }

        transform.GetComponentInChildren<IFireable>().Reset();
        this.CanRespawn = false;
        this.IsDying = false;
        this.transform.position = this.startPosition;
        this.health = this.startingHealth; 
        this.isDropping = false;
        this.gameObject.layer = this.ownLayer;
        if (this.isFacingRight != this.OriginalIsFacingRight)
        {
            this.Flip();
        }

        this.gameObject.SetActive(true);

        if(this.OnRespawn != null)
        {
            OnRespawn(this, null);
        }
    }

    public void SetReadyToRespawn()
    {
        this.CanRespawn = true;
    }

	public void Fire() 
	{
        transform.GetComponentInChildren<IFireable>().Fire(() => this.isFacingRight, this.ProjectileLayerMask);
	}

    protected bool IsGrounded (bool needsPlatform)
    {
        var collider = this.groundCheck.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            return false;
        }

        var numberOfObjects = Physics2D.OverlapAreaNonAlloc(collider.bounds.min, collider.bounds.max, this.isGroundedColliderResults); 
        if (numberOfObjects > 1)
        {
            if (!needsPlatform )
            {
                return true;
            }
            else
            {
                for (var i = 0; i < numberOfObjects; i++)
                {
                    if (this.isGroundedColliderResults[i].gameObject.layer == this.platformLayer)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void Jump(float speed)
    {
        if (this.rigidBody.velocity.y == 0 && this.IsGrounded(false))
        {
            this.rigidBody.AddForce(Vector2.up * (Mathf.Clamp(speed, 0, this.MaxJumpSpeed)));

            if (this.ShouldPlayMovementSounds)
            {
                SfxHelper.PlayFromResourceAtCamera(ResourceNames.WalkAudioClip);
            }
        }
    }

    public void Drop()
    {
        if (this.rigidBody.velocity.y == 0 && this.IsGrounded(true))
        {
            this.gameObject.layer = this.dropLayer;
            this.isDropping = true;
            this.dropStartAt = this.rigidBody.position.y;

            if (this.ShouldPlayMovementSounds)
            {
                SfxHelper.PlayFromResourceAtCamera(ResourceNames.WalkAudioClip);
            }
        }
    }

    public void FixedUpdate()
    {
        if (this.isDropping)
        {
            if (this.dropStartAt - this.rigidBody.position.y >= this.ledgeThickness + this.ownHeight)
            {
                this.isDropping = false;
                this.gameObject.layer = this.ownLayer;
            }
        }

        animator.SetBool("Jump", !(this.rigidBody.velocity.y == 0 && this.IsGrounded(false)));
        animator.SetFloat("Speed", Mathf.Abs(this.rigidBody.velocity.x));

        UpdateHealthBar();
    }

    public void OnTriggerEnter2D (Collider2D col)
    {
        if (col.IsTouching(this.groundCheck.gameObject.GetComponent<BoxCollider2D>()))
        {
            if (this.rigidBody.velocity.y < 0 && 
                ((col.tag == TagNames.Player && this.tag == TagNames.Badguy) 
                || (col.tag == TagNames.Badguy && this.tag == TagNames.Player)
                || (col.tag == TagNames.Player && this.tag == TagNames.Player)))
            {
                // jump on head
                var damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    this.rigidBody.AddForce(Vector2.up * 3000);
                    damageable.AddDamage(int.MaxValue, false);
                }
            }
        }

        if (col.tag == TagNames.Badguy && col.gameObject.GetComponent<EnemyPodController>())
        {
            col.gameObject.GetComponent<EnemyPodController>().Touch();
        }
    }

    private void UpdateHealthBar()
    {
        var healthBarSpriteRenderer = gameObject.GetComponentsInChildren<SpriteRenderer>();
        var healthBar = healthBarSpriteRenderer.FirstOrDefault(x => x.name == "Health Bar Foreground");
        if (healthBar != null)
        {
            var scale = healthBar.gameObject.transform.localScale;
            scale.x = (this.health <= 0 ? 0 : this.health)/(float)this.startingHealth;
            healthBar.gameObject.transform.localScale = scale;
        }
    }
}
