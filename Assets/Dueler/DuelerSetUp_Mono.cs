using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class DuelerSetUp_Mono : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] Dueler_DataBase_SO duelerDatabase;
    [SerializeField] Skill_DataBase_SO skillDatabase;
    [SerializeField] Dueler_Mono owner;
    [SerializeField] bool UseLocal;
    public int duelerId;
    public int[] skills;
    public const byte CustomManualInstantiationEventCode = 1;

    private void Start()
    {
        if (UseLocal)
        {
            LocalSetUp();
        }
        else
        {
            UserSetUp();
        }
    }

    public void LocalSetUp()
    {
        SetUpRPC(duelerId, skills);
    }

    public void UserSetUp()
    {
        if (photonView.IsMine)
        {
            SetUpRPC(DuelerDataSingleton.Instance.VFXId, DuelerDataSingleton.Instance.Skills);
            SetSkillls(DuelerDataSingleton.Instance.Skills);
            DuelerUI_Mono.Instance.Init(owner);
        }
    }
    public void SetUpRPC(int id, int[] skills)
    {
        photonView.RPC(nameof(SetUp), RpcTarget.AllBuffered, id, skills);
    }

    [PunRPC]
    void SetUp(int duelerId, int[] skills)
    {
        SetBody(duelerId);
        //SetSkillls(skills);
        owner.MyStart();
    }

    void SetBody(int duelerId)
    {
        if(owner.Dueler_VFX?.gameObject != null)
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

    void SetSkillls(int[] skill_Ids)
    {

        for(int n = 0; n < skill_Ids.Length; ++n)
        {
            Skill_Base_SO skill_SO = skillDatabase.GetSkillSO(skill_Ids[n]);
            //var skill = PhotonNetwork.Instantiate("Skills/" + skill_SO.PrefabName, Vector3.zero, Quaternion.identity);
            //var skill = Resources.Load<GameObject>("Skills/" + skill_SO.PrefabName) as GameObject;
            var skill = Instantiate(skill_SO.skillPrefab);
            skill.transform.SetParent(owner.transform, false);

            //Skill_Base_Mono s = skill.GetComponent<Skill_Base_Mono>();
            skill.Init(owner, skill_SO, n, owner.Input);
            owner.Skills.Add(skill);
            
            PhotonView photonView = skill.GetComponent<PhotonView>();
            if (PhotonNetwork.AllocateViewID(photonView))
            {
                int skillId = skill_Ids[n];
                object[] data = new object[]
                {
                    skillId, n, photonView.ViewID
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

                PhotonNetwork.RaiseEvent(CustomManualInstantiationEventCode, data, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                Destroy(skill);
            }
            

        }


    }

    public void OnEvent(EventData photonEvent)
    {
        if (!photonView.IsMine)
        {
            if (photonEvent.Code == CustomManualInstantiationEventCode)
            {
                object[] data = (object[])photonEvent.CustomData;

                Skill_Base_SO skill_SO = skillDatabase.GetSkillSO((int)data[0]);
                Skill_Base_Mono skill = (Skill_Base_Mono)Instantiate(skill_SO.skillPrefab);
                skill.transform.SetParent(owner.transform, false);
                skill.Init(owner, skill_SO, (int)data[1], owner.Input);
                owner.Skills.Add(skill);

                PhotonView photonView = skill.GetComponent<PhotonView>();
                photonView.ViewID = (int)data[2];
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

public class DuelerDataSingleton
{
    DuelerData duelerData;
    public int VFXId { get => duelerData.body; set => duelerData.body = value; }
    public int[] Skills => duelerData.skills;

    static DuelerDataSingleton instance;

    public DuelerDataSingleton()
    {
        duelerData = new DuelerData();
    }

    public DuelerDataSingleton(int duelerId, int[] skill)
    {
        duelerData = new DuelerData(duelerId, skill);
    }

    public static DuelerDataSingleton Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new DuelerDataSingleton();
            }
            return instance;
        }
    }


}

[System.Serializable]
public class DuelerData
{
    public int body;
    public int[] skills;

    public DuelerData(int body, int[] skills)
    {
        this.body = body;
        this.skills = skills;
    }
    public DuelerData()
    {
        this.body = 0;
        this.skills = new int[6];
        for(int n = 0; n < skills.Length; ++n)
        {
            skills[n] = n+1;
        }
    }
}