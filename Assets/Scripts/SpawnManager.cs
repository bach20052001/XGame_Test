using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Star,
    Powerup,
    Obstacle
}

public class SpawnManager : MonoBehaviour
{
    //[SerializeField] private ObjectPooling obstacles;
    //[SerializeField] private ObjectPooling stars;
    //[SerializeField] private ObjectPooling powerups;

    //[SerializeField] private GameObject[] points;

    [SerializeField] private GameObject player;

    [SerializeField] private GameObject crashObstacle;

    [SerializeField] private GameObject claimReward;


    private void Start()
    {
        this.RegisterListener(GameEvent.OnColliderObstacle, OnColliderObstacleHandler);
        this.RegisterListener(GameEvent.OnClaimStar, OnClaimStarHandler);

    }

    private void OnClaimStarHandler(object obj)
    {
        Instantiate(claimReward, (Vector3)obj + Vector3.up * 2.5f, Quaternion.identity);
    }

    private void OnColliderObstacleHandler(object obj)
    {
        Instantiate(crashObstacle, (Vector3)obj + Vector3.up * 2.5f, Quaternion.identity);
    }

}
