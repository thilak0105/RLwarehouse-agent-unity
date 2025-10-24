using UnityEngine;

public class ParcelSpawner : MonoBehaviour
{
    // The prefab to be spawned (your green circle)
    public GameObject parcelPrefab;

    // *** MODIFIED: An array to hold all possible spawn points ***
    public Transform[] spawnPoints;

    private GameObject currentParcel;

    // This function is called by the GameManager to spawn a new parcel
    public void SpawnParcel()
    {
        // Destroy the old parcel if it exists
        if (currentParcel != null)
        {
            Destroy(currentParcel);
        }

        // --- Error checks ---
        if (parcelPrefab == null)
        {
            Debug.LogError("ParcelSpawner: 'Parcel Prefab' is not assigned in the Inspector!");
            return;
        }
        // Check if the spawn points array is empty or not set up
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("ParcelSpawner: 'Spawn Points' array is not set up in the Inspector!");
            return;
        }

        // *** NEW: Pick a random spawn point from the array ***
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform randomSpawnPoint = spawnPoints[randomIndex];

        // Spawn the parcel at the chosen random spawn point's position
        currentParcel = Instantiate(parcelPrefab, randomSpawnPoint.position, Quaternion.identity);
    }
}