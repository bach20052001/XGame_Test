using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    Button startBTN;
    // Start is called before the first frame update
    void Start()
    {
        startBTN = this.GetComponent<Button>();
        startBTN.onClick.AddListener(NextScene);
    }

    private void NextScene()
    {
        SceneController.Instance.NextScene();
    }
}
