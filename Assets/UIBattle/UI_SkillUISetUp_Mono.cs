using System.Collections.Generic;
using UnityEngine;

public class UI_SkillUISetUp_Mono : MonoBehaviour
{
    [SerializeField] Transform skillUIParent;
    [SerializeField] UISkill_Base_Mono skillUIPrefab;
    [SerializeField] Sprite defaultIcon;
    List<UICooldown_Mono> skillUIList = new List<UICooldown_Mono>();

    public void SetSkillUIs(Dueler_Mono owner)
    {
        foreach (Transform n in skillUIParent)
        {
            GameObject.Destroy(n.gameObject);
        }
        skillUIList.Clear();

        for (int n = 0; n < owner.Skills.Count; ++n)
        {
            UISkill_Base_Mono c = Instantiate(skillUIPrefab, skillUIParent);
            var icon = owner.Skills[n].Icon != null ? owner.Skills[n].Icon : defaultIcon;
            c.Init(owner.Skills[n], icon, SaveData.Instance.keyConfig.GetSkillInput(n).GetString());
        }
    }
}
