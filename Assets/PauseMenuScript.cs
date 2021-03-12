using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] GameObject menu;

    bool menuIsActive;
    // Start is called before the first frame update
    void Start()
    {

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuIsActive) Continue();
            else Pause();
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
        menuIsActive = true;
    }

    public void Continue()
    {
        menu.SetActive(false);
        menuIsActive = false;
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }


}
