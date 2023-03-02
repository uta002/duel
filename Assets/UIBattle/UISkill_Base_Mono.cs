using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkill_Base_Mono : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image useCoolDown;
    [SerializeField] Image canUse;
    [SerializeField] TextMeshProUGUI buttonText;
    Cooldown cooldown;
    Skill_Base_Mono skill;
    public void Init(Skill_Base_Mono skill, Sprite icon, string buttonStr)
    {
        this.skill = skill;
        cooldown = skill.Cooldown;
        this.icon.sprite = icon;
        buttonText.text = buttonStr;
    }

    void Update()
    {
        canUse.gameObject.SetActive(!skill.CanUse);
        useCoolDown.fillAmount = Mathf.Clamp01(1f - cooldown.CoolDownTimeNormalized);
    }
}
