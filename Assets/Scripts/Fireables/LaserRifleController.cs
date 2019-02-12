using UnityEngine;
using System.Collections;
using System.Linq;

public class LaserRifleController : Fireable {

    public Sprite ReloadingSprite;
    private Sprite regularSprite;

    public override void Start()
    {
        var muzzleflash = this.transform.Find ("LaserMuzzleflash");
		this.muzzleflashRenderer = muzzleflash.GetComponent<SpriteRenderer>();
        this.regularSprite = GetComponent<SpriteRenderer>().sprite;
        base.Start();
	}
	
	public override void Fire(System.Func<bool> getIsFacingRight, LayerMask layerMask, Vector2 targetPosition) 
	{		
		if (this.canFire)
		{
            StartCoroutine(FireLaser(getIsFacingRight, layerMask, targetPosition));
            StartCoroutine(ResetCanFire());
		}
	}

    public override void Reset()
    {
        GetComponent<SpriteRenderer>().sprite = this.regularSprite;
        base.Reset();
    }

    IEnumerator FireLaser(System.Func<bool> getIsFacingRight, LayerMask layerMask, Vector2 targetPositionWorld)
    {
        this.canFire = false;

        var isFacingRight = getIsFacingRight();
        var z = isFacingRight ? 0 : 180f;
        var target1 = this.GetProjectileVectorAndRotate(targetPositionWorld, getIsFacingRight());
        SfxHelper.PlaySound(GetComponent<AudioSource>());
        StartCoroutine(ShowMuzzleflash());
        var laserShotInstance = Instantiate(Resources.Load<GameObject>("LaserShot"), this.MuzzlePositionObject.position, Quaternion.Euler(new Vector3(0, 0, z))) as GameObject;
        var lineRenderer = laserShotInstance.GetComponent<LineRenderer>();
        //var endTarget = targetPositionWorld;
        var stopAt = this.GetNextObstacleHorizontalPosition(isFacingRight, target1);
        var from = new Vector3(this.MuzzlePositionObject.position.x, this.MuzzlePositionObject.position.y, -1);
        var to = stopAt;// new Vector3(stopAt, this.MuzzlePositionObject.position.y, -1);
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
        lineRenderer.sortingLayerName = LayerNames.Foreground;
        var controller = lineRenderer.GetComponent<LaserShotController>();
        controller.FromPosition = from;
        controller.ToPosition = to;
        GetComponent<SpriteRenderer>().sprite = this.ReloadingSprite;
        yield return new WaitForSeconds (1.4f);
        GetComponent<SpriteRenderer>().sprite = this.regularSprite;
    }

    Vector2 GetNextObstacleHorizontalPosition(bool isFacingRight, Vector2 lineVector)
    {
        var from = new Vector2(this.MuzzlePositionObject.position.x, this.MuzzlePositionObject.position.y);
        //var to = target;//new Vector2(isFacingRight ? Camera.main.pixelWidth : 0, this.MuzzlePositionObject.position.y);
        var to = from + (500 * lineVector);
        var overlaps = Physics2D.LinecastAll(from, to, LayerMask.GetMask(LayerNames.Level, LayerNames.Platforms));
        if (overlaps.Length == 0)
        {
            return to;
        }

        return overlaps.First().point;
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
