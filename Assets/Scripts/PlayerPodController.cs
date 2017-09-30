using UnityEngine;

public class PlayerPodController : MonoBehaviour {

    [HideInInspector]
    public Vector2 SpawnLocation { get; set;}

    [HideInInspector]
    public CharacterBase Character { get; set;}

    private float speed = 1f;

    private bool hasReachedTarget = false;
	
	public void Update () 
    {
        this.transform.position = Vector2.Lerp(this.transform.position, this.SpawnLocation, this.speed * Time.deltaTime);
        if (!this.hasReachedTarget && Vector2.Distance(this.transform.position, this.SpawnLocation) < 3)
        {
            this.hasReachedTarget = true;
            this.Character.SetReadyToRespawn();
            this.Character.OnRespawn += HandlePlayerRespawn;
        }
	}

    private void HandlePlayerRespawn(object sender, System.EventArgs e)
    {
        this.Character.OnRespawn -= HandlePlayerRespawn;
        Destroy(this.gameObject);
    }
}
