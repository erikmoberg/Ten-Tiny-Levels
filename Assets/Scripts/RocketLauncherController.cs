using UnityEngine;
using System.Collections;

public class RocketLauncherController : MonoBehaviour, IFireable {

    private float rocketSpeed = 25f;
    private Transform muzzlePositionObject;
    private Renderer muzzleflashRenderer;
    private bool canFire = true;

    public void Start () 
    {
        this.muzzlePositionObject = this.transform.Find("MuzzlePosition");
        var muzzleflash = this.transform.Find ("PlasmaMuzzleflash");
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
            var rocketInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.Rocket), this.muzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
            rocketInstance.GetComponent<Rigidbody2D>().velocity = new Vector2((this.rocketSpeed) * multiplyBy, 0);
            rocketInstance.layer = layerMask;

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
        yield return new WaitForSeconds (0.4f);
        // SfxHelper.PlayFromResourceAtCamera(ResourceNames.RocketReloadAudioClip);
        yield return new WaitForSeconds (0.6f);
        this.canFire = true;
    }
}
