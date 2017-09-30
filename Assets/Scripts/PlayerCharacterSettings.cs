using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerCharacterSettings
{
    public string SpriteSheet { get; set; }
    public string ProjectileLayer { get; set; }
    public int Layer { get; set; }
    public string SelectedWeapon { get; set; }
    public string JumpKey { get; set; }
    public string DropKey { get; set; }
    public string LeftKey { get; set; }
    public string RightKey { get; set; }
    public string FireKey { get; set; }
    public int LivesLeft { get; set; }
    public string LivesLeftIconResource { get; set; }
    public PointingDeviceData PointingDeviceData { get; set; }
}

