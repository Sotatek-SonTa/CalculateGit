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
        public int indexLevel = 1;
        public LevelManager levelManager;
        public UIManager uIManager;
        public CalculateSoundManager calculateSoundManager;
        private Dictionary<Button, int> buttonClickCounts = new Dictionary<Button, int>();
        private Dictionary<Button, int> operationClikCount = new Dictionary<Button, int>();
        private int hintIndex = 0;
        private int hintClickCount = 0;
        public static float hintCoolDown = 10f;
        void Start()
        {
            LoadManager();
            levelManager.LoadLevel(indexLevel);
            AddListener();
            levelManager.GenerateTextFromHint(levelManager.currentLevel.hintFormula);
            LoadData();
            uIManager.SetUpUI(levelManager.currentLevel);
            ButtonPrefab.OnButtonPressed += HandleButtonPress;
        }
        void LoadManager()
        {
            levelManager = LevelManager.instance;
            uIManager = UIManager.instance;
            calculateSoundManager = CalculateSoundManager.instance;
        }
        void LoadData()
        {
            hintClickCount = levelManager.currentLevel.hintCount;
            turnCount = levelManager.currentLevel.hintFormula.Length - levelManager.visblaeOperators;
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
            if (index == -1) return;

            calculateSoundManager.PlaySound(CalculateSoundName.CLICK, 0.3f);
            levelManager.formulaSegments[index] = input;
            levelManager.generatedTexts[index].text = input;
            turnCount--;
            uIManager.UpdateDisplay(turnCount);

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
            buttonPrefab.StartVFX();
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
            levelManager.GenerateTextFromHint(levelManager.currentLevel.hintFormula);
            turnCount = levelManager.currentLevel.hintFormula.Length - levelManager.visblaeOperators;
            calculateSoundManager.PlaySound(CalculateSoundName.RETRY, 0.2f);
            uIManager.SetUpUI(levelManager.currentLevel);
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
    }
}

