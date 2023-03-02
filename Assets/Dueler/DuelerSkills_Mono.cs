using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelerSkills_Mono : MonoBehaviour
{
    /*
    ISkill skill1;
    ISkill skill2;
    ISkill skill3;
    ISkill skill4;
    Dueler_Mono owner;
    IDuelerInput Input => owner?.Input;
    private void Awake()
    {
        owner = GetComponent<Dueler_Mono>();
    }

    void Update()
    {
        if (owner.photonView.IsMine)
        {


        }
    }
    */
}

public interface ISkill
{
    void TryUse();
}

