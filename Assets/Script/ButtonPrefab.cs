using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class ButtonPrefab : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    public static event Action<string,ButtonPrefab> OnButtonPressed;
    [SerializeField] private string vaule;
    [SerializeField] private TextMeshProUGUI vauleText;
    private void Awake()
    {
        vaule = vauleText.text;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnButtonPressed?.Invoke(vaule,this);
        });
    }
    public void StartVFX()
    {
        StartCoroutine(VFXduration());
    }
    IEnumerator VFXduration()
    {
        particleSystem.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        particleSystem.gameObject.SetActive(false);
    }
}


