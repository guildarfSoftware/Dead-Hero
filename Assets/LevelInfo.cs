using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    [SerializeField] Transform startPointTransform;
    public Vector3 startPoint { get => startPointTransform == null ? Vector3.zero : startPointTransform.position; }
    [SerializeField] int levelIndex;
    public int LevelIndex { get => levelIndex; }
}
