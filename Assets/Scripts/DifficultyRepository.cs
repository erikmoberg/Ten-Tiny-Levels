using UnityEngine;

public static class DifficultyRepository
{
    public static int GetHealth(string tag)
    {
        if (tag == TagNames.Badguy)
        {
            return (int)(5 + (Factor * 1.5));
        }
        else if (tag == TagNames.Player)
        {
            if (GameState.GameMode == GameMode.TwoPlayerDeathmatch)
            {
                return 20;
            }

            return Mathf.Max(10, 50 - (Factor));
        }
        else
        {
            Debug.Log("Unknown tag: " + tag);
            return 10;
        }
    }

    public static float GetTimeUntilFlipSeconds()
    {
        return 1f / (1 + Factor);
    }

    public static float GetTimeUntilFireSeconds()
    {
        return GetTimeUntilFlipSeconds() * 1.5f;
    }

    public static float GetTimeUntilMoveActionSeconds()
    {
        return 1 + 3f / (1 + Factor);
    }

    public static float GetRunSpeed()
    {
        return 25 + ((float)Factor/10) * 3;
    }

    public static float GetAggressiveRunSpeed()
    {
        return 35 + ((float)Factor / 10) * 5;
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
        return Mathf.Max(5, 10 - (Factor / 10));
    }

    public static float GetIdleTimeSeconds
    {
        get
        {
            return Mathf.Max(1, 5 - (Factor / 10));
        }
    }

    private static int Factor
    {
        get
        {
            return GameState.Difficulty * 10 + GameState.CurrentLevel;
        }
    }
}
