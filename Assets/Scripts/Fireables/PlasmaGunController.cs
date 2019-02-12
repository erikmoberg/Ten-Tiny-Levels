using UnityEngine;
using System.Collections;

public class PlasmaGunController : Fireable {

    private float bulletSpeed = 100f;
    public Sprite ReloadingSprite;
    private Sprite regularSprite;
    private float reloadTime = 0.1f;

    public override void Start()
    {
        var muzzleflash = this.transform.Find ("PlasmaMuzzleflash");
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
            var plasmaShotInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.PlasmaShot), this.MuzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
            plasmaShotInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(target.x * this.bulletSpeed, target.y * this.bulletSpeed);
            plasmaShotInstance.layer = layerMask;
            plasmaShotInstance.GetComponent<PlasmaShotController>().SetTimeToLive(0.6f + Random.Range(-0.2f, 0.2f));

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
        GetComponent<SpriteRenderer>().sprite = this.ReloadingSprite;
        yield return new WaitForSeconds(this.reloadTime);
        GetComponent<SpriteRenderer>().sprite = this.regularSprite;
    }
}
