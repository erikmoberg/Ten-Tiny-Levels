using UnityEngine;
using System.Collections;

public class GrenadeLauncherController : Fireable {

    private float grenadeShellSpeed = 100f;
    private readonly float reloadTime = 1.2f;
    public Sprite ReloadingSprite1;
    public Sprite ReloadingSprite2;
    private Sprite regularSprite;

    public override void Start()
    {
        this.MuzzlePositionObject = this.transform.Find("MuzzlePosition");
        var muzzleflash = this.transform.Find("Muzzleflash");
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
            var multiplyBy = isFacingRight ? 1 : -1;
            var target = this.GetProjectileVectorAndRotate(targetPositionWorld, getIsFacingRight());
            var z = GetAngle(target, a => a);
            var grenadeInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.GrenadeShell), this.MuzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
            grenadeInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(target.x * this.grenadeShellSpeed, target.y * this.grenadeShellSpeed);
            grenadeInstance.GetComponent<GrenadeShellController>().SetTimeToLive(4);
            grenadeInstance.layer = layerMask;

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
        GetComponent<SpriteRenderer>().sprite = this.ReloadingSprite1;
        yield return new WaitForSeconds(this.reloadTime / 2);
        GetComponent<SpriteRenderer>().sprite = this.ReloadingSprite2;
        yield return new WaitForSeconds(this.reloadTime / 2);
        GetComponent<SpriteRenderer>().sprite = this.regularSprite;
    }
}
