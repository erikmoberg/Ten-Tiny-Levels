using UnityEngine;
using System.Collections;

public class ShotgunController : MonoBehaviour, IFireable {
	
	private float bulletSpeed = 200f;
	private Transform muzzlePositionObject;
	private Renderer muzzleflashRenderer;
	private bool canFire = true;
	 
	void Start () 
	{
        this.muzzlePositionObject = this.transform.Find("MuzzlePosition");
        var muzzleflash = this.transform.Find ("ShotgunMuzzleflash");
		this.muzzleflashRenderer = muzzleflash.GetComponent<SpriteRenderer>();
        this.Reset();
	}
	 
	public void Fire(System.Func<bool> getIsFacingRight, LayerMask layerMask) 
	{		
		if (this.canFire)
		{
            SfxHelper.PlaySound(GetComponent<AudioSource>());
            this.canFire = false;

            var isFacingRight = getIsFacingRight();
            var z = isFacingRight ? 0 : 180f;
            var multiplyBy = isFacingRight ? 1 : -1;
			for (var i = 0; i < 5; i++) 
			{
				var yRandomness = Random.Range(-30, 70);
				var xRandomness = Random.Range(-30, 30);
                var bulletInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.ShotgunPellet), this.muzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
                bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2((this.bulletSpeed - xRandomness) * multiplyBy, yRandomness);
				bulletInstance.layer = layerMask;
                bulletInstance.GetComponent<BulletController>().SetTimeToLive(0.6f + Random.Range(-0.2f, 0.2f));
			}

            StartCoroutine(ResetCanFire());
            StartCoroutine(ShowMuzzleflash());
		}
	}

    public void Reset()
    {
        muzzleflashRenderer.enabled = false;
        this.canFire = true;
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
