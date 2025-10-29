using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public GameObject portalPrefab;   // 器呕 橇府普
    private int killedEnemyCount = 0; // 贸摹等 利 荐
    private bool portalSpawned = false;
    private Spawner spawner;

    public int TotalEnemyCount;

    private void Awake()
    {
        portalPrefab.SetActive(false);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        spawner = FindObjectOfType<Spawner>();
    }

    private void Start()
    {
        TotalEnemyCount = spawner.maxSpawnCount * 2;

    }
    public void SetTotalEnemyCount(int count)
    {
        TotalEnemyCount = count;
        killedEnemyCount = 0;
        portalSpawned = false;
    }


    public void OnEnemyKilled()
    {
        killedEnemyCount++;

        if (!portalSpawned && killedEnemyCount >= TotalEnemyCount)
        {
            OpenPortal();
        }
    }

    private void OpenPortal()
    {
        portalSpawned = true;
        portalPrefab.SetActive (true);
    }
}