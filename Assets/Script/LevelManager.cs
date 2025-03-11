using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CalculateGameSoundManager;
using CalculateUIManager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using CalculateGameSoundManager;
using System.Text;

namespace CalculateLevelManager
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance { get; private set; }

        [SerializeField] private Transform parentTransform;
        [SerializeField] public TextMeshProUGUI textPrefab;
        [SerializeField] public List<string> formulaSegments;
        [SerializeField] public List<TextMeshProUGUI> generatedTexts;
        [SerializeField] public string currentExpression;
        [SerializeField] CalculateSoundManager calculateSoundManager;
        public UIManager uIManager;
        public int visblaeOperators;
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
            calculateSoundManager = CalculateSoundManager.instance;
            uIManager = UIManager.instance;
        }
        public LevelSO currentLevel;
        public void LoadLevel(int index)
        {
            currentLevel = Resources.Load<LevelSO>($"Levels/Level {index}");
        }
        #region Special Expression
        public string SolvePowerExpression(string expression)
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
        public string SolveFactorialExpression(string expression)
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
        public void GenerateTextWithResult(string expression, double result)
        {
            foreach (var item in generatedTexts)
            {
                Destroy(item.gameObject);
            }
            generatedTexts.Clear();
            formulaSegments.Clear();
            foreach (char c in expression)
            {
                GameObject newTextObject = Instantiate(textPrefab.gameObject, parentTransform);
                TextMeshProUGUI newText = newTextObject.GetComponent<TextMeshProUGUI>();
                newText.text = c.ToString();
                generatedTexts.Add(newText);
                formulaSegments.Add(newText.text);
            }
            GameObject equalTextObject = Instantiate(textPrefab.gameObject, parentTransform);
            TextMeshProUGUI equalText = equalTextObject.GetComponent<TextMeshProUGUI>();
            equalText.text = "=";
            generatedTexts.Add(equalText);
            formulaSegments.Add(equalText.text);

            GameObject resultTextObject = Instantiate(textPrefab.gameObject, parentTransform);
            TextMeshProUGUI resultText = resultTextObject.GetComponent<TextMeshProUGUI>();
            resultText.text = result.ToString();
            generatedTexts.Add(resultText);
            formulaSegments.Add(resultText.text);
        }
        public void GenerateTextFromHint(string hintFormula)
        {
            foreach (var text in generatedTexts)
            {
                Destroy(text.gameObject);
            }
            generatedTexts.Clear();
            formulaSegments.Clear();
            int hideCount = 0;
            visblaeOperators = 0;
            for (int i = 0; i < hintFormula.Length; i++)
            {
                char c = hintFormula[i];
                GameObject newTextObject = Instantiate(textPrefab.gameObject, parentTransform);
                TextMeshProUGUI newText = newTextObject.GetComponent<TextMeshProUGUI>();

                if (char.IsDigit(c))
                {
                    newText.text = "<sprite name=\"blank\">";
                }
                else if ("+-*/^!".Contains(c))
                {
                    if (hideCount < 2)
                    {
                        newText.text = "<sprite name=\"blank\">";
                        hideCount++;
                    }
                    else
                    {
                        newText.text = c.ToString();
                        visblaeOperators++;
                    }
                }
                else
                {
                    newText.text = c.ToString();
                }
                formulaSegments.Add(newText.text);
                generatedTexts.Add(newText);
            }
        }
        public void FinalResult()
        {
            currentExpression = string.Join("", formulaSegments);
            if (!string.IsNullOrEmpty(currentExpression))
            {
                try
                {
                    string processedExpression = SolvePowerExpression(SolveFactorialExpression(currentExpression));
                    object result = new System.Data.DataTable().Compute(processedExpression, null);
                    if (double.TryParse(result.ToString(), out double finalResult))
                    {
                        GenerateTextWithResult(currentExpression, finalResult);
                        if (finalResult == CalculateRequirementResult(currentLevel.hintFormula))
                        {
                            UIManager.instance.SetActiveWinUI();
                            CalculateSoundManager.instance.PlaySound(CalculateSoundName.WIN, 0.2f);
                        }
                        else
                        {
                            UIManager.instance.SetActiveLoseUI();
                            CalculateSoundManager.instance.PlaySound(CalculateSoundName.LOSE, 0.2f);
                        }
                    }
                    else
                    {
                        CalculateSoundManager.instance.PlaySound(CalculateSoundName.LOSE, 0.2f);
                        UIManager.instance.SetActiveLoseUI();
                    }
                }
                catch (Exception)
                {
                    CalculateSoundManager.instance.PlaySound(CalculateSoundName.LOSE, 0.2f);
                    UIManager.instance.SetActiveLoseUI();
                }
            }
        }
        public double CalculateRequirementResult(string hintFormula)
        {
            try
            {
                string processedExpression = SolvePowerExpression(SolveFactorialExpression(currentLevel.hintFormula));
                object result = new System.Data.DataTable().Compute(processedExpression, null);


                if (double.TryParse(result.ToString(), out double finalResult))
                {
                    return finalResult;
                }
                else
                {
                    Debug.LogError("Lỗi khi chuyển đổi kết quả sang double.");
                    return double.NaN;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Lỗi tính toán: " + ex.Message);
                return double.NaN;
            }
        }
        public double? EvaluateExpression(string expression)
        {
            try
            {
                string processedExpression = SolvePowerExpression(SolveFactorialExpression(expression));
                object result = new System.Data.DataTable().Compute(processedExpression, null);
                return Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Lỗi khi tính toán biểu thức: {ex.Message}");
            }
            return null;
        }
        public string BuildExpression(List<string> segments)
        {
            StringBuilder expression = new StringBuilder();

            for (int i = 0; i < segments.Count; i++)
            {
                string current = segments[i];

                if (current == "<sprite name=\"blank\">") continue;

                if (current == "!" && i > 0 && segments[i - 1] != "<sprite name=\"blank\">")
                {
                    expression.Append(current);
                    continue;
                }
                if ("+-*/^".Contains(current))
                {
                    bool hasLeftOperand = (i > 0 && segments[i - 1] != "<sprite name=\"blank\">" && !" +-*/^".Contains(segments[i - 1]));
                    bool hasRightOperand = (i < segments.Count - 1 && segments[i + 1] != "<sprite name=\"blank\">" && !" +-*/^".Contains(segments[i + 1]));

                    if (hasLeftOperand && hasRightOperand)
                    {
                        expression.Append(current);
                    }
                    continue;
                }

                expression.Append(current);
            }

            return expression.ToString();
        }
    }
}

