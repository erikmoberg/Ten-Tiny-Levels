﻿public static class GameState {

    public static string[] AvailableWeapons = new string[0];

    public static GameMode GameMode;

    public static Difficulty Difficulty = Difficulty.Easy;

    public static int CurrentLevel = 0;

    public static bool HasClickedGui { get; internal set; }

    public static int Score = 0;
}
