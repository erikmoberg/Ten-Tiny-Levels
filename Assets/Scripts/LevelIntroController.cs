using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class LevelIntroController : MonoBehaviour {

    public static float GetWaitTimeUntilSpawnSeconds()
    {
        if (GameState.GameMode == GameMode.SinglePlayer || GameState.GameMode == GameMode.TwoPlayerCoop)
        {
            return MagicNumbers.WaitTimeUntilSpawnSeconds;
        }

        return 0;
    }

	void Start () {        
        var textObject = gameObject.GetComponentsInChildren<Text>().FirstOrDefault();
        textObject.text = "LEVEL " + (GameState.CurrentLevel + 1) + "\r\n" + LevelRepository.AllLevels[GameState.CurrentLevel].Title.ToUpperInvariant();
        Destroy(gameObject, GetWaitTimeUntilSpawnSeconds());
    }
}
