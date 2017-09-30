using UnityEngine;

public interface IFireable
{
    void Fire(System.Func<bool> getIsFacingRight, LayerMask layerMask);

    void Reset();
}
