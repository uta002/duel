using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Skill_Base_Mono : MonoBehaviourPunCallbacks
{
    IDuelerInput input;
    [SerializeField] protected Cooldown cooldown;
    protected Dueler_Mono owner;
    public Dueler_Mono Owner => owner;

    protected int skillSlotID;
    public int SKillSlotID => skillSlotID;

    public Sprite Icon => baseSO?.Icon;
    Skill_Base_SO baseSO;
    public Skill_Base_SO SkillSO => baseSO;
    public virtual void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input) { 
        this.owner = owner;
        baseSO = so;
        
        SetInput(input, skillSlotID);
    }

    public void SetInput(IDuelerInput input,int skillSlotID)
    {
        this.skillSlotID = skillSlotID;
        this.input = input;
    }

    public Cooldown Cooldown => cooldown;
    public virtual bool CanUse => cooldown.CanUse && owner.CanUseSkills;

    public virtual void Start() { }

    public virtual void MyUpdate()
    {
        //Debug.Log(input.SkillInputs.Length);
        if (input.SkillInputs[skillSlotID].ButtonDown)
        {
            ButtonDown();
        }
        if (input.SkillInputs[skillSlotID].Button)
        {
            Button();
        }
        if (input.SkillInputs[skillSlotID].ButtonUp)
        {
            ButtonUp();
        }
    }

    public virtual void ButtonUp()
    {

    }

    public virtual void ButtonDown()
    {
        TryActivate();
    }

    public virtual void Button()
    {

    }

    public virtual void TryActivate()
    {
        if (CanUse)
        {
            Activated();
            cooldown.Use();
        }
    }

    protected abstract void Activated();
}
