using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [Header("RequirementBar")]
    [SerializeField] private TextMeshProUGUI requiremntResult;
    [SerializeField] private TextMeshProUGUI turnLeft;
    [SerializeField] int turnCount;
    [Header("Input")]
    [SerializeField] private TextMeshProUGUI display;
    private string currentExpression;
    public int indexLevel = 1;
    public LevelManager levelManager;
    public UIManager uIManager;
    void Start()
    {
        levelManager = LevelManager.instance;
        uIManager = UIManager.instance;
        levelManager.LoadLevel(indexLevel);
        requiremntResult.text = levelManager.currentLevel.requiremntResult.ToString();
        turnLeft.text = levelManager.currentLevel.turnCount.ToString();
        turnCount = levelManager.currentLevel.turnCount;
         uIManager.nextLevel.onClick.AddListener(NextLevel);
         uIManager.retry.onClick.AddListener(Retry);

    }
    public void OnNumberPressed(string number)
    {
        if (turnCount <= 0) return;
        currentExpression += number;
        turnCount--;
        UpdateDisplay();
        if (turnCount == 0)
        {
            FinalResult();
        }
        if (levelManager.currentLevel.oneTimeClick)
        {
            Button pressedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
            pressedButton.interactable = false;
        }
    }
    public void OnOperation(string op)
    {
        if (turnCount <= 0) return;
        if (string.IsNullOrEmpty(currentExpression))
        {
            if (op == "√")
            {
                currentExpression = "√";
                turnCount--;
            }
            else if (op == "-")
            {
                currentExpression = "-";
                turnCount--;
            }
        }
        else
        {
            currentExpression += op;
            turnCount--;
        }

        UpdateDisplay();

        if (turnCount == 0)
        {
            FinalResult();
        }
        if (levelManager.currentLevel.oneTimeClick)
        {
            Button pressedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
            pressedButton.interactable = false;
        }
    }
    #region Special Expression
    private string SolvePowerExpression(string expression)
    {
        Regex powerRegex = new Regex(@"(\d+)\s*\^\s*(\d+)");
        while (powerRegex.IsMatch(expression))
        {
            expression = powerRegex.Replace(expression, math =>
            {
                double baseNume = Convert.ToDouble(math.Groups[1].Value);
                double exponent = Convert.ToDouble(math.Groups[2].Value);
                return Math.Pow(baseNume, exponent).ToString();
            });
        }
        return expression;
    }
    private string SolveSquareRootExpression(string expression)
    {
        Regex sqrtRegx = new Regex(@"√(\d+)");
        while (sqrtRegx.IsMatch(expression))
        {
            expression = sqrtRegx.Replace(expression, match =>
            {
                double num = Convert.ToDouble(match.Groups[1].Value);
                return Math.Sqrt(num).ToString();
            });
        }
        return expression;
    }
    private string SolveFactorialExpression(string expression)
    {
        Regex factorialRegex = new Regex(@"(\d+)!");

        while (factorialRegex.IsMatch(expression))
        {
            expression = factorialRegex.Replace(expression, match =>
            {
                int num = Convert.ToInt32(match.Groups[1].Value);
                return Factorial(num).ToString();
            });
        }
        return expression;
    }
    #endregion
    // private bool IsLasstCharOperator()
    // {
    //     char lastChar = currentExpression[currentExpression.Length - 1];
    //     return lastChar == '+' || lastChar == '-' || lastChar == '*' || lastChar == '/' || lastChar == '^' || lastChar == '!';
    // }
    private void UpdateDisplay()
    {
        display.text = string.IsNullOrEmpty(currentExpression) ? "0" : currentExpression;
        turnLeft.text = turnCount.ToString();
    }
    public void FinalResult()
    {
        if (!string.IsNullOrEmpty(currentExpression))
        {
            try
            {
                string processedExpression = SolveFactorialExpression(currentExpression);
                processedExpression = SolvePowerExpression(processedExpression);
                processedExpression = SolveSquareRootExpression(processedExpression);


                object result = new System.Data.DataTable().Compute(processedExpression, null);


                if (double.TryParse(result.ToString(), out double finalResult))
                {
                    display.text += "=" + finalResult.ToString();
                    Debug.Log("Final result" + finalResult);
                    if (finalResult == levelManager.currentLevel.requiremntResult)
                    {
                         uIManager.SetActiveWinUI();
                    }
                    else
                    {
                         uIManager.SetActiveLoseUI();
                    }
                }
                else
                {
                    display.text = "Error";
                }
            }
            catch (Exception)
            {
                display.text = "No Result";
                 uIManager.SetActiveLoseUI();
            }
        }
    }
    private long Factorial(int n)
    {
        if (n == 0 || n == 1) return 1;
        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }
    public void NextLevel()
    {

        indexLevel++;
        levelManager.LoadLevel(indexLevel);
        currentExpression = "";
        display.text = currentExpression.ToString();
        turnCount = levelManager.currentLevel.turnCount;
        Debug.Log(levelManager.currentLevel.requiremntResult);
        requiremntResult.text = levelManager.currentLevel.requiremntResult.ToString();
        turnLeft.text = levelManager.currentLevel.turnCount.ToString();
         uIManager.SetInteractable();
         uIManager.ResetActive();
    }
    public void Retry()
    {
        levelManager.LoadLevel(indexLevel);
        currentExpression = "";
        display.text = currentExpression.ToString();
        turnCount = levelManager.currentLevel.turnCount;
        requiremntResult.text = levelManager.currentLevel.requiremntResult.ToString();
        turnLeft.text = levelManager.currentLevel.turnCount.ToString();
         uIManager.SetInteractable();
         uIManager.ResetActive();
    }
}
