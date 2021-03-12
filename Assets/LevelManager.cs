using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    int currentLevel;
    [SerializeField] int nextLevel;
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Health>().OnDeath += RestartLevel;

    }

    IEnumerator StartTransition()
    {
        Transitioner transitioner = FindObjectOfType<Transitioner>();

        yield return transitioner.TransitionOut(0);
        yield return new WaitForSeconds(0.2f);
        yield return transitioner.TransitionIn(0.6f);

    }
    IEnumerator LoadScene(int sceneIndex, float waitTime = 0)
    {
        yield return new WaitForSeconds(waitTime);
        Transitioner transitioner = FindObjectOfType<Transitioner>();
        DontDestroyOnLoad(transform.parent);
        yield return transitioner.TransitionOut(0.4f);
        yield return SceneManager.LoadSceneAsync(sceneIndex);
        yield return transitioner.TransitionIn(0.6f);
        Destroy(transform.parent.gameObject);
    }

    public void RestartLevel()
    {
        StartCoroutine(LoadScene(currentLevel, 0.5f));
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
