using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public LevelSO currentLevel;
    public void LoadLevel(int index)
    {
        currentLevel = Resources.Load<LevelSO>($"Levels/Level {index}");
    }
}
