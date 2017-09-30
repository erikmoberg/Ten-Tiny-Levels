using UnityEngine;
using System.Collections;
using System;

public class BadguySpawnLocation : SpawnLocation {

    public GameObject Weapon;

    public bool CanJump;

    protected override GameObject Character
    {
        get
        {
            return Resources.Load<GameObject>(ResourceNames.Badguy);
        }
    }

    protected override bool ShouldInstantiate(GameMode gameMode)
    {
        return gameMode == GameMode.SinglePlayer || gameMode == GameMode.TwoPlayerCoop;
    }

    protected override void AfterInstantiation(CharacterBase instance)
    {
        var badguy = instance as BadguyController;
        badguy.WeaponName = this.Weapon.name;
        badguy.CanJump = this.CanJump;
    }

    protected override void BeforeInstantiation(GameMode gameMode)
    {
        // do nothing
    }
}
