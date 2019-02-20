using System;
using System.Collections;
using UnityEngine;

public class PlayerPodController : MonoBehaviour {

    [HideInInspector]
    public Vector2 SpawnLocation { get; set;}

    [HideInInspector]
    public CharacterBase Character { get; set;}

    private float speed = 1f;

    private bool hasReachedTarget = false;

    private SpriteRenderer spriteRenderer;
    private ParticleSystem particles;
    private bool playerHasRespawned;

    public void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.particles = this.GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        this.transform.position = Vector2.Lerp(this.transform.position, this.SpawnLocation, this.speed * Time.deltaTime);
        if (!this.hasReachedTarget && Vector2.Distance(this.transform.position, this.SpawnLocation) < 3)
        {
            this.hasReachedTarget = true;
            this.Character.SetReadyToRespawn();
            this.Character.OnRespawn += HandlePlayerRespawn;
            StartCoroutine(IndicateCanRespawn());
        }
	}

    IEnumerator IndicateCanRespawn()
    {
        while (!this.playerHasRespawned)
        {
            yield return new WaitForSeconds(0.4f);
            if (!this.playerHasRespawned)
            {
                this.spriteRenderer.enabled = !this.spriteRenderer.enabled;
            }
        }
    }

    private void HandlePlayerRespawn(object sender, System.EventArgs e)
    {
        this.playerHasRespawned = true;
        this.Character.OnRespawn -= HandlePlayerRespawn;
        RemoveSelf();
    }

    private void RemoveSelf()
    {
        Destroy(this.gameObject);
    }
}
