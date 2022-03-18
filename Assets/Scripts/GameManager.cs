using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private int countdownTime = 3;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
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

        countdownTime = 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.RegisterListener(GameEvent.OnFinishLevel, NextLevelHandler);
        StartCoroutine(CountDownToPlayGame());
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.RegisterEvent();
        }
    }

    private void NextLevelHandler(object obj)
    {
        SceneController.Instance.ReloadScene();
    }

    private IEnumerator CountDownToPlayGame()
    {
        while (countdownTime > 0)
        {
            countdownTime--;
            yield return new WaitForSeconds(1f);
            this.PostEvent(GameEvent.OnCountdown, countdownTime);
        }

        this.PostEvent(GameEvent.OnFinishCoundown);
        yield break;
    }
}
