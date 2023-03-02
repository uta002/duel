using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealthView_Mono : MonoBehaviour
{
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    HealthSystem_Mono target;
    public void Init(HealthSystem_Mono target)
    {
        this.target = target;
    }
    
    void Update()
    {
        if (target == null)
            return;
        healthBar.fillAmount = target.HealthNormalized;
        healthText.text = target.CurrentHealth.ToString("N0") + "/" + target.MaxHealth.ToString("N0");
    }
}
