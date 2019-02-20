using UnityEngine;
using System;

public class PointingDeviceManager : MonoBehaviour
{
    public static PointingDeviceData Player1Data = new PointingDeviceData();
    public static PointingDeviceData Player2Data = new PointingDeviceData();

    private float deadzoneInMillimeters = 6f;
    private float deadzoneInPixels = 50;

    void Start()
    {
        this.deadzoneInPixels = this.deadzoneInMillimeters * (Screen.dpi == 0 ? 150 : Screen.dpi)/25f;
    }

    void Update ()
    {
        DetectSwipe();
    }

    public void DetectSwipe ()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            // Handle mouse input if on PC
            if (Input.GetMouseButtonDown(0))
            {
                this.HandleStartPress(Input.mousePosition, Int32.MaxValue);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var data = Player1Data.fingerId != null ? Player1Data : Player2Data.fingerId != null ? Player2Data : null;
                if (data != null)
                {
                    this.HandleEndPress(Input.mousePosition, data);
                }
            }

            if (Player1Data.IsPressing)
            {
                this.HandleMove(Input.mousePosition, Player1Data);
            }

            if (Player2Data.IsPressing)
            {
                this.HandleMove(Input.mousePosition, Player2Data);
            }
        }

        if (Input.touches.Length > 0)
        {
            for (var i = 0; i < Input.touchCount; i++)
            {
                var t = Input.GetTouch(i);
                if (t.phase == TouchPhase.Began)
                {
                    if (Player1Data.fingerId == null || Player2Data.fingerId == null)
                    {
                        this.HandleStartPress(t.position, t.fingerId);
                    }
                }
                else
                {
                    var data = Player1Data.fingerId == t.fingerId ? Player1Data : Player2Data.fingerId == t.fingerId ? Player2Data : null;
                    if (data != null)
                    {
                        if (t.phase == TouchPhase.Moved)
                        {
                            this.HandleMove(t.position, data);
                        }
                        else if (t.phase == TouchPhase.Ended)
                        {
                            this.HandleEndPress(t.position, data);
                        }
                    }
                }
            }
        }

        if (!Player1Data.hasAction)
        {
            Player1Data.swipeDirection.Down = Player1Data.swipeDirection.Up = Player1Data.swipeDirection.Left = Player1Data.swipeDirection.Right = Player1Data.swipeDirection.Tap = false;
        }

        if (!Player2Data.hasAction)
        {
            Player2Data.swipeDirection.Down = Player2Data.swipeDirection.Up = Player2Data.swipeDirection.Left = Player2Data.swipeDirection.Right = Player2Data.swipeDirection.Tap = false;
        }
    }

    private static Vector3 ScreenPositionToWorld1(Vector3 screenPosition)
    {
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    void HandleStartPress(Vector2 position, int fingerId)
    {
        var worldPosition = ScreenPositionToWorld1(position);
        if (worldPosition.y > 7 || GameState.HasClickedGui)
        {
            // the user clicked the pause menu, do nothing
            return;
        }

        PointingDeviceData data = null;
        if (GameState.GameMode == GameMode.TwoPlayerDeathmatch || GameState.GameMode == GameMode.TwoPlayerCoop)
        {
            var players = GameObject.FindGameObjectsWithTag(TagNames.Player);
            var shortestDistance = float.MaxValue;
            PointingDeviceData pointingDeviceData = null;
            foreach (var player in players)
            {
                var distance = Vector2.Distance(player.transform.position, worldPosition);
                if (distance < shortestDistance)
                {
                    var controller = player.GetComponent<PlayerController>();
                    if (controller == null)
                    {
                        var podController = player.GetComponent<PlayerPodController>();
                        if (podController)
                        {
                            // player is trying to respawn
                            controller = podController.Character as PlayerController;
                        }
                    }
                    if (controller != null)
                    {
                        shortestDistance = distance;
                        pointingDeviceData = controller.PlayerSettings.PointingDeviceData;
                    }
                }
            }

            if (pointingDeviceData != null && pointingDeviceData.fingerId == null)
            {
                data = pointingDeviceData;
            }
        }
        else
        {
            data = Player1Data;
        }

        if (data != null)
        {
            data.ExecutedMove = false;
            data.firstPressPosition = new Vector2(position.x, position.y);
            data.fingerId = fingerId;
            data.hasAction = true;
            data.touchStartedOn = Time.time;
        }
    }

    void HandleMove(Vector2 position, PointingDeviceData pointingDeviceData)
    {
        var time = Time.time - pointingDeviceData.touchStartedOn;
        pointingDeviceData.secondPressPosition = new Vector2(position.x, position.y);
        pointingDeviceData.currentSwipe = new Vector2(pointingDeviceData.secondPressPosition.x - pointingDeviceData.firstPressPosition.x, pointingDeviceData.secondPressPosition.y - pointingDeviceData.firstPressPosition.y);

        pointingDeviceData.swipeDirection.Up = pointingDeviceData.currentSwipe.y > this.deadzoneInPixels;
        pointingDeviceData.swipeDirection.Down = pointingDeviceData.currentSwipe.y < -this.deadzoneInPixels;
        pointingDeviceData.swipeDirection.Left = pointingDeviceData.currentSwipe.x < -this.deadzoneInPixels;
        pointingDeviceData.swipeDirection.Right = pointingDeviceData.currentSwipe.x > this.deadzoneInPixels;
        
        pointingDeviceData.swipeVelocity = pointingDeviceData.currentSwipe.magnitude / time;

        pointingDeviceData.ExecutedMove = pointingDeviceData.swipeDirection.Up || pointingDeviceData.swipeDirection.Down || pointingDeviceData.swipeDirection.Left || pointingDeviceData.swipeDirection.Right;
        if (pointingDeviceData.ExecutedMove)
        {
            pointingDeviceData.fingerId = null;
            pointingDeviceData.hasAction = true;
            pointingDeviceData.IsPressing = false;
        }
    }

    void HandleEndPress(Vector2 position, PointingDeviceData data)
    {
        this.HandleMove(position, data);

        if (!data.ExecutedMove)
        {
            data.swipeDirection.Tap = true;
        }

        data.hasAction = true;
        data.IsPressing = false;
        data.fingerId = null;
    }
}