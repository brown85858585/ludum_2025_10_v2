
//====================================================================================================
//
// Example:
//
// Spawn an object examples:
// GameObject enemy1 =PoolSystem.instance.Spawn("Enemy Ship 1", pos, rot); // using tags (not used in Player) because dont allow 'growing' the pool.
// GameObject enemy1 =PoolSystem.instance.Spawn(enemyPrefab, pos, rot); // using prefab gameObject (declared in your script to drag a prefab in the Inspector)
//
//  Despawn Examples:
//  Mode v1 --> Despawn (enemy1); // Not used in Player, because all object has an AutoDisableTimed script attached.
//  Mode v1 --> enemy1.SetActive(false); or using AutoDisableTimed script.
//
//====================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public GameObject objectToPool;
    public string tag = string.Empty;
    public int size = 5;
    public bool willGrow = true;

    // Constructor (not necessary, but helpful)
    public Pool(GameObject _obj, string _tag, int _size, bool _willGrow)
    {
        objectToPool = _obj;
        tag = _tag;
        size = _size;
        willGrow = _willGrow;
    }
}

public class PoolSystem : Singletone<PoolSystem>
{

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;
    [Tooltip("Show all 'Debug.Log' spawning messages.")]
    [ShowWhen("showDebug")]
    public bool showSpawnDebug = false;
    [Tooltip("Show all 'Debug.Log' OnDestroy pool objets messages.")]
    [ShowWhen("showDebug")]
    public bool showDestroyDebug = false;

    [Header("Pool")]
    public List<Pool> pools;


    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private bool isCreatedInRuntime = false;        // Flag to know if this pool is being creating in runtime.

    // Initialize the necesary objects
    private void Init()
    {
        if (poolDictionary == null)
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
        }

        // Check if pools is null (it will happen if we are creating the poolsystem in runtime.
        if (pools == null)
        {
            pools = new List<Pool>();
        }
    }

    // Create (instantiate) the objects that will be used by the pool and update the dictionary.
    private void InstatiateObjects(Pool pool)
    {
        if (poolDictionary.ContainsKey(tag)) return; // Security sentence. Do NOT create objects if the Tag already exists 
                                                     // (can happen when creating the pool system in runtime).

        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.size; i++)
        {
            GameObject obj = (GameObject)Instantiate(pool.objectToPool);
            obj.transform.parent = this.transform;
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
        poolDictionary.Add(pool.tag, objectPool);
    }


    void Start()
    {
        Init();

        // Dont initialize the pool objects if we are creating this pool system in runtime (because there will be empty) .
        if (!isCreatedInRuntime)
        {
            foreach (Pool pool in pools)
            {
                if (pool.tag == string.Empty || pool.tag == "") pool.tag = pool.objectToPool.name;
                InstatiateObjects(pool);
            }
        }
    }

    // Destroy all the objects in the pool.
    private void OnDestroy()
    {
        for (int i = 0; i < pools.Count; i++)
        {
            if (showDebug && showDestroyDebug) Debug.Log("PoolSystem -> OnDestroy -> Destroying Pool. Tag: " + pools[i].tag + "Count: " + poolDictionary[pools[i].tag].Count.ToString());
            while (poolDictionary[pools[i].tag].Count > 0)
            {
                GameObject _obj = poolDictionary[pools[i].tag].Dequeue();
                if (showDebug && showDestroyDebug) Debug.Log("PoolSystem -> OnDestroy -> Destroying Pool. Tag: " + pools[i].tag + "Obj: " + _obj.name);
                Destroy(_obj);
            }
        }

        if (showDebug && showDestroyDebug) Debug.Log("PoolSystem -> OnDestroy -> Pool Destroyed");
    }


    private int SearchByObjectInPool(GameObject _obj)
    {
        int index = -1;
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].objectToPool == _obj)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    private int SearchByTagInPool(string _tag)
    {
        int index = -1;
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].tag.Contains(_tag))
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public GameObject Spawn(GameObject objectToPool, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary == null) { isCreatedInRuntime = true; Init(); } // Security Sentence (just in case we create the pool in runtime).

        // Search in the pool for that object and get its tag.
        //string tag = string.Empty;

        // Search that object in the pool
        int index = SearchByObjectInPool(objectToPool);

        if (index >= 0) // Object exists
        {
            if (showDebug && showSpawnDebug) Debug.Log("PoolSystem -> Spawn(Obj) -> The Object " + objectToPool.name + " is in the pool with tag: " + pools[index].tag);
            return Spawn(pools[index].tag, position, rotation);
        }
        else // The object doesn't exit. Instantiate this object and get it into the pool in runtime.
        {
            if (showDebug && showSpawnDebug) Debug.Log("PoolSystem -> Spawn(Obj) -> Can't find this Object " + objectToPool.name + " in the pool.");

            Debug.LogWarning("PoolSystem WARNING!!! You are trying to spawn an object (" + objectToPool.name + ") that doesn't exist in the pool. It is very expensive!!! Please remember to add it in your scene.");
            Pool _pool = new Pool(objectToPool, objectToPool.name, 1, true);
            pools.Add(_pool);
            InstatiateObjects(_pool);

            string _tag = objectToPool.name;
            return Spawn(_tag, position, rotation);
        }

    }

    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary == null) { isCreatedInRuntime = true; Init(); } // Security Sentence (just in case we create the pool in runtime).

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + "doesn't exist.");
            return null;
        }

        // Search for a object in the queue.
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // if the object is already active and the pool can grow, we make a new object, add to the pool and use ot.
        // if not, we just use that actie object in an eternal cycle.
        if (objectToSpawn.activeSelf)
        {
            if (showDebug && showSpawnDebug) Debug.Log("PoolSystem -> Spawn(Tag) -> The Object " + objectToSpawn.name + " is active. Trying to create another more.");

            // Search for the pool that contains that tag.
            int index = SearchByTagInPool(tag);

            if (showDebug && showSpawnDebug) Debug.Log("PoolSystem -> Spawn(Tag) -> Searching by tag: " + tag);

            //  Make sure we have found the correct pool index searching by tag.
            if (index >= 0)
            {
                if (showDebug && showSpawnDebug) Debug.Log("PoolSystem -> Spawn(Tag) -> Searching by tag sucessfull. Pool Index: " + index.ToString());

                // See if in the pools's data is set to grow the queue
                if (pools[index].willGrow)
                {
                    poolDictionary[tag].Enqueue(objectToSpawn); // Insert the previous object that was active

                    // Create a new object to use and get in into the pool too.
                    objectToSpawn = Instantiate(pools[index].objectToPool);
                    objectToSpawn.transform.parent = this.transform;
                    objectToSpawn.SetActive(true);
                }
            }
        }

        // Use the object.
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    public void Despawn(GameObject objToDespawn)
    {
        objToDespawn.SetActive(false);
    }
}



