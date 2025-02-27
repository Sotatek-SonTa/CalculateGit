using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObject/LevelSO")]
public class LevelSO : ScriptableObject {
    public int requiremntResult;
    public int turnCount;
    public bool oneTimeClick;
    public int clickCount;
    
}

