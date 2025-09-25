using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private SpawnPoint[] spawnPoints;

    [Header("Runtime Watcher")]
    [SerializeField]
    //[Disable]
    private int currentSpawnPoint = 0;


    public Transform GetCurrentSpawnPoint(){ return spawnPoints[currentSpawnPoint-1].transform; }
    public Transform GetInitialSpawnPoint()
    {
        Transform result = null;
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].IsStartPoint)
            {
                result = spawnPoints[i].transform;
                break;
            }
        }

        return result;
    }

    public Transform GetFinalSpawnPoint()
    {
        Transform result = null;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].IsEndPoint)
            {
                result = spawnPoints[i].transform;
                break;
            }
        }

        return result;
    }

    private void Start ()
    {
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }

    public virtual void OnEnable()
    {
        EventManagerv2.instance.StartListening("SpawnPointReached", UpdateSpawnStatus);
    }

    public virtual void OnDisable()
    {
        if (!EventManagerv2.IsDestroyed)
            EventManagerv2.instance.StopListening("SpawnPointReached", UpdateSpawnStatus);
    }

    public virtual void UpdateSpawnStatus(EventParam eventParam)
    {
        currentSpawnPoint++;
        if (spawnPoints[currentSpawnPoint - 1].IsEndPoint)
        {
            EventManagerv2.instance.TriggerEvent("LevelCompleted");
        }
    }
        
}
