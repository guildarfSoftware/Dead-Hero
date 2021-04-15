using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject persistentObjectPrefab;
    static GameObject _persistentObject;
    // Start is called before the first frame update
    void Awake()
    {
        if(_persistentObject != null) return;

        _persistentObject = SpawnPersistentObject();
    }

    private GameObject SpawnPersistentObject()
    {
        GameObject persistentObject = Instantiate(persistentObjectPrefab,Vector3.zero,Quaternion.identity);
        DontDestroyOnLoad(persistentObject);

        return persistentObject;
    }

    public static void DestroyPersistentObject()
    {
        Destroy(_persistentObject);
    }
}
