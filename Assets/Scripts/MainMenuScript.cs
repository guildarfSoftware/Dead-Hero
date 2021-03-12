using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] GameObject persistentObjects;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartGame()
    {
        GameObject.Instantiate(persistentObjects);
        Transitioner transitioner = FindObjectOfType<Transitioner>();

        LevelManager levelManager = FindObjectOfType<LevelManager>();

        levelManager.StartFirstLevel();
    }
}
