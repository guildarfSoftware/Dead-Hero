using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] int firstLevel = 1;
    [SerializeField] Transitioner transitioner;

    public void StartGame()
    {
        StartCoroutine(LoadFirstLevel());
    }

    IEnumerator LoadFirstLevel()
    {
        DontDestroyOnLoad(transitioner);
        DontDestroyOnLoad(this);
        GetComponent<Canvas>().enabled=false;
        yield return transitioner.TransitionOut(0);
        yield return SceneManager.LoadSceneAsync(firstLevel);
        yield return transitioner.TransitionIn(0.6f);
        Destroy(transitioner);
        Destroy(gameObject);
    }


}
