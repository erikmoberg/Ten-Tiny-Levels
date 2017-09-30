using UnityEngine;
using System.Collections;

public class SmgController : MonoBehaviour, IFireable {
	
	private float bulletSpeed = 150f;
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
            StartCoroutine(FireBullets(getIsFacingRight, layerMask));
            StartCoroutine(ResetCanFire());
		}
	}

    public void Reset()
    {
        muzzleflashRenderer.enabled = false;
        this.canFire = true;
    }

    IEnumerator FireBullets(System.Func<bool> getIsFacingRight, LayerMask layerMask)
    {
        this.canFire = false;

        for (var i = 0; i < 3; i++) 
        {
            var isFacingRight = getIsFacingRight();
            var z = isFacingRight ? 0 : 180f;
            var multiplyBy = isFacingRight ? 1 : -1;

            SfxHelper.PlaySound(GetComponent<AudioSource>());
            StartCoroutine(ShowMuzzleflash());
            var yRandomness = Random.Range(-10, 10);
            var xRandomness = 0;
            var bulletInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.Bullet), this.muzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
            bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2((this.bulletSpeed - xRandomness) * multiplyBy, yRandomness);
            bulletInstance.layer = layerMask;
            yield return new WaitForSeconds (0.1f);
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
