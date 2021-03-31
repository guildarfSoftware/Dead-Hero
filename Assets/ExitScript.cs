using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{


    [SerializeField] int nextLevel;
    private void OnCollisionEnter2D(Collision2D other)
    {

        LevelManager manager = FindObjectOfType<LevelManager>();
        if (manager != null)
        {
            manager.NextLevel(nextLevel);
        }
    }

}
