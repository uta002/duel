using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class DuelerSetUpper_Mono : MonoBehaviour, IOnEventCallback
{
    [SerializeField] string prefabName;
    [SerializeField] Dueler_DataBase_SO duelerDatabase;
    [SerializeField] Skill_DataBase_SO skillDatabase;

    public Dueler_DataBase_SO Dueler_DataBase_SO => duelerDatabase;
    public Skill_DataBase_SO Skill_DataBase_SO => skillDatabase;

    public const byte DuelerSpawnEventCode = 1;
    public const byte SkillCustomEventCode = 2;

    public Dueler_Mono SetUp(int teamID, Vector3 pos, int vfxId, int[] skill_Ids, IDuelerInput input)
    {
         
        //object[] duelerData = { vfxId , skill_Ids.Length, teamID };
        var obj = PhotonNetwork.Instantiate(prefabName, pos, Quaternion.identity);
        var owner = obj.GetComponent<Dueler_Mono>();
        owner.PlayerName = name;
        owner.Init(teamID, input);

        SetBody(owner, vfxId);
        SendSetVFXEvent(owner.ID, vfxId, teamID);



        owner.MyStart();

        for(int n = 0; n < skill_Ids.Length; ++n)
        {
            Skill_Base_SO skill_SO = skillDatabase.GetSkillSO(skill_Ids[n]);
            var skill = Instantiate(skill_SO.skillPrefab);

            skill.transform.SetParent(owner.transform, false);
            skill.Init(owner, skill_SO, n, owner.Input);
            owner.Skills.Add(skill);

            PhotonView photonView = skill.photonView;
            if (PhotonNetwork.AllocateViewID(photonView))
            {
                int skillId = skill_Ids[n];
                object[] data = new object[]
                {
                    owner.photonView.ViewID, skillId, n, photonView.ViewID
                };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                PhotonNetwork.RaiseEvent(SkillCustomEventCode, data, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                Destroy(skill);
            }


        
        }
        return owner;
    }

    void SendSetVFXEvent(int ownerViewId, int vfxId, int teamID)
    {
        object[] data = new object[]
        {
            ownerViewId, vfxId, teamID
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        PhotonNetwork.RaiseEvent(DuelerSpawnEventCode, data, raiseEventOptions, sendOptions);

    }

    void SetBody(Dueler_Mono owner, int duelerId)
    {
        if (owner.Dueler_VFX?.gameObject != null)
        {
            Destroy(owner.Dueler_VFX.gameObject);
        }

        DuelerStatus_SO dueler_SO = duelerDatabase.GetDueler_SO(duelerId);
        dueler_SO.SetDueler(owner);
        Dueler_VFX_Mono vfx = Instantiate(dueler_SO.VFX, owner.Root);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localRotation = Quaternion.identity;
        owner.Dueler_VFX = vfx;
    }

    public void OnEvent(EventData photonEvent)
    {
        //Debug.Log("onEvent");
        if(photonEvent.Code == DuelerSpawnEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Dueler_Mono owner = PhotonNetwork.GetPhotonView((int)data[0]).GetComponent<Dueler_Mono>();
            owner.SetTeamID((int)data[2]);

            if (!owner.photonView.IsMine)
            {
                SetBody(owner, (int)data[1]);
                owner.SetInput(new InputDuelerDummy());
            }
        }
        else if (photonEvent.Code == SkillCustomEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Dueler_Mono owner = PhotonNetwork.GetPhotonView((int)data[0]).GetComponent<Dueler_Mono>();
            if (!owner.photonView.IsMine)
            {
                Skill_Base_SO skill_SO = skillDatabase.GetSkillSO((int)data[1]);
                Skill_Base_Mono skill = (Skill_Base_Mono)Instantiate(skill_SO.skillPrefab);
                skill.transform.SetParent(owner.transform, false);
                skill.Init(owner, skill_SO, (int)data[2], owner.Input);
                owner.Skills.Add(skill);

                skill.photonView.ViewID = (int)data[3];
            }
        }
    }


    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
