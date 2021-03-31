using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistebObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject persistentObjectPrefab;
    static bool hasSPawned;
    // Start is called before the first frame update
    void Awake()
    {
        if(hasSPawned) return;

        SpawnPersistentObject();

        hasSPawned = true;
    }

    private void SpawnPersistentObject()
    {
        GameObject persistentObject = Instantiate(persistentObjectPrefab,Vector3.zero,Quaternion.identity);
        DontDestroyOnLoad(persistentObject);
    }
}
