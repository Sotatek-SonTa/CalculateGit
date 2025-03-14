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
using CalculateGameplayManager;

namespace CalculateLevelManager
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance { get; private set; }

        [SerializeField] private Transform parentTransform;
        [SerializeField] public TMPrefab textPrefab;
        [SerializeField] public List<string> formulaSegments;
        [SerializeField] public List<TextMeshProUGUI> generatedTexts;
        [SerializeField] public List<TMPrefab> tMPrefabs;
        [SerializeField] public string currentExpression;
        [SerializeField] CalculateSoundManager calculateSoundManager;
        public int indexLevel = 1;
        public UIManager uIManager;
        public int visblaeOperators;
        public string automaticFormula;
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
        public void GenerateTextFromHint(string hintFormula, LevelSO levelSO)
        {
            ResetList();
            int operatorIndex = 0;
            for (int i = 0; i < hintFormula.Length; i++)
            {
                char c = hintFormula[i];
                TMPrefab newTextObject = Instantiate(textPrefab, parentTransform);

                if (char.IsDigit(c))
                {
                    newTextObject.textMesh.text = "<sprite name=\"blank\">";
                }
                else if ("+-*/^!".Contains(c))
                {
                    if (levelSO.operationHideIndex.Contains(operatorIndex))
                    {
                        newTextObject.textMesh.text = "<sprite name=\"blank\">";
                    }
                    else
                    {
                        newTextObject.textMesh.text = c.ToString();
                        visblaeOperators++;
                    }
                    operatorIndex++;
                }
                else
                {
                    newTextObject.textMesh.text = c.ToString();
                }
                tMPrefabs.Add(newTextObject);
                formulaSegments.Add(newTextObject.textMesh.text);
                generatedTexts.Add(newTextObject.textMesh);
            }
        }
        public void GenerateTextFromAutomatic(string formula, DifficultyConfig difficultyConfig)
        {
            ResetList();
            for (int i = 0; i < formula.Length; i++)
            {
                char c = formula[i];
                TMPrefab newTextObject = Instantiate(textPrefab, parentTransform);
                if (char.IsDigit(c))
                {
                    newTextObject.textMesh.text = "<sprite name=\"blank\">";
                }
                else if ("+-*/^!".Contains(c))
                {
                    if (UnityEngine.Random.value < difficultyConfig.hideOperatosChance)
                    {
                        newTextObject.textMesh.text = "<sprite name=\"blank\">";
                    }
                    else
                    {
                        newTextObject.textMesh.text = c.ToString();
                        visblaeOperators++;
                    }
                }
                tMPrefabs.Add(newTextObject);
                formulaSegments.Add(newTextObject.textMesh.text);
                generatedTexts.Add(newTextObject.textMesh);
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
                        if (finalResult == CalculateRequirementResult(indexLevel < 100 ? currentLevel.hintFormula : automaticFormula))
                        {
                            UIManager.instance.SetActiveWinUI();
                            CalculateSoundManager.instance.PlaySound(CalculateSoundName.WIN, 0.2f);
                        }
                        else
                        {
                            UIManager.instance.SetActiveLoseUI(finalResult.ToString());
                            CalculateSoundManager.instance.PlaySound(CalculateSoundName.LOSE, 0.2f);
                        }
                    }
                    else
                    {
                        CalculateSoundManager.instance.PlaySound(CalculateSoundName.LOSE, 0.2f);
                        UIManager.instance.SetActiveLoseUI(finalResult.ToString());
                    }
                }
                catch (Exception)
                {
                    CalculateSoundManager.instance.PlaySound(CalculateSoundName.LOSE, 0.2f);
                    UIManager.instance.SetActiveLoseUI("No Result");
                }
            }
        }
        public double CalculateRequirementResult(string hintFormula)
        {
            try
            {
                string processedExpression = SolvePowerExpression(SolveFactorialExpression(hintFormula));
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

                    if ((current == "+" || current == "-") && i == 0 && hasRightOperand)
                    {
                        expression.Append(current);
                        continue;
                    }
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
        public string AutomaticGenerateLevel(DifficultyConfig difficultyConfig)
        {
            StringBuilder formula = new StringBuilder();
            int length = UnityEngine.Random.Range(difficultyConfig.minLength, difficultyConfig.maxLength);
            List<string> number = difficultyConfig.number;
            List<string> operators = difficultyConfig.operation;
            bool isExpert = difficultyConfig.diffculty == Diffculty.Expert;

            int numberCount = 0;
            bool lastWasOperator = false;
            bool lastWasFactorial = false;

            for (int i = 0; i < length; i++)
            {
                // Xử lý vị trí cuối cùng
                if (i == length - 1)
                {
                    AppnedLastCharcater(formula, isExpert, number);
                }
                // Không cho phép '!' ở vị trí áp chót
                else if (i == length - 2)
                {
                    ApppendSecondLastCharacaters(formula, ref numberCount, ref lastWasOperator, ref lastWasFactorial, number, operators);
                }
                // Nếu đã có 2 số liên tiếp → thêm toán tử (hoặc '!' nếu là Expert)
                else if (numberCount == 2)
                {
                    AppendFactorialOperator(formula, ref lastWasOperator, ref lastWasFactorial, number, operators);
                    numberCount = 0;
                }
                // Sau toán tử, bắt buộc phải là số
                else if (lastWasOperator || lastWasFactorial)
                {
                    AppendAfterOperatorOrFactorial(formula, ref numberCount, ref lastWasOperator, ref lastWasFactorial, number, operators);
                }
                // Sau "!" bắt buộc là toán tử
                else
                {
                    if (i == 0)
                    {
                        formula.Append(RandomChoice(number));
                        numberCount++;
                    }
                    else
                    {
                        AppendNumberOrOperator(formula, ref numberCount, ref lastWasOperator, ref lastWasFactorial, number, operators);
                    }
                }
            }
            return formula.ToString();
        }
        #region HandleSpecialCondition
        private void AppnedLastCharcater(StringBuilder fomula, bool isExpert, List<string> number)
        {
            if (isExpert && char.IsDigit(fomula[fomula.Length - 1]) && UnityEngine.Random.value < 0.1f)
            {
                fomula.Append("!");
            }
            else
            {
                fomula.Append(RandomChoice(number));
            }
        }
        private void AppendNumberOrOperator(StringBuilder formula, ref int numberCount, ref bool lastWasOperator, ref bool lastWasFactorial, List<string> number, List<string> operators)
        {
            if (UnityEngine.Random.value < 0.4f)
            {
                formula.Append(RandomChoice(number));
                numberCount++;
            }
            else
            {
                if (UnityEngine.Random.value < 0.2f)
                {
                    formula.Append("!");
                    numberCount = 0;
                    lastWasFactorial = true;
                }
                else
                {
                    formula.Append(RandomChoice(operators));
                    numberCount = 0;
                    lastWasOperator = true;
                }
            }
        }
        private void ApppendSecondLastCharacaters(StringBuilder formula, ref int numberCount, ref bool lastWasOperator, ref bool lastWasFactorial, List<string> number, List<string> operators)
        {
            if (formula.Length > 0 && formula[formula.Length - 1] == '!')
            {
                formula.Append(RandomChoice(operators)); // Sau '!' luôn phải là toán tử
                lastWasOperator = true;
                lastWasFactorial = false;
            }
            else
            {
                if (numberCount < 2)
                {
                    formula.Append(RandomChoice(number));
                    numberCount++;
                    lastWasOperator = false;
                    lastWasFactorial = false;
                }
                else
                {
                    formula.Append(RandomChoice(operators));
                    lastWasOperator = true;
                    lastWasFactorial = false;
                }
            }
        }
        private void AppendAfterOperatorOrFactorial(StringBuilder formula, ref int numberCount, ref bool lastWasOperator, ref bool lastWasFactorial, List<string> number, List<string> operators)
        {
            if (lastWasOperator)
            {
                formula.Append(RandomChoice(number));
                numberCount++;
                lastWasOperator = false;
                lastWasFactorial = false;
            }
            // Sau "!" bắt buộc là toán tử
            else if (lastWasFactorial)
            {
                formula.Append(RandomChoice(operators));
                lastWasOperator = true;
                lastWasFactorial = false;
            }
        }
        private void AppendFactorialOperator(StringBuilder formula, ref bool lastWasOperator, ref bool lastWasFactorial, List<string> number, List<string> operators)
        {

            if (UnityEngine.Random.value < 0.2f)
            {
                formula.Append("!");
                lastWasFactorial = true;
            }
            else
            {
                formula.Append(RandomChoice(operators));
                lastWasOperator = true;
            }

        }
        #endregion
        private static string RandomChoice(List<string> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        private void ResetList()
        {
            foreach (var text in generatedTexts)
            {
                Destroy(text.gameObject);
            }
            foreach (var item in tMPrefabs)
            {
                Destroy(item.gameObject);
            }
            formulaSegments.Clear();
            generatedTexts.Clear();
            tMPrefabs.Clear();
            visblaeOperators = 0;
        }
    }
}

