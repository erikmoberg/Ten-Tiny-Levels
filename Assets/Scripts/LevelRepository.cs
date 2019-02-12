using System.Linq;

public class LevelRepository
{
    private static Level[] cachedLevels;

    private static Level[] randomizedLevels;

    static int currentRandomLevel;

    public static Level[] AllLevels
    {
        get
        {
            if (cachedLevels == null)
            {
                cachedLevels = new[] 
                    {
                        new Level { SceneName = "Level-010-City", Title = "City", BackgroundImage = "back_city" },
                        new Level { SceneName = "Level-020-Forest", Title = "Forest", BackgroundImage = "back_forest" },
                        new Level { SceneName = "Level-030-Snow", Title = "Snow", BackgroundImage = "back_snow" },
                        new Level { SceneName = "Level-040-Desert", Title = "Desert", BackgroundImage = "back_desert" },
                        new Level { SceneName = "Level-050-Dungeon", Title = "Dungeon", BackgroundImage = "back_dungeon" },
                        new Level { SceneName = "Level-060-Ocean", Title = "Ocean", BackgroundImage = "back_ocean" },
                        new Level { SceneName = "Level-070-Bros", Title = "Bros", BackgroundImage = "back_bros" },
                        new Level { SceneName = "Level-080-Space", Title = "Space", BackgroundImage = "back_space" },
                        new Level { SceneName = "Level-090-Ponies", Title = "Ponies", BackgroundImage = "back_ponies" },
                        new Level { SceneName = "Level-100-Arcade", Title = "Arcade", BackgroundImage = "back_arcade" },
                    };
            }

            return cachedLevels;
        }
    }

    public static void Randomize()
    {
        randomizedLevels = AllLevels.OrderBy(x => UnityEngine.Random.value).ToArray();
        currentRandomLevel = 0;
    }

    public static Level NextRandomized()
    {
        if (randomizedLevels == null)
        {
            Randomize();
        }
            
        return randomizedLevels[++currentRandomLevel % AllLevels.Length]; 
    }

    public static Level Next()
    {
        return AllLevels[++currentRandomLevel % AllLevels.Length];
    }
}
