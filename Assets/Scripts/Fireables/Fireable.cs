using System;
using System.Collections;
using UnityEngine;

public abstract class Fireable : MonoBehaviour
{
    private int rotations = 0;

    public abstract void Fire(System.Func<bool> getIsFacingRight, LayerMask layerMask, Vector2 targetPosition);
    
    public static string[] AllWeaponResourceNames = new[] { "Smg", "Shotgun", "Plasma Gun", "Laser Rifle", "Rocket Launcher", "Grenade Launcher" };

    protected Renderer muzzleflashRenderer;

    protected bool canFire = true;

    public virtual void Reset()
    {
        if (muzzleflashRenderer != null)
        {
            muzzleflashRenderer.enabled = false;
        }
        
        this.canFire = true;
        this.transform.rotation = Quaternion.identity;
        this.rotations = 0;
    }

    public Transform MuzzlePositionObject { get; set; }

    private int rotationLimit = 45;

    public virtual void Start()
    {
        this.Reset();
        this.MuzzlePositionObject = this.transform.Find("MuzzlePosition");
    }

    protected Vector2 GetProjectileVectorAndRotate(Vector2 targetPositionWorld, bool isFacingRight)
    {
        var extrapolatedTargetPosition = this.Extrapolate(targetPositionWorld);

        this.Rotate(extrapolatedTargetPosition, isFacingRight);
        var target = this.GetProjectileVector(extrapolatedTargetPosition, isFacingRight, 0);
        return target;
    }

    private Vector2 Extrapolate(Vector2 targetPositionWorld)
    {
        var center = new Vector2(this.transform.position.x, this.transform.position.y);
        var vector = targetPositionWorld - center;
        return center + (100 * vector);
    }

    protected Vector2 GetProjectileVector(Vector2 targetPositionWorld, bool isFacingRight, int rotateBy)
    {
        var muzzle = new Vector2(this.MuzzlePositionObject.position.x, this.MuzzlePositionObject.position.y);
        var target = targetPositionWorld - muzzle;
        var angleDegrees = GetAngle(target, a => isFacingRight 
            ? Mathf.Clamp(a, -90 + this.rotationLimit, 90 - this.rotationLimit)
            : a > 0 ? Mathf.Clamp(a, 90 + this.rotationLimit, 180) : Mathf.Clamp(a, -180, -90 - this.rotationLimit));
        angleDegrees += rotateBy;

        var right = Quaternion.AngleAxis(angleDegrees, Vector3.forward) * Vector3.right;
        right.Normalize();
        return right;
    }

    protected void Rotate(Vector2 targetPositionWorld, bool isFacingRight)
    {
        var rotateBy = isFacingRight ? 0 : 180;
        var target = this.GetProjectileVector(targetPositionWorld, isFacingRight, rotateBy);
        this.rotations++;
        var angleDegrees = GetAngle(target, a => a);
        this.transform.rotation = Quaternion.AngleAxis(angleDegrees, Vector3.forward);
        StartCoroutine(ResetRotation());
    }

    protected static float GetAngle(Vector2 target, Func<float, float> clamper)
    {
        var angleRadians = Mathf.Atan2(target.y, target.x);
        var angleDegrees = angleRadians * Mathf.Rad2Deg;

        angleDegrees = clamper(angleDegrees);

        return angleDegrees;
    }

    IEnumerator ResetRotation()
    {
        yield return new WaitForSeconds(1);
        this.rotations--;
        if(this.rotations == 0)
        {
            this.transform.rotation = Quaternion.identity;
        }
    }
}
