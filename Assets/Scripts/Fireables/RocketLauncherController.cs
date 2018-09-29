using UnityEngine;
using System.Collections;

public class RocketLauncherController : Fireable {

    private float rocketSpeed = 25f;

    public override void Start()
    {
        this.MuzzlePositionObject = this.transform.Find("MuzzlePosition");
        var muzzleflash = this.transform.Find ("PlasmaMuzzleflash");
        this.muzzleflashRenderer = muzzleflash.GetComponent<SpriteRenderer>();
        base.Start();
    }

    public override void Fire(System.Func<bool> getIsFacingRight, LayerMask layerMask, Vector2 targetPositionWorld) 
    {       
        if (this.canFire)
        {
            SfxHelper.PlaySound(GetComponent<AudioSource>());
            this.canFire = false;

            var isFacingRight = getIsFacingRight();
            //var z = isFacingRight ? 0 : 180f;
            var multiplyBy = isFacingRight ? 1 : -1;
            var target = this.GetProjectileVectorAndRotate(targetPositionWorld, getIsFacingRight());
            var z = GetAngle(target, a => a);
            var rocketInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.Rocket), this.MuzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
            rocketInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(target.x * this.rocketSpeed, target.y * this.rocketSpeed);
            rocketInstance.layer = layerMask;

            StartCoroutine(ResetCanFire());
            StartCoroutine(ShowMuzzleflash());
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
        yield return new WaitForSeconds (0.4f);
        // SfxHelper.PlayFromResourceAtCamera(ResourceNames.RocketReloadAudioClip);
        yield return new WaitForSeconds (0.6f);
        this.canFire = true;
    }
}
