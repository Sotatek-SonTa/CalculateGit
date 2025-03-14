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
using System.Linq;
namespace CalculateGameplayManager
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] int turnCount;
        public LevelManager levelManager;
        public UIManager uIManager;
        public CalculateSoundManager calculateSoundManager;
        private Dictionary<Button, int> buttonClickCounts = new Dictionary<Button, int>();
        private Dictionary<Button, int> operationClikCount = new Dictionary<Button, int>();
        private int hintIndex = 0;
        private int hintClickCount = 0;
        public static float hintCoolDown = 10f;
        [SerializeField] private DifficultyConfig randomDifficulty;
        public GameConfig gameConfig;

        void Start()
        {
            LoadManager();
            levelManager.LoadLevel(levelManager.indexLevel);
            AddListener();
            ButtonPrefab.OnButtonPressed += HandleButtonPress;
            if (levelManager.indexLevel <= 100)
            {
                levelManager.GenerateTextFromHint(levelManager.currentLevel.hintFormula, levelManager.currentLevel);
                LoadData();
                uIManager.SetUpUI(levelManager.currentLevel);
            }
            else
            {
                SetAutoMaticLevel();
            }
        }
        void LoadManager()
        {
            levelManager = LevelManager.instance;
            uIManager = UIManager.instance;
            calculateSoundManager = CalculateSoundManager.instance;
            gameConfig = GameConfig.instance;
        }
        void LoadData()
        {
            if (levelManager.indexLevel <= 100)
            {
                hintClickCount = levelManager.currentLevel.hintCount;
                turnCount = levelManager.currentLevel.hintFormula.Length - levelManager.visblaeOperators;
            }
            else
            {
                turnCount = levelManager.automaticFormula.Length - levelManager.visblaeOperators;
            }
        }
        void AddListener()
        {
            uIManager.nextLevel.onClick.AddListener(NextLevel);
            uIManager.retry.onClick.AddListener(Retry);
            uIManager.hintButton.onClick.AddListener(ShowNextHintCharcater);
        }
        private void HandleButtonPress(string input, ButtonPrefab buttonPrefab)
        {
            if (turnCount <= 0) return;
            int index = levelManager.formulaSegments.IndexOf("<sprite name=\"blank\">");
            calculateSoundManager.PlaySound(CalculateSoundName.CLICK, 0.3f);

            levelManager.formulaSegments[index] = input;
            levelManager.generatedTexts[index].text = input;
            turnCount--;
            buttonPrefab.StartVFX();
            string updatedExpression = levelManager.BuildExpression(levelManager.formulaSegments);
            if (!string.IsNullOrEmpty(updatedExpression))
            {
                double? result = levelManager.EvaluateExpression(updatedExpression);
                if (result.HasValue)
                {
                    uIManager.UpdateCurrentResult(result.Value);
                }
            }
            if (turnCount == 0)
            {
                levelManager.FinalResult();
            }

            if ("+-*/^!".Contains(input))
            {
                CountClickTime(operationClikCount, levelManager.currentLevel.operationClickCount);
            }
            else
            {
                CountClickTime(buttonClickCounts, levelManager.currentLevel.clickCount);
            }
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

        public void NextLevel()
        {
            if (levelManager.indexLevel <= 100)
            {
                levelManager.indexLevel++;
                ResetLevel();
            }
            else
            {
                levelManager.indexLevel++;
                SetAutoMaticLevel();
            }

        }
        public void Retry()
        {
            if (levelManager.indexLevel <= 100)
            {
                ResetLevel();
            }
            else
            {
                ResetAutoMaticLevel();
            }
        }
        public void ResetLevel()
        {
            if (levelManager.indexLevel <= 100)
            {
                levelManager.LoadLevel(levelManager.indexLevel);
                levelManager.GenerateTextFromHint(levelManager.currentLevel.hintFormula, levelManager.currentLevel);
                turnCount = levelManager.currentLevel.hintFormula.Length - levelManager.visblaeOperators;
                calculateSoundManager.PlaySound(CalculateSoundName.RETRY, 0.2f);
                uIManager.SetUpUI(levelManager.currentLevel);
                ResetHint();
                ResetDictionary();
                ResetUI();
            }
        }
        public void ShowNextHintCharcater()
        {
            calculateSoundManager.PlaySound(CalculateSoundName.RETRY, 0.2f);
            if (hintClickCount == 0) return;
            hintClickCount--;
            if (hintIndex < levelManager.currentLevel.hintFormula.Length)
            {
                StartCoroutine(HintButtonCoolDown());
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
            uIManager.hintButton.interactable = true;
            uIManager.ResetHintDisPlay();
        }
        private void ResetDictionary()
        {
            buttonClickCounts = new Dictionary<Button, int>();
            operationClikCount = new Dictionary<Button, int>();
        }
        IEnumerator HintButtonCoolDown()
        {
            uIManager.hintButton.interactable = false;
            yield return new WaitForSeconds(hintCoolDown);
            if (hintClickCount > 0)
            {
                uIManager.hintButton.interactable = true;
            }
            else
            {
                uIManager.hintButton.interactable = false;
            }
        }
        public void SetAutoMaticLevel()
        {
            randomDifficulty = gameConfig.difficultyConfigs[UnityEngine.Random.Range(0, 4)];
            levelManager.automaticFormula = levelManager.AutomaticGenerateLevel(randomDifficulty);
            Debug.Log(levelManager.automaticFormula);
            levelManager.GenerateTextFromAutomatic(levelManager.automaticFormula, randomDifficulty);
            LoadData();
            uIManager.SetUpUIForAutomatic(levelManager.automaticFormula, levelManager.indexLevel);
            ResetUI();
        }
        public void ResetAutoMaticLevel()
        {
            Debug.Log(levelManager.automaticFormula);
            levelManager.GenerateTextFromAutomatic(levelManager.automaticFormula, randomDifficulty);
            LoadData();
            uIManager.SetUpUIForAutomatic(levelManager.automaticFormula, levelManager.indexLevel);
            ResetUI();

        }
        public void ResetUI()
        {
            uIManager.SetBlankForCurrentResult();
            uIManager.SetInteractable();
            uIManager.ResetActive();
        }
    }
}


