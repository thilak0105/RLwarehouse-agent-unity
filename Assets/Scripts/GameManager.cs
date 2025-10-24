using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WarehouseAgent agent;
    public ParcelSpawner spawner;

    void Start()
    {
        // This can now be simplified, we'll have the agent call it.
        // For safety, we'll leave it here to make sure the first parcel spawns.
        spawner.SpawnParcel();
    }

    public void ResetEpisode()
    {
        // This function should now ONLY spawn a new parcel.
        // The agent's OnEpisodeBegin() will be called automatically by ML-Agents when EndEpisode() runs.
        spawner.SpawnParcel();
    }
}