using UnityEngine;
using System.Collections;

public class ShotgunController : Fireable {

    public Sprite ReloadingSprite;
    private Sprite regularSprite;
    private float bulletSpeed = 200f;
    private float reloadTime = 0.5f;

    public override void Start()
    {
        var muzzleflash = this.transform.Find ("ShotgunMuzzleflash");
		this.muzzleflashRenderer = muzzleflash.GetComponent<SpriteRenderer>();
        this.regularSprite = GetComponent<SpriteRenderer>().sprite;
        base.Start();
	}
	 
	public override void Fire(System.Func<bool> getIsFacingRight, LayerMask layerMask, Vector2 targetPositionWorld) 
	{		
		if (this.canFire)
		{
            SfxHelper.PlaySound(GetComponent<AudioSource>());
            this.canFire = false;

            var isFacingRight = getIsFacingRight();
            var z = isFacingRight ? 0 : 180f;
            var multiplyBy = isFacingRight ? 1 : -1;
            var target = this.GetProjectileVectorAndRotate(targetPositionWorld, getIsFacingRight());
            for (var i = 0; i < 5; i++) 
			{
				var yRandomness = UnityEngine.Random.Range(-30, 70);
				var xRandomness = UnityEngine.Random.Range(-30, 30);
                var bulletInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.ShotgunPellet), this.MuzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
                bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(target.x * this.bulletSpeed + xRandomness, target.y * this.bulletSpeed + yRandomness);
                bulletInstance.layer = layerMask;
                bulletInstance.GetComponent<BulletController>().SetTimeToLive(0.6f + UnityEngine.Random.Range(-0.2f, 0.2f));
			}

            StartCoroutine(ResetCanFire());
            StartCoroutine(ShowMuzzleflash());
            StartCoroutine(ShowReloadAnimation());
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
		yield return new WaitForSeconds (this.reloadTime);
		this.canFire = true;
	}

    IEnumerator ShowReloadAnimation()
    {
        yield return new WaitForSeconds(this.reloadTime / 2);
        GetComponent<SpriteRenderer>().sprite = this.ReloadingSprite;
        yield return new WaitForSeconds(this.reloadTime / 2);
        GetComponent<SpriteRenderer>().sprite = this.regularSprite;
    }
}
