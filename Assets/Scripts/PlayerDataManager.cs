using System.Collections;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static int numberOfCoins = 0;
    public static int level = 0;
    public static int levelDisplay = 1;

    // Singleton
    private static PlayerDataManager instance;


    public static PlayerDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerDataManager>();
            }
            return instance;
        }
    }
    //======

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (this != instance)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadPlayerData()
    {

    }

    public void SavePlayerData()
    {

    }

    public void IncreaseLevel()
    {
        ++levelDisplay;
        //++level;
    }

    public void ResetLevel()
    {
        level = 0;
    }
}
