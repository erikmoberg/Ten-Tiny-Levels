using UnityEngine;
using System.Collections;
using System.Linq;

public class LaserRifleController : MonoBehaviour, IFireable {
	
	private Transform muzzlePositionObject;

    public Sprite ReloadingSprite;

	private Renderer muzzleflashRenderer;
	private bool canFire = true;
    private Sprite regularSprite;
	 
	void Start () 
	{
        this.muzzlePositionObject = this.transform.Find("MuzzlePosition");
        var muzzleflash = this.transform.Find ("LaserMuzzleflash");
		this.muzzleflashRenderer = muzzleflash.GetComponent<SpriteRenderer>();
        this.regularSprite = GetComponent<SpriteRenderer>().sprite;
        this.Reset();
	}
	
	public void Fire(System.Func<bool> getIsFacingRight, LayerMask layerMask) 
	{		
		if (this.canFire)
		{
            StartCoroutine(FireLaser(getIsFacingRight, layerMask));
            StartCoroutine(ResetCanFire());
		}
	}

    public void Reset()
    {
        muzzleflashRenderer.enabled = false;
        this.canFire = true;
        GetComponent<SpriteRenderer>().sprite = this.regularSprite;
    }

    IEnumerator FireLaser(System.Func<bool> getIsFacingRight, LayerMask layerMask)
    {
        this.canFire = false;

        var isFacingRight = getIsFacingRight();
        var z = isFacingRight ? 0 : 180f;

        SfxHelper.PlaySound(GetComponent<AudioSource>());
        StartCoroutine(ShowMuzzleflash());
        var laserShotInstance = Instantiate(Resources.Load<GameObject>("LaserShot"), this.muzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
        var lineRenderer = laserShotInstance.GetComponent<LineRenderer>();
        var stopAt = this.GetNextObstacleHorizontalPosition(isFacingRight);
        var from = new Vector3(this.muzzlePositionObject.position.x, this.muzzlePositionObject.position.y, -1);
        var to = new Vector3(stopAt, this.muzzlePositionObject.position.y, -1);
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
        var controller = lineRenderer.GetComponent<LaserShotController>();
        controller.FromPosition = from;
        controller.ToPosition = to;
        GetComponent<SpriteRenderer>().sprite = this.ReloadingSprite;
        yield return new WaitForSeconds (1.4f);
        GetComponent<SpriteRenderer>().sprite = this.regularSprite;
    }

    float GetNextObstacleHorizontalPosition(bool isFacingRight)
    {
        var from = this.muzzlePositionObject.position;
        var to = new Vector2(isFacingRight ? Camera.main.pixelWidth : 0, this.muzzlePositionObject.position.y);
        var overlaps = Physics2D.LinecastAll(from, to, LayerMask.GetMask(LayerNames.Level, LayerNames.Platforms));
        if (overlaps.Length == 0)
        {
            return to.x;
        }

        if (isFacingRight)
        {
            return overlaps.Min(x => x.point.x);
        }

        return overlaps.Max(x => x.point.x);
    }

	IEnumerator ShowMuzzleflash() 
	{
		muzzleflashRenderer.enabled = true;
		yield return new WaitForSeconds (0.05f);
		muzzleflashRenderer.enabled = false;
	}

	IEnumerator ResetCanFire() 
	{
		yield return new WaitForSeconds (1.5f);
		this.canFire = true;
	}
}
