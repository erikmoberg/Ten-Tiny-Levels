public static class GameRules 
{
    public static bool ShouldPlayerCreatePodOnDeath(PlayerController player)
    {
        return (GameState.GameMode == GameMode.SinglePlayer || GameState.GameMode == GameMode.TwoPlayerCoop) && player.NumberOfLives > 0; 
    }

    public static bool IsTestMode
    {
        get
        {
            return false;
        }
    }
}
