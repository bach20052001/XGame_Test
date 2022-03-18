using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Coin coin;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject winnerText;
    [SerializeField] private GameObject currentLife;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private Button exit;
    [SerializeField] private Button replay;
    [SerializeField] private TextMeshProUGUI Level;



    private int life;
    // Start is called before the first frame update
    void Start()
    {
        life = 3;

        exit.onClick.AddListener(ExitGame);
        replay.onClick.AddListener(ReplayGame);

        this.RegisterListener(GameEvent.OnCountdown, OnCountdownHandler);
        this.RegisterListener(GameEvent.OnFinishCoundown, OnCountdownFinishHandler);
        this.RegisterListener(GameEvent.OnFinishLevel, OnFinishLevelHandler);
        this.RegisterListener(GameEvent.OnColliderObstacle, OnColliderObstacleHandler);
        this.RegisterListener(GameEvent.OnGameOver, OnGameOverHandler);

    }

    private void ReplayGame()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.ReloadScene();
        }
    }

    private void ExitGame()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.PreviousScene();
        }
    }

    private void OnGameOverHandler(object obj)
    {
        gameoverPanel.SetActive(true);
    }

    private void OnColliderObstacleHandler(object obj)
    {
        life--;
        currentLife.transform.GetChild(life).gameObject.SetActive(false); 
    }

    private void OnFinishLevelHandler(object obj)
    {
        winnerText.SetActive(true);
    }

    private void OnCountdownFinishHandler(object obj)
    {
        countdownText.gameObject.SetActive(false);
    }

    private void OnCountdownHandler(object obj)
    {
        Level.text = "Level " + PlayerDataManager.levelDisplay.ToString();
        StartCoroutine(levelDisplay());
        countdownText.text = ((int)obj).ToString();
    }

    private IEnumerator levelDisplay()
    {
        Level.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        Level.gameObject.SetActive(false);
    }
}
