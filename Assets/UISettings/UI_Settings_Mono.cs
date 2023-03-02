using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Settings_Mono : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI sensitivityText;

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        slider.value = SaveData.Instance.sensitivity;
        sensitivityText.text = SaveData.Instance.sensitivity.ToString("N2");
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    public void OnSensitivityChanged(float value)
    {
        SaveData.Instance.sensitivity = value;
        sensitivityText.text = SaveData.Instance.sensitivity.ToString("N2");
    }
    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        
    }
}
