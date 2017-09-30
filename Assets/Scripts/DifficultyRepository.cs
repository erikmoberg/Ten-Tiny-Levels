using UnityEngine;
using System.Collections;

public static class DifficultyRepository {

    public static int GetHealth(string tag)
    {
        if (tag == TagNames.Badguy)
        {
            return 5 + ((int)GameState.Difficulty) * 10;
        }
        else if (tag == TagNames.Player)
        {
            if (GameState.GameMode == GameMode.TwoPlayerDeathmatch)
            {
                return 20;
            }

            return Mathf.Max(10, 50 - ((int)GameState.Difficulty) * 10);
        }
        else
        {
            Debug.Log("Unknown tag: " + tag);
            return 10;
        }
    }

    public static float GetTimeUntilFlipSeconds()
    {
        return 1f / (1 + (int)GameState.Difficulty);
    }

    public static float GetTimeUntilFireSeconds()
    {
        return GetTimeUntilFlipSeconds() * 1.5f;
    }

    public static float GetTimeUntilMoveActionSeconds()
    {
        return 1 + 3f / (1 + (int)GameState.Difficulty);
    }

    public static float GetRunSpeed()
    {
        return 25 + (int)GameState.Difficulty * 3;
    }

    public static float GetAggressiveRunSpeed()
    {
        return 35 + (int)GameState.Difficulty * 5;
    }

    public static int GetNumberOfLives()
    {
        if (GameState.GameMode == GameMode.TwoPlayerDeathmatch)
        {
            return 5;
        }

        return 3;
    }

    public static float GetPodTimeToLiveSeconds()
    {
        return Mathf.Max(5, 10 - (int)GameState.Difficulty);
    }

    public static float GetIdleTimeSeconds
    {
        get
        {
            return Mathf.Max(1, 5 - (int)GameState.Difficulty);
        }
    }
}
