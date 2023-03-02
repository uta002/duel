using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeUI_Mono : MonoBehaviour
{
    [SerializeField] Skill_DataBase_SO database;

    [SerializeField] UI_Skill_Info_Mono skillInfo;
    // [SerializeField] Button equipButton;
    [SerializeField] Transform currentSkillParent;
    [SerializeField] Transform contentParent;

    [SerializeField] UI_CurrentSkill_Mono currentSkillPrefab;
    List<UI_CurrentSkill_Mono> currentSkillList = new List<UI_CurrentSkill_Mono>();

    [SerializeField] UI_CandidateSkill_Mono candidateSkillPrefab;
    List<UI_CurrentSkill_Mono> candidateSkillList = new List<UI_CurrentSkill_Mono>();

    [SerializeField] Sprite defSkillIcon;

    int selectedCurrentSkill;
    int selectedSkillId;

    private void Start()
    {
        ClearCurrentSkill();
        DisplayCurrentSkill();

        ClearCandidateSkill();
        ShowCandidateSkills();

        CurrentSkillPressed(0);
    }

    public void DisplayCurrentSkill()
    {
        for(int n=0;n< SaveData.Instance.customizeData.skills.Length; ++n)
        {
            UI_CurrentSkill_Mono skill = Instantiate(currentSkillPrefab, currentSkillParent);
            int skillId = SaveData.Instance.customizeData.skills[n];
            //Debug.Log(skillId);
            Skill_Base_SO skill_SO = database.GetSkillSO(skillId);
            var icon = skill_SO.Icon != null ? skill_SO.Icon : defSkillIcon;
            skill.Init(skillId, icon, SaveData.Instance.keyConfig.GetSkillInput(n).GetString());
            int num = n;
            skill.Button.onClick.AddListener(() => CurrentSkillPressed(num));
            currentSkillList.Add(skill);
        }
    }

    void ClearCurrentSkill()
    {
        foreach (Transform n in currentSkillParent)
        {
            GameObject.Destroy(n.gameObject);
        }
        currentSkillList.Clear();
    }

    public void CurrentSkillPressed(int n)
    {
        selectedCurrentSkill = n;
        foreach(var elem in currentSkillList)
        {
            elem.Deselect();
        }
        currentSkillList[n].Select();

        ShowSkillInfo(currentSkillList[n].SkillID);
    }

    void ClearCandidateSkill()
    {
        foreach (Transform n in contentParent)
        {
            GameObject.Destroy(n.gameObject);
        }
        candidateSkillList.Clear();
    }


    public void CandidateSkillPressed(int n)
    {
        ShowSkillInfo(n);
        selectedSkillId = n;
    }
    public void ShowCandidateSkills()
    {
        for(int n = 0; n < database.database.Length; ++n)
        {
            UI_CandidateSkill_Mono candidate = Instantiate(candidateSkillPrefab, contentParent);
            //Skill_Base_SO skill_SO = database.database[n];
            Skill_Base_SO skill_SO = database.GetSkillSO(n);
            var icon = skill_SO.Icon != null ? skill_SO.Icon : defSkillIcon;

            candidate.Init(icon);
            int num = n;
            candidate.Button.onClick.AddListener(()=> CandidateSkillPressed(num));
        }
    }

    void ShowSkillInfo(int skillId)
    {
        Skill_Base_SO skill_SO = database.GetSkillSO(skillId);
        var icon = skill_SO.Icon != null ? skill_SO.Icon : defSkillIcon;
        skillInfo.Init(icon, skill_SO.SkillName, skill_SO.description);
    }

    public void Equip()
    {
        SaveData.Instance.customizeData.skills[selectedCurrentSkill] = selectedSkillId;
        ClearCurrentSkill();
        DisplayCurrentSkill();
        CurrentSkillPressed(selectedCurrentSkill);
    }

    private void OnDestroy()
    {
        SaveData.Instance.Save();
        
    }
}
