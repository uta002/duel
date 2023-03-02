using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPlayer : MonoBehaviour
{
    [SerializeField] Dueler_Mono owner;
    Dueler_Mono target;
    public void Init(Dueler_Mono owner)
    {
        this.owner = owner;
    }

    void Update()
    {
        if (target == null)
        {
            SearchTarget();
        }
        else
        {
            owner.CameraPiv.rotation = Quaternion.LookRotation(target.HeartPos -  owner.CameraPiv.position);
        }
        
    }

    void SearchTarget()
    {
        foreach(var elem in DuelerManager_Mono.AllDuelers)
        {
            if(elem.TeamID != owner.TeamID)
            {
                target = elem;
            }
        }
    }
}
