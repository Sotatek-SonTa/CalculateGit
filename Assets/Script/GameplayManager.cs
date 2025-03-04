using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CalculateLevelManager;
using CalculateUIManager;
using CalculateGameSoundManager;
namespace CalculateGameplayManager
{
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
        public CalculateSoundManager calculateSoundManager;
        private Dictionary<Button, int> buttonClickCounts = new Dictionary<Button, int>();
        private Dictionary<Button, int> operationClikCount = new Dictionary<Button, int>();
        private int hintIndex = 0;
        private int hintClickCount = 0;
        void Start()
        {
            LoadManager();
            levelManager.LoadLevel(indexLevel);
            requiremntResult.text = levelManager.currentLevel.requiremntResult.ToString();
            turnLeft.text = levelManager.currentLevel.turnCount.ToString();
            turnCount = levelManager.currentLevel.turnCount;
            AddListener();
            uIManager.levelTitle.text = levelManager.currentLevel.name;
            hintClickCount = levelManager.currentLevel.hintCount;
            uIManager.hintCount.text = hintClickCount.ToString();
        }
        public void OnNumberPressed(string number)
        {
            if (turnCount <= 0) return;
            calculateSoundManager.PlaySound(CalculateSoundName.CLICK, 0.3f);
            currentExpression += number;
            turnCount--;
            UpdateDisplay();
            if (turnCount == 0)
            {
                FinalResult();
            }
            CountClickTime(buttonClickCounts, levelManager.currentLevel.clickCount);
        }
        void LoadManager()
        {
            levelManager = LevelManager.instance;
            uIManager = UIManager.instance;
            calculateSoundManager = CalculateSoundManager.instance;
        }
        void AddListener()
        {
            uIManager.nextLevel.onClick.AddListener(NextLevel);
            uIManager.retry.onClick.AddListener(Retry);
            uIManager.hintButton.onClick.AddListener(ShowNextHintCharcater);
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
            calculateSoundManager.PlaySound(CalculateSoundName.CLICK, 0.3f);
            UpdateDisplay();

            if (turnCount == 0)
            {
                FinalResult();
            }
            CountClickTime(operationClikCount, levelManager.currentLevel.operationClickCount);
        }
        public void CountClickTime(Dictionary<Button, int> saveClick, int clickCount)
        {
            Button pressedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
            if (pressedButton != null)
            {
                if (!saveClick.ContainsKey(pressedButton))
                {
                    saveClick[pressedButton] = 0;
                }
                saveClick[pressedButton]++;
                if (saveClick[pressedButton] == clickCount)
                {
                    pressedButton.interactable = false;
                }
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
                        if (finalResult == levelManager.currentLevel.requiremntResult)
                        {
                            uIManager.SetActiveWinUI();
                            calculateSoundManager.PlaySound(CalculateSoundName.WIN, 0.2f);
                        }
                        else
                        {
                            uIManager.SetActiveLoseUI();
                            calculateSoundManager.PlaySound(CalculateSoundName.LOSE, 0.2f);
                        }
                    }
                    else
                    {
                        display.text = "Error";
                    }
                }
                catch (Exception)
                {
                    calculateSoundManager.PlaySound(CalculateSoundName.LOSE, 0.2f);
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
            ResetLevel();
        }
        public void Retry()
        {
            ResetLevel();
        }
        public void ResetLevel()
        {
            levelManager.LoadLevel(indexLevel);
            calculateSoundManager.PlaySound(CalculateSoundName.RETRY, 0.2f);
            SetUpUI();
            ResetHint();
            ResetDictionary();
            uIManager.SetInteractable();
            uIManager.ResetActive();
        }
        public void ShowNextHintCharcater()
        {
            calculateSoundManager.PlaySound(CalculateSoundName.RETRY, 0.2f);
            if (hintClickCount == 0) return;
            hintClickCount--;
            if (hintIndex < levelManager.currentLevel.hintFormula.Length)
            {
                uIManager.hintDisplay.text += levelManager.currentLevel.hintFormula[hintIndex];
                uIManager.hintCount.text = hintClickCount.ToString();
                hintIndex++;
            }
        }
        private void ResetHint()
        {
            hintClickCount = levelManager.currentLevel.hintCount;
            uIManager.hintCount.text = hintClickCount.ToString();
            hintIndex = 0;
            uIManager.ResetHintDisPlay();
        }
        private void ResetDictionary()
        {
            buttonClickCounts = new Dictionary<Button, int>();
            operationClikCount = new Dictionary<Button, int>();
        }
        private void SetUpUI()
        {
            currentExpression = "";
            display.text = currentExpression.ToString();
            turnCount = levelManager.currentLevel.turnCount;
            requiremntResult.text = levelManager.currentLevel.requiremntResult.ToString();
            turnLeft.text = levelManager.currentLevel.turnCount.ToString();
            uIManager.levelTitle.text = levelManager.currentLevel.name;
        }
    }
}

