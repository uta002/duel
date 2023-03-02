using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStackableCooldown_Mono : MonoBehaviour
{
    [SerializeField] Image useCoolDown;
    [SerializeField] Image stackCoolDown;
    [SerializeField] TextMeshProUGUI stackCount;
    StackableSkill stackableCooldown;
    
    public void Init(StackableSkill target)
    {
        stackableCooldown = target;
    }

    void Update()
    {
        useCoolDown.fillAmount = Mathf.Clamp01(1f - stackableCooldown.UseCoolDownNormalized);
        stackCoolDown.fillAmount = Mathf.Clamp01(1f - stackableCooldown.StackCoolDownNormalized);
        stackCount.text = stackableCooldown.StackCount.ToString();
    }
}
