using UnityEngine;

internal class IdlePlayerData
{
    public PlayerController Player { get; internal set; }
    public Vector2 Position { get; internal set; }
    public float StartIdleTime { get; internal set; }
}