using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class BadguyController : CharacterBase {

	Vector2 vector2; 
    Transform frontCheck;
    public LayerMask playerLayers;
	bool hasSpottedPlayer;
	float spottedTime;

    [HideInInspector]
    public string WeaponName;

    [HideInInspector]
    public bool CanJump; 

    private float timeUntilFlipSeconds = 0.8f;
    private float timeUntilFireSeconds = 1.5f; 
    private float timeUntilMoveActionSeconds = 2f;

    private Dictionary<string, Sprite> defaultSpriteSheet;
    private Dictionary<string, Sprite> aggressiveSpriteSheet;
    private Dictionary<string, Sprite> spriteSheetToUse;

    float idleTime;

    bool isFiring;

    bool hasCollidedWithPlayer;

    BoxCollider2D playerDamager;

    public bool IsAggressive;

    private float jumpSpeed = 5000f;

    protected override LayerMask ProjectileLayerMask
	{
		get
		{
			return LayerMask.NameToLayer(LayerNames.BadguyProjectiles);
		}
	}

    protected override string DeathParticleSystemName
    {
        get
        {
            return ResourceNames.DeathParticleSystemBadguy;
        }
    }

    public override string DeathAudioClipName
    {
        get
        {
            return ResourceNames.BadguyDeathAudioClip;
        }
    }

    public override string HitAudioClipName
    {
        get
        {
            return ResourceNames.BadguyHitAudioClip;
        }
    }

    public override bool ShouldPlayMovementSounds
    {
        get
        {
            return false;
        }
    }

    public new void Start() 
	{ 
        this.MaxRunSpeed = DifficultyRepository.GetRunSpeed();
        this.timeUntilFlipSeconds = DifficultyRepository.GetTimeUntilFlipSeconds();
        this.timeUntilFireSeconds = DifficultyRepository.GetTimeUntilFireSeconds();
        this.timeUntilMoveActionSeconds = DifficultyRepository.GetTimeUntilMoveActionSeconds();

        this.frontCheck = transform.Find("FrontCheck");
        this.defaultSpriteSheet = Resources.LoadAll<Sprite>(this.CanJump ? "yellow_bad_guy" : "green_bad_guy" ).ToDictionary(x => x.name, x => x);
        this.aggressiveSpriteSheet = Resources.LoadAll<Sprite>("red_bad_guy").ToDictionary(x => x.name, x => x);
        this.spriteSheetToUse = this.defaultSpriteSheet;
        this.InstantiateWeapon(this.WeaponName);
        this.timeUntilMoveActionSeconds = this.timeUntilMoveActionSeconds / 2 + UnityEngine.Random.Range(0, timeUntilMoveActionSeconds);

        this.playerDamager = transform.Find("PlayerDamager").GetComponent<BoxCollider2D>();

        this.OnDeath += (sender, e) => 
        {
            var podInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.BadguyPod), this.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject;
            podInstance.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-10000, 10000), UnityEngine.Random.Range(5000, 10000)));
            podInstance.GetComponent<EnemyPodController>().SetWeapon(this.WeaponName);
        };

        base.Start();
	}
	
	public new void FixedUpdate()
	{
        if (this.spriteSheetToUse != this.aggressiveSpriteSheet && (this.IsAggressive || GameObject.FindGameObjectsWithTag(TagNames.Badguy).Length == 1))
        {
            this.spriteSheetToUse = this.aggressiveSpriteSheet;
            this.MaxRunSpeed = DifficultyRepository.GetAggressiveRunSpeed();
            this.CanJump = true;
        }

        if (GameRules.IsTestMode)
        {
            return;
        }

        if (this.isStunned)
        {
            return;
        }

        this.vector2.Set(this.hasCollidedWithPlayer ? 0 : this.MaxRunSpeed * (this.isFacingRight ? 1 : -1), this.rigidBody.velocity.y);
        this.rigidBody.velocity = this.vector2;

        var p = Physics2D.OverlapPoint(this.frontCheck.position, (1 << LayerMask.NameToLayer(LayerNames.Platforms)) | (1 << LayerMask.NameToLayer(LayerNames.Level)));
        if (p)
        {
            if (p.gameObject.layer != this.platformLayer || this.rigidBody.velocity.y == 0)
            {
                // only turn if we are on the ground or the object is not a platform - to avoid turning inside platforms when jumping
                this.Flip();
            }
        }
        else
        {
            var left = new Vector2(0, this.transform.position.y);
            var right = new Vector2(Camera.main.pixelWidth, this.transform.position.y);
            var player = Physics2D.Linecast(left, right, this.playerLayers);
            if (player != default(RaycastHit2D))
            {
                if (!this.hasSpottedPlayer)
                {
                    this.hasSpottedPlayer = true;
                    this.spottedTime = 0;
                }

                this.spottedTime += Time.fixedDeltaTime;

                if ((player.point.x < this.transform.position.x && this.isFacingRight)
                    || (player.point.x > this.transform.position.x && !this.isFacingRight))
                {
                    if (this.spottedTime > this.timeUntilFlipSeconds)
                    {
                        this.Flip();
                    }
                }

                if (this.spottedTime > this.timeUntilFireSeconds)
                {
                    this.isFiring = true;
                }
            }
            else
            {
                this.hasSpottedPlayer = false;
                this.spottedTime = 0;
                this.idleTime += Time.fixedDeltaTime;
                this.isFiring = false;
                this.hasCollidedWithPlayer = false;
                if (this.CanJump)
                {
                    if (this.idleTime > this.timeUntilMoveActionSeconds + UnityEngine.Random.Range(-0.3f, 0.3f))
                    {
                        this.idleTime = 0;

                        if (this.IsGrounded(false))
                        {
                            // get player from the scene
                            var playerObject = GameObject.FindGameObjectWithTag(TagNames.Player);
                            if (playerObject != null)
                            {
                                // check if above - then jump, otherwise, drop.
                                if (playerObject.transform.position.y > this.transform.position.y)
                                {
                                    this.Jump(this.jumpSpeed);    
                                }
                                else
                                { 
                                    this.Drop();
                                }
                            }
                        }
                    } 
                }
            }
        }    

        if (this.isFiring)
        {
            this.Fire();
        }

        base.FixedUpdate();
    }

    public new void OnTriggerEnter2D (Collider2D col)
    {
        if (col.IsTouching(this.playerDamager))
        {
            this.HandleCollision(col);
        }

        base.OnTriggerEnter2D(col);
    }

    void OnTriggerStay2D (Collider2D col)
    {
        this.HandleCollision(col);
    }

    void HandleCollision(Collider2D col)
    {
        if (col.tag == TagNames.Player)
        {
            this.hasCollidedWithPlayer = true;
            this.isFiring = true;
        }
    }

    public void LateUpdate()
    {
        this.spriteRenderer.sprite = this.spriteSheetToUse[this.spriteRenderer.sprite.name];
    }
}
