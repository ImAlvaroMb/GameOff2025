using UnityEngine;

public class Persistance : MonoBehaviour
{
    private const string LEVEL_KEY = "CuerrrentMaxLevel";

    public void UnlockLevel(int level)
    {
        if(level > PlayerPrefs.GetInt(LEVEL_KEY, 0))
        {
            PlayerPrefs.SetInt(LEVEL_KEY, level);
            PlayerPrefs.Save();
        }
    }

    public void UnlockAll()
    {
        PlayerPrefs.SetInt(LEVEL_KEY, 7);
        PlayerPrefs.Save();
    }
}
