using System.Linq;
using UnityEngine;

public class IdleMonitorController : MonoBehaviour
{
    public GameObject IdlePlayerHunter;

    private IdlePlayerData[] idlePlayerData;

    private float minimumMoveDistance = 10;

    void Update()
    {
        if (GameRules.IsTestMode)
        {
            return;
        }

        if (idlePlayerData == null)
        {
            var generalScript = GameObject.FindObjectOfType<GeneralScript>();
            if (generalScript.HasInitialized && this.idlePlayerData == null)
            {
                var players = GeneralScript.GetPlayers();
                this.InitializeIdlePlayerData(players);
            }
        }
        else
        {
            foreach (var entry in this.idlePlayerData)
            {
                if (entry != null)
                {
                    if (entry.Player.IsDying)
                    {
                        entry.StartIdleTime = Time.time;
                        continue;
                    }

                    var newPosition = this.GetPlayerPosition(entry.Player);
                    if (this.PlayerHasMoved(entry.Position, newPosition))
                    {
                        entry.StartIdleTime = Time.time;
                        entry.Position = newPosition;
                    }
                    else
                    {
                        if (this.TimerExpired(entry.StartIdleTime))
                        {
                            entry.StartIdleTime = Time.time;
                            var hunter = Instantiate(this.IdlePlayerHunter, this.GetHunterStartPosition(newPosition), Quaternion.Euler(new Vector3(0, 0)));
                            hunter.GetComponent<IdlePlayerHunterController>().EndLocation = newPosition;
                        }
                    }
                }
            }
        }
    }

    private Vector2 GetHunterStartPosition(Vector2 newPosition)
    {
        // Coordinates of each corner of the screen
        var positions = new[]
        {
            Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelRect.xMin, Camera.main.pixelRect.yMin)),
            Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelRect.xMax, Camera.main.pixelRect.yMin)),
            Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelRect.xMin, Camera.main.pixelRect.yMax)),
            Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelRect.xMax, Camera.main.pixelRect.yMax))
        };

        // Find the corner farthest away from the player
        return positions.FirstOrDefault(x => Vector2.Distance(x, newPosition) == positions.Max(y => Vector2.Distance(y, newPosition)));
    }

    private bool TimerExpired(float startIdleTime)
    {
        return Time.time - startIdleTime > DifficultyRepository.GetIdleTimeSeconds;
    }

    private bool PlayerHasMoved(Vector2 position, Vector2 newPosition)
    {
        return Vector2.Distance(position, newPosition) > this.minimumMoveDistance;
    }

    private void InitializeIdlePlayerData(PlayerController[] players)
    {
        this.idlePlayerData = players.Where(x => x != null).Select(x => new IdlePlayerData { Player = x, Position = this.GetPlayerPosition(x), StartIdleTime = Time.time }).ToArray();
    }

    private Vector2 GetPlayerPosition(PlayerController player)
    {
        return player.gameObject.transform.position;
    }
}
