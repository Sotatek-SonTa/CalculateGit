using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObject/LevelSO")]
public class LevelSO : ScriptableObject {
    public int clickCount;
    public int operationClickCount;
    public string hintFormula;
    public int hintCount;
    public List<int> operationHideIndex;
}

