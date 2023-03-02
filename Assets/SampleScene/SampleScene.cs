using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SampleScene : MonoBehaviourPunCallbacks
{
    public bool isOfflineMode;
    [SerializeField] Camera mainCamera;
    [SerializeField] DuelerSetUpper_Mono setUpper;
    [SerializeField] DuelerUI_Mono duelerUI;

    //[SerializeField] DuelerData playerData;
    [SerializeField] DuelerData enemyData;

    //[SerializeField] InputDuelerKM inputKM;
    [SerializeField] InputDuelerProb inputEnemy;

    [SerializeField] bool dummy;
    [SerializeField] int random;
    [SerializeField] bool paticular;
    //[SerializeField] AI_Input_Random inputEnemy;
    void Start()
    {
        if (isOfflineMode)
        {
            PhotonNetwork.OfflineMode = true;
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        //Time.timeScale = 0.5f;
        SpawnPlayer();

        //SpawnRandomEnemy(inputEnemy);
        

        for (int n = 0; n < random; n++)
        {
            SpawnRandomEnemy(inputEnemy);
        }
        if (dummy)
        {
            SpawnEnemy(new InputDuelerDummy());
        }

        if (paticular)
        {
            SpawnEnemy(inputEnemy);
        }

        //SpawnEnemy(inputEnemy1);
        //SpawnEnemy(inputEnemy2);
        //SpawnEnemy(inputEnemy3, 1);
        //SpawnEnemy(inputEnemy4, 1);
        //SpawnEnemy(inputEnemy5, 1);
    }

    void SpawnEnemy(IDuelerInput input, int teamID = 0)
    {
        SpawnEnemy(input, enemyData.body, enemyData.skills, teamID);
    }

    void SpawnRandomEnemy(IDuelerInput input, int teamID = 0)
    {
        int skillNum = 6;
        int[] skills = new int[skillNum];
        for (int n = 0; n < skillNum; n++)
        {
            skills[n] = setUpper.Skill_DataBase_SO.GetRandomSkillSO().id;
        }

        int body = setUpper.Dueler_DataBase_SO.GetRandomID();

        SpawnEnemy(input, body, skills, teamID);

    }

    void SpawnEnemy(IDuelerInput input, int bodyId, int[] skills, int teamID = 0)
    {
        float range = 7f;
        var pos = new Vector3(Random.Range(-range, range), 5f, Random.Range(-range, range));

        var dueler = setUpper.SetUp(teamID, pos, bodyId, skills, input);
        dueler.gameObject.AddComponent<LookPlayer>().Init(dueler);
    }

    void SpawnPlayer()
    {
        DuelerData playerData = SaveData.Instance.customizeData;
        InputDuelerKM inputKM = SaveData.Instance.keyConfig;

        float range = 3f;
        var pos = new Vector3(Random.Range(-range, range), 4f, Random.Range(-range, range));

        var dueler = setUpper.SetUp(1, pos, playerData.body, playerData.skills, inputKM);



        dueler.SetCamera(mainCamera);

        duelerUI.Init(dueler);
    }


    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, Screen.width - 20, Screen.height - 20), PhotonNetwork.ServerTimestamp.ToString(), GUIStyle.none);
    }
}

[System.Serializable]
public class InputDuelerKM : IDuelerInput
{
    public IButtonState jump;
    public IButtonState dash;

    [SerializeReference] public IButtonState[] skillInputs = new IButtonState[] { 
        new KeybordButton(KeyCode.Mouse0), 
        new KeybordButton(KeyCode.Mouse1), 
        new KeybordButton(KeyCode.Q), 
        new KeybordButton(KeyCode.E), 
        new KeybordButton(KeyCode.R), 
        new KeybordButton(KeyCode.F) 
    };


    public Vector2 LookInput => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    public Vector3 MoveInput => new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

    public IButtonState JumpInput => jump;

    public IButtonState DashInput => dash;

    public IButtonState[] SkillInputs => skillInputs;

    public IButtonState GetSkillInput(int num) => skillInputs[num % skillInputs.Length];


    public InputDuelerKM()
    {
        jump = new KeybordButton(KeyCode.Space);
        dash = new KeybordButton(KeyCode.LeftShift);

        skillInputs = new IButtonState[] {
        new KeybordButton(KeyCode.Mouse0),
        new KeybordButton(KeyCode.Mouse1),
        new KeybordButton(KeyCode.Q),
        new KeybordButton(KeyCode.E),
        new KeybordButton(KeyCode.R),
        new KeybordButton(KeyCode.F)
        };
    }

}

[System.Serializable]
public class InputDuelerProb : IDuelerInput
{
    public Vector2 look;
    public Vector3 move;
    public ButtonRandom jump;
    public ButtonRandom dash;
    public ButtonRandom skill0;
    public ButtonRandom skill1;
    public ButtonRandom skill2;
    public ButtonRandom skill3;
    public ButtonRandom skill4;
    public ButtonRandom skill5;
    public Vector2 LookInput => look;
    public Vector3 MoveInput => move;

    public IButtonState JumpInput => jump;


    public IButtonState DashInput => dash;

    IButtonState[] skillInputs;
    public IButtonState[] SkillInputs
    {
        get
        {
            if(skillInputs == null)
            {
                skillInputs = new IButtonState[] { skill0, skill1, skill2, skill3, skill4, skill5 };
            }
            return skillInputs;
        }
    }

    public IButtonState GetSkillInput(int num)
    {
        switch (num)
        {
            case 0:
                return skill0;
            case 1:
                return skill1;
            case 2:
                return skill2;
            case 3:
                return skill3;
            case 4:
                return skill4;
            case 5:
                return skill5;
            default:
                return new DummyButton();
        }

    }



}

[System.Serializable]
public class ButtonRandom : IButtonState
{
    public float prob = 0.1f;
    public bool ButtonDown => Lottery(prob);

    public bool Button => false;

    public bool ButtonUp => false;

    bool Lottery(float prob) => Random.value < prob * Time.deltaTime;
    public string GetString()
    {
        return "Random";
    }

}