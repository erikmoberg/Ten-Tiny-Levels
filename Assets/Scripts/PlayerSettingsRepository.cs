using UnityEngine;
using System.Collections;

public static class PlayerSettingsRepository
{
    public static PlayerCharacterSettings PlayerOneSettings = new PlayerCharacterSettings 
        {
            ProjectileLayer = LayerNames.Player1Projectiles,
            Layer = LayerMask.NameToLayer(LayerNames.Player),
            SelectedWeapon = "Rocket Launcher",
            JumpKey = "up",
            DropKey = "down",
            LeftKey = "left",
            RightKey = "right",
            FireKey = "right ctrl",
            LivesLeft = 3,
            SpriteSheet = "player",
            LivesLeftIconResource = "Life",
            PointingDeviceData = PointingDeviceManager.Player1Data
        };

    public static PlayerCharacterSettings PlayerTwoSettings = new PlayerCharacterSettings 
        {
            ProjectileLayer = LayerNames.Player2Projectiles,
            Layer = LayerMask.NameToLayer(LayerNames.Player),
            SelectedWeapon = "Smg",
            JumpKey = "w",
            DropKey = "s",
            LeftKey = "a",
            RightKey = "d",
            FireKey = "space",
            LivesLeft = 3,
            SpriteSheet = "player2",
            LivesLeftIconResource = "Life Player 2",
            PointingDeviceData = PointingDeviceManager.Player2Data
        };
}

