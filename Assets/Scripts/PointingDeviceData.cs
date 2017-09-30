using System;
using UnityEngine;

public class PointingDeviceData
{
    public Vector2 firstPressPosition;
    public Vector2 secondPressPosition;
    public Vector2 currentSwipe;

    public bool IsPressing;
    public bool ExecutedMove;

    public CurrentInput swipeDirection = new CurrentInput();

    public int? fingerId;

    public bool hasAction;
    public float swipeVelocity = 10000;
    internal float touchStartedOn;
}
