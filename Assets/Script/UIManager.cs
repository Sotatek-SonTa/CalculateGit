using System.Collections;
using System.Collections.Generic;
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
        public TextMeshProUGUI levelTitle;
        public TextMeshProUGUI hintDisplay;
        public TextMeshProUGUI hintCount;
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
    }

}
