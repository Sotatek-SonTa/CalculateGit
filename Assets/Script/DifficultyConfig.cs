using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DifficultySO",menuName ="ScriptableObject/DifficultySO")]
public class DifficultyConfig : ScriptableObject
{
     //min length 
     public int minLength;
     //max Length
     public int maxLength;
     //Các biến bao gồm
     //Độ khó
     public Diffculty diffculty;
     //Các toán tử cho phép
     public List<string> number;
     //Các số cho phép(lúy thừa sẽ cho vào ở đây)
     public List<string> operation;
     //Hint formula
}
public enum Diffculty
{
    Easy =0,
    Medium=1,
    Hard=2,
    Expert=4,
}
