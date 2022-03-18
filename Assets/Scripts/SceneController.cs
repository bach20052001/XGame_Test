using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Animator startBTN;
    [SerializeField] private Animator gameName;

    const string Animation_OUT = "TransitionOut";

    // Singleton
    private static SceneController instance;

    public static SceneController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneController>();
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

    private void Start()
    {
    }

    public void TransitionOut()
    {
        if (startBTN != null && gameName != null)
        {
            startBTN.SetBool(Animation_OUT, true);
            gameName.SetBool(Animation_OUT, true);
        } 
    }

    public void NextScene()
    {
        StartCoroutine(NextSceneDelay(0.75f));
    }

    private IEnumerator NextSceneDelay(float timeInterval)
    {
        TransitionOut();
        yield return new WaitForSeconds(timeInterval);
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene < SceneManager.sceneCount)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(currentScene + 1);
            while (!ao.isDone)
            {
                yield return null;
            }
        }
    }

    public void ReloadScene()
    {
        StartCoroutine(ReloadSceneDelay());
    }

    private IEnumerator ReloadSceneDelay()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        AsyncOperation ao = SceneManager.LoadSceneAsync(currentScene);
        while (!ao.isDone)
        {
            yield return null;
        }
    }

    public void PreviousScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene > 0)
        {
            SceneManager.LoadSceneAsync(currentScene - 1);
        }
    }
}
