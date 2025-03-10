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
        [SerializeField] private TextMeshProUGUI turnLeft;


        public void SetActiveWinUI()
        {
            winLoseUI.SetActive(true);
            winUI.SetActive(true);
        }
        public void SetActiveLoseUI()
        {
            winLoseUI.SetActive(true);
            LoseUI.SetActive(true);
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
            turnLeft.text = (currentLevel.hintFormula.Length - LevelManager.instance.visblaeOperators).ToString();
        }
        public void UpdateDisplay(int turnCount)
        {
            turnLeft.text = turnCount.ToString();
        }
    }
}
