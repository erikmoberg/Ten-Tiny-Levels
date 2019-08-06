using UnityEngine;
using System.Collections;
using System;

public class PlayerSpawnLocation : SpawnLocation {

    public SpawnLocationType SpawnLocationType;

    protected override GameObject Character
    {
        get
        {
            return Resources.Load<GameObject>(ResourceNames.Player);
        }
    }
        
    protected override bool ShouldInstantiate(GameMode gameMode)
    {
        if(gameMode == GameMode.TwoPlayerDeathmatch 
            && (this.SpawnLocationType == SpawnLocationType.Player1Deatchmatch 
                || this.SpawnLocationType == SpawnLocationType.Player2Deatchmatch))
        {
            return true;
        }

        if (gameMode == GameMode.TwoPlayerCoop
            && (this.SpawnLocationType == SpawnLocationType.Player1Single
                || this.SpawnLocationType == SpawnLocationType.Player2Single))
        {
            return true;
        }

        if (gameMode == GameMode.SinglePlayer && (this.SpawnLocationType == SpawnLocationType.Player1Single))
        {
            return true;
        }

        return false;
    }
        
    protected override void AfterInstantiation(CharacterBase instance)
    {
        if (this.SpawnLocationType == SpawnLocationType.Player1Single || this.SpawnLocationType == SpawnLocationType.Player1Deatchmatch)
        {
            var player1 = instance as PlayerController;
            GeneralScript.Player1 = player1;
            player1.PlayerSettings = PlayerSettingsRepository.PlayerOneSettings;
        }

        if (this.SpawnLocationType == SpawnLocationType.Player2Single || this.SpawnLocationType == SpawnLocationType.Player2Deatchmatch)
        {
            var player2 = instance as PlayerController;
            GeneralScript.Player2 = player2;
            player2.PlayerSettings = PlayerSettingsRepository.PlayerTwoSettings;
        }
    }

    protected override void BeforeInstantiation(GameMode gameMode)
    {
        if (gameMode == GameMode.TwoPlayerDeathmatch)
        {
            var topMargin = 32;
            var bottomMargin = 16;
            var minY = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y + bottomMargin;
            var maxY = Camera.main.ScreenToWorldPoint(new Vector2(0, Camera.main.pixelHeight)).y - topMargin;
            this.transform.position = new Vector2(this.transform.position.x, UnityEngine.Random.Range(minY, maxY));
        }
    }
}
