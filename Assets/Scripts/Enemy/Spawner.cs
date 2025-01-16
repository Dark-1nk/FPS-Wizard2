using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // Prefab to be spawned
    private Transform spawnPoint; // The location to spawn the prefab
    public float spawnInterval = 60f; // Time interval between spawns

    private void Start()
    {
        spawnPoint = gameObject.transform;
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Spawner: No prefab assigned to spawn.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawner: No spawn point assigned. Defaulting to spawner's position.");
            spawnPoint = transform;
        }

        StartCoroutine(SpawnPrefab());
    }

    private IEnumerator SpawnPrefab()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Spawn()
    {
        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
    }
}
