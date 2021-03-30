using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    int currentLevel;
    [SerializeField] int nextLevel;
    static int firstLevelIndex=1;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) player.GetComponent<Health>().OnDeath += RestartLevel;
        currentLevel = SceneManager.GetActiveScene().buildIndex;

    }

    public void NextLevel()
    {
        StartCoroutine(LoadScene(nextLevel, 0, 0.6f));
    }

    IEnumerator StartTransition()
    {
        Transitioner transitioner = FindObjectOfType<Transitioner>();

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
        Transitioner transitioner = FindObjectOfType<Transitioner>();
        DontDestroyOnLoad(transform.parent);
        yield return transitioner.TransitionOut(fadeOutTime);
        yield return SceneManager.LoadSceneAsync(sceneIndex);
        yield return transitioner.TransitionIn(fadeInTime);
        Destroy(transform.parent.gameObject);
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
