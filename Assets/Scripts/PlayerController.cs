﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : CharacterBase {
    
	private Vector2 vector2 = new Vector2();
    private List<GameObject> lifeIcons = new List<GameObject>();

    private Dictionary<string, Sprite> spriteSheet;
    private float slowdownFactor = 1.2f;

    private float swipeJumpSpeedMultiplier = 4f;
    private float swipeRunSpeedMultiplier = 0.06f;

    public PlayerCharacterSettings PlayerSettings
    {
        get;
        set;
    }
	
	protected override LayerMask ProjectileLayerMask
	{
		get
		{
            return LayerMask.NameToLayer(this.PlayerSettings.ProjectileLayer);
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
            return ResourceNames.PlayerDeathAudioClip;
        }
    }

    public override string HitAudioClipName
    {
        get
        {
            return ResourceNames.PlayerHitAudioClip;
        }
    }

    public override bool ShouldPlayMovementSounds
    {
        get
        {
            return true;
        }
    }

    public new void Start() 
	{
        this.gameObject.layer = this.PlayerSettings.Layer;
        this.NumberOfLives = this.PlayerSettings.LivesLeft;
        var livesLeftIconResource = Resources.Load<GameObject>(PlayerSettings.LivesLeftIconResource);
        var livesLeftLocation = livesLeftIconResource.transform.position;
        var drawFromLeft = livesLeftIconResource.GetComponent<LifeIconController>().DrawFromLeft;
        for (var i = 0; i < this.NumberOfLives; i++)
        {
            var position = new Vector2();
            var xBounds = livesLeftIconResource.GetComponent<SpriteRenderer>().bounds.size.x + 1;
            position.x = livesLeftLocation.x + (drawFromLeft ? 1 : -1) * i * xBounds;
            position.y = livesLeftLocation.y;
            var gameObject = Instantiate(livesLeftIconResource, position, Quaternion.Euler(new Vector3())) as GameObject;
            this.lifeIcons.Add(gameObject);
        }

        this.OnDeath += (sender, e) =>
        {
            var toRemove = this.lifeIcons[this.lifeIcons.Count - 1];
            this.lifeIcons.Remove(toRemove);
            Destroy(toRemove);
            this.PlayerSettings.LivesLeft = this.NumberOfLives;
            if (GameRules.ShouldPlayerCreatePodOnDeath(this)) 
            {
                var pod = Instantiate(Resources.Load<GameObject>(ResourceNames.PlayerPod), this.transform.position, Quaternion.Euler(new Vector3())) as GameObject;
                var podController = pod.GetComponent<PlayerPodController>();
                podController.SpawnLocation = this.startPosition;
                podController.Character = this;
            }
        };

        this.InstantiateWeapon(this.PlayerSettings.SelectedWeapon);

        this.spriteSheet = Resources.LoadAll<Sprite>(this.PlayerSettings.SpriteSheet).ToDictionary(x => x.name, x => x);

        base.Start();
	}

	void Update()
	{
        if (this.PlayerSettings.PointingDeviceData.swipeDirection.Tap)
        {
            this.Fire();
        }

        if (Input.GetKeyDown(this.PlayerSettings.FireKey)) 
		{
            this.Fire();
		}
	}

	public new void FixedUpdate () 
	{
        var moveHorizontal = (float)(Input.GetKey(this.PlayerSettings.LeftKey) ? -1 : Input.GetKey(this.PlayerSettings.RightKey) ? 1 : 0);
        var moveVertical = Input.GetKey(this.PlayerSettings.DropKey) ? -1 : Input.GetKey(this.PlayerSettings.JumpKey) ? 1 : 0;

        if (Input.GetKey(KeyCode.K))
        {
            var toDestroy = GameObject.FindGameObjectsWithTag(TagNames.Badguy);
            foreach(var t in toDestroy)
            {
                var badguy = t.GetComponent<BadguyController>();
                if (badguy != null)
                {
                    badguy.AddDamage(int.MaxValue, false);
                }

                var pod = t.GetComponent<EnemyPodController>();
                if (pod != null)
                {
                    pod.Touch();
                }
            }
        }

        if (Input.GetKey(KeyCode.L))
        {
            var weapons = new[] { "Smg", "Shotgun", "Plasma Gun", "Laser Rifle", "Rocket Launcher" };
            var index = System.Array.IndexOf(weapons, this.PlayerSettings.SelectedWeapon);
            index = (index + 1) % weapons.Length;
            GameObject.Destroy(this.weapon);
            this.PlayerSettings.SelectedWeapon = weapons.ElementAt(index);
            this.InstantiateWeapon(this.PlayerSettings.SelectedWeapon);
        }

        if (Input.GetKey(KeyCode.J))
        {
            SceneManager.LoadScene(LevelRepository.NextRandomized().SceneName);
        }

        if (this.PlayerSettings.PointingDeviceData.hasAction)
        {
            if (this.PlayerSettings.PointingDeviceData.swipeDirection.Up)
            {
                moveVertical = 1;
            }
            if (this.PlayerSettings.PointingDeviceData.swipeDirection.Down)
            {
                moveVertical = -1;
            }
            if (this.PlayerSettings.PointingDeviceData.swipeDirection.Left)
            {
                moveHorizontal = -1;
            }
            if (this.PlayerSettings.PointingDeviceData.swipeDirection.Right)
            {
                moveHorizontal = 1;
            }

            this.PlayerSettings.PointingDeviceData.hasAction = false;
        }

        if (moveHorizontal != 0)
        {
            this.vector2.Set(moveHorizontal * (Mathf.Clamp(this.PlayerSettings.PointingDeviceData.swipeVelocity * this.swipeRunSpeedMultiplier, 0, this.MaxRunSpeed)), this.rigidBody.velocity.y);
            if(this.rigidBody.velocity != this.vector2)
            {
                SfxHelper.PlayFromResourceAtCamera(ResourceNames.WalkAudioClip);
            }

            this.rigidBody.velocity = this.vector2;
        }
        else
        {
            var xSpeed = this.rigidBody.velocity.x + (this.isFacingRight ? -this.slowdownFactor : this.slowdownFactor);
            if ((this.isFacingRight && xSpeed < 5) || (!this.isFacingRight && xSpeed > -5))
            {
                xSpeed = 0;
            }

            this.vector2.Set(xSpeed, this.rigidBody.velocity.y);
            this.rigidBody.velocity = this.vector2;
        }

		if (moveVertical > 0)
		{
            this.Jump(this.PlayerSettings.PointingDeviceData.swipeVelocity * this.swipeJumpSpeedMultiplier);
		}

		if (moveVertical < 0) 
		{
            this.Drop();
		}

        if (this.rigidBody.velocity.x > 0 && !this.isFacingRight) 
		{
			this.Flip();
		} 
        else if (this.rigidBody.velocity.x < 0 && this.isFacingRight) 
		{
			this.Flip();
		}

        base.FixedUpdate();
	}

    void LateUpdate()
    {
        this.spriteRenderer.sprite = this.spriteSheet[this.spriteRenderer.sprite.name];
    }
}