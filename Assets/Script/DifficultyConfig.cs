using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultySO", menuName = "ScriptableObject/DifficultySO")]
public class DifficultyConfig : ScriptableObject
{
    //Độ dài ngắn nhất
    public int minLength;
    //Độ dài lớn nhất
    public int maxLength;
    //Đô khó
    public Diffculty diffculty;
    //Các số cho phép từ 1 đến 9 bao gồm cả ! là trường hợp đặc biệt
    public List<string> number;
    //Các toán tử
    public List<string> operation;
    //Xác xuất toán tử bị che
    public float hideOperatosChance;
}
public enum Diffculty
{
    Easy = 0,
    Medium = 1,
    Hard = 2,
    Expert = 4,
}
