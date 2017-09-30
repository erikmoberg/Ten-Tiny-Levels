using UnityEngine;
using System.Collections;

public class PlasmaGunController : MonoBehaviour, IFireable {

    private float bulletSpeed = 100f;
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
            var plasmaShotInstance = Instantiate(Resources.Load<GameObject>(ResourceNames.PlasmaShot), this.muzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
            plasmaShotInstance.GetComponent<Rigidbody2D>().velocity = new Vector2((this.bulletSpeed) * multiplyBy, 0);
            plasmaShotInstance.layer = layerMask;
            plasmaShotInstance.GetComponent<PlasmaShotController>().SetTimeToLive(0.6f + Random.Range(-0.2f, 0.2f));

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
        yield return new WaitForSeconds (0.1f);
        this.canFire = true;
    }
}
