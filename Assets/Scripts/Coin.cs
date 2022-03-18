using System;
using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public int numberOfCoins = 0;
    // Start is called before the first frame update
    void Start()
    {
        coinsText = this.GetComponent<TextMeshProUGUI>();
        this.RegisterListener(GameEvent.OnClaimStar, OnClaimStarHandle);
    }

    private void OnClaimStarHandle(object obj)
    {
        numberOfCoins++;
        Debug.Log(numberOfCoins);
        Debug.Log(coinsText);
        coinsText.SetText($"{numberOfCoins}");
    }
}
