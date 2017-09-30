using UnityEngine;
using System.Collections;

public abstract class SpawnLocation : MonoBehaviour {

    public bool StartWalkRight = true;

    protected abstract GameObject Character { get; }

    protected abstract void AfterInstantiation(CharacterBase instance);

    protected abstract void BeforeInstantiation(GameMode gameMode);

    protected abstract bool ShouldInstantiate(GameMode gameMode);

	void Awake () 
    {
        StartCoroutine(this.SpawnCharacter());
    }

    IEnumerator SpawnCharacter()
    {
        if (!this.ShouldInstantiate(GameState.GameMode))
        {
            yield break;
        }

        this.BeforeInstantiation(GameState.GameMode);

        yield return new WaitForSeconds(LevelIntroController.GetWaitTimeUntilSpawnSeconds());

        var gameObject = Instantiate(this.Character, this.transform.position, new Quaternion()) as GameObject;
        var instance = gameObject.GetComponent<CharacterBase>();
        instance.OriginalIsFacingRight = this.StartWalkRight;
        if (!this.StartWalkRight)
        {
            instance.Flip();
        }

        this.AfterInstantiation(instance);
    }
}
