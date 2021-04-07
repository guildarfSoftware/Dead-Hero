using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretRoomWalls : MonoBehaviour
{
    bool needUpdate;
    bool _wallIsVisible;

    TilemapRenderer _renderer;

    [SerializeField]
    bool wallIsVisible
    {
        get
        {
            return _wallIsVisible;
        }
        set
        {
            needUpdate = true;
            _wallIsVisible = value;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        wallIsVisible = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        wallIsVisible = true;
    }

    private void Start()
    {
        _renderer = GetComponent<TilemapRenderer>();
    }

    private void Update()
    {
        if (needUpdate)
        {
            needUpdate = false;
            _renderer.enabled = wallIsVisible; 
        }
    }

}
