using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Skill_Info_Mono : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI SkillName;
    public TextMeshProUGUI Description;

    public void Init(Sprite icon, string skillName, string description)
    {
        Icon.sprite = icon;
        SkillName.text = skillName;
        Description.text = description;
    }
}
