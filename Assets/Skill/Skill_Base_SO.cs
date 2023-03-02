using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(fileName ="Skill", menuName ="Skill/General")]
public class Skill_Base_SO : ScriptableObject
{
    public int id;
    public Skill_Base_Mono skillPrefab;
    public string PrefabName => skillPrefab.name;
    [SerializeField] string skillName;
    public string SkillName => skillName == "" ? PrefabName : skillName;
    [SerializeField] Sprite icon;
    public Sprite Icon => icon;
    //public float coolDown;
    //public string coolDownTest => "CD:" + coolDown + "\n";
    public string coolDownText => "CD:" + skillPrefab.Cooldown.CoolDownTime + "\n";
    public string Description => coolDownText + description;
    public string description;


    protected virtual Skill_Base_Mono SetParam(GameObject obj)
    {
        var skill = obj.GetComponent<Skill_Base_Mono>();
        //skill.Cooldown.CoolDownTime = coolDown;
        return skill;
    }

    public bool Equals(int id)
    {
        return this.id == id;
    } 
}


