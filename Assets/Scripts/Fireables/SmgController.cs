using UnityEngine;
using System.Collections;
using System;

public class SmgController : Fireable {
	
	private float bulletSpeed = 150f;
	 
	public override void Start () 
	{
        this.MuzzlePositionObject = this.transform.Find("MuzzlePosition");
        var muzzleflash = this.transform.Find ("ShotgunMuzzleflash");
		this.muzzleflashRenderer = muzzleflash.GetComponent<SpriteRenderer>();
        base.Start();
	}
	
	public override void Fire(System.Func<bool> getIsFacingRight, LayerMask layerMask, Vector2 targetPosition) 
	{		
		if (this.canFire)
		{
            StartCoroutine(FireBullets(getIsFacingRight, layerMask, targetPosition));
            StartCoroutine(ResetCanFire());
		}
	}

    IEnumerator FireBullets(System.Func<bool> getIsFacingRight, LayerMask layerMask, Vector2 targetPositionWorld)
    {
        this.canFire = false;

        var target = this.GetProjectileVectorAndRotate(targetPositionWorld, getIsFacingRight());

        for (var i = 0; i < 3; i++)
        {
            var isFacingRight = getIsFacingRight();
            var z = isFacingRight ? 0 : 180f;
            var multiplyBy = isFacingRight ? 1 : -1;

            SfxHelper.PlaySound(GetComponent<AudioSource>());
            StartCoroutine(ShowMuzzleflash());
            var randomness = UnityEngine.Random.Range(-5, 5);
            var bulletInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.Bullet), this.MuzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
            bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(target.x * this.bulletSpeed + randomness, target.y * this.bulletSpeed + randomness);
            bulletInstance.layer = layerMask;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ShowMuzzleflash() 
	{
		muzzleflashRenderer.enabled = true;
		yield return new WaitForSeconds (0.05f);
		muzzleflashRenderer.enabled = false;
	}

	IEnumerator ResetCanFire() 
	{
		yield return new WaitForSeconds (0.5f);
		this.canFire = true;
	}
}
