using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    static int mainMenuIndex = 0;
    static int firstLevelIndex = 1;
    int currentLevel = -1;

    GameObject player;
    Health playerHealth;
    public LevelInfo LevelInfo { get; private set; }
    [SerializeField] Transitioner transitioner;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            playerHealth.OnDeath += RestartLevel;
        }
        UpdateLevelInfo();
    }

    public void NextLevel(int nextLevelIndex)
    {
        StartCoroutine(LoadScene(nextLevelIndex, 0.3f, 0.6f));
    }

    IEnumerator StartTransition()
    {

        yield return transitioner.TransitionOut(0);
        yield return new WaitForSeconds(0.2f);
        yield return transitioner.TransitionIn(0.6f);

    }

    public void StartFirstLevel()
    {
        StartCoroutine(LoadScene(firstLevelIndex, 0, 0.6f));

    }

    IEnumerator LoadScene(int sceneIndex, float fadeOutTime, float fadeInTime, float waitTime = 0)
    {
        yield return new WaitForSeconds(waitTime);
        yield return transitioner.TransitionOut(fadeOutTime);
        yield return SceneManager.LoadSceneAsync(sceneIndex);

        if (sceneIndex == mainMenuIndex)
        {
            PersistentObjectSpawner.DestroyPersistentObject();
            yield break;
        }
        else
        {
            UpdateLevelInfo();
            if (playerHealth.IsDead) player.GetComponent<PlayerController>().Initialize();
            player.transform.position = (LevelInfo != null) ? LevelInfo.startPoint : Vector3.zero;
        }
        yield return new WaitForSeconds(0.2f);
        yield return transitioner.TransitionIn(fadeInTime);
    }

    private void UpdateLevelInfo()
    {
        LevelInfo[] activeLevelInfos = FindObjectsOfType<LevelInfo>();

        if (activeLevelInfos.Length == 1)
        {
            LevelInfo = activeLevelInfos[0];
            currentLevel = activeLevelInfos[0].LevelIndex;
            return;
        }

        foreach (var info in activeLevelInfos)
        {
            if (info.LevelIndex != currentLevel)
            {
                LevelInfo = info;
                currentLevel = info.LevelIndex;
                return;
            }
        }

        LevelInfo = null;
    }

    public void RestartLevel()
    {
        StartCoroutine(LoadScene(currentLevel, 0.4f, 0.6f, 0.5f));
    }

    private void OnDisable()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<Health>().OnDeath -= RestartLevel;
        }
    }
}
