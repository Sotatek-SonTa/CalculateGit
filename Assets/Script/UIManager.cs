using System.Collections;
using System.Collections.Generic;
using CalculateLevelManager;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace CalculateUIManager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance { get; private set; }
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
        public GameObject winLoseUI;
        public GameObject winUI;
        public GameObject LoseUI;
        public List<Button> listButton;
        public Button nextLevel;
        public Button retry;
        public Button hintButton;
        [Header("Level title")]
        public TextMeshProUGUI levelTitle;
        [Header("Hint")]
        public TextMeshProUGUI hintDisplay;
        public TextMeshProUGUI hintCount;
        [Header("RequirementBar")]
        [SerializeField] private TextMeshProUGUI requiremntResult;
        [SerializeField] private TextMeshProUGUI currentResult;
        [Header("LoseUIComponent")]
        [SerializeField] private TextMeshProUGUI resultText;
        [Header("RepeatUI")]
        [SerializeField] private TextMeshProUGUI numberRepeatCount;
        [SerializeField] private TextMeshProUGUI operationRepeatCount;
        public void SetActiveWinUI()
        {
            winLoseUI.SetActive(true);
            winUI.SetActive(true);
        }
        public void SetActiveLoseUI(string result)
        {
            winLoseUI.SetActive(true);
            LoseUI.SetActive(true);
            UpdateResult(result);
        }
        public void ResetActive()
        {
            winLoseUI.SetActive(false);
            LoseUI.SetActive(false);
            winUI.SetActive(false);
        }
        public void SetInteractable()
        {
            foreach (var item in listButton)
            {
                item.interactable = true;
            }
        }
        public void ResetHintDisPlay()
        {
            hintDisplay.text = "";
        }
        public void SetUpUI(LevelSO currentLevel)
        {
            requiremntResult.text = LevelManager.instance.CalculateRequirementResult(currentLevel.hintFormula).ToString();
            levelTitle.text = currentLevel.name;
            hintCount.text = currentLevel.hintCount.ToString();
            SetUpRepeatAllowance(currentLevel.clickCount,currentLevel.operationClickCount);
        }
        public void SetUpUIForAutomatic(string formula, int indexLevel)
        {
            requiremntResult.text = LevelManager.instance.CalculateRequirementResult(formula).ToString();
            levelTitle.text = "Level " + indexLevel.ToString();
        }
        public void UpdateCurrentResult(double result)
        {
            currentResult.text = result.ToString();
        }
        public void SetBlankForCurrentResult()
        {
            currentResult.text = "0";
        }
        public void UpdateResult(string result)
        {
            resultText.text = $"Result: <color=#FF0000>{result}</color>";
        }
        public void SetUpRepeatAllowance(int number, int operation)
        {
            numberRepeatCount.text = number.ToString();
            operationRepeatCount.text = operation.ToString();
        }
    }
}
