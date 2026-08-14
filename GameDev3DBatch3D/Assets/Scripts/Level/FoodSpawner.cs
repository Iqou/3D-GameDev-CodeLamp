using System.Collections;
using System;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private FoodCrate createPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float RespawnDelay = 5f;
    [SerializeField] private bool respawnEnabled = true;

    private FoodCrate[] crates;

    private void Start()
    {
        if (createPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("FoodSpawner is not properly configured.");
            return;
        }

        crates = new FoodCrate[spawnPoints.Length];
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnAt(i);
        }
    }

    private void SpawnAt(int index)
    {
        Transform point = spawnPoints[index];
        FoodCrate crate = Instantiate(createPrefab, point.position, point.rotation);

        crates[index] = crate;
        crate.OnCollected += HandleCrateCollected;
    }

    private void HandleCrateCollected(FoodCrate crate)
    {
        crate.OnCollected -= HandleCrateCollected;

        int index = Array.IndexOf(crates, crate);
        if (index < 0) return;

        crates[index] = null;

        if (respawnEnabled)
        {
            StartCoroutine(RespawnAfterDelay(index));
        }
    }

    private IEnumerator RespawnAfterDelay(int index)
    {
        yield return new WaitForSeconds(RespawnDelay);
        
        if (crates[index] == null)
        {
            SpawnAt(index);
        }
    }
}
