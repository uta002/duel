using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class BattleManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform[] spawnPos;
    [SerializeField] UIBattleResult_Mono resultUI;
    [SerializeField] LoadScene_Mono loadScene;
    [SerializeField] MouseCursorManager mouseCursorManager;
    [SerializeField] UI_SettingInBattle ui_setting;

    List<Dueler_Mono> readyList;
    List<Dueler_Mono> deadList;
    public Dueler_Mono MyDueler { get; set; }
    int MyTeamID => MyDueler.TeamID;

    bool IsBattleStarted;

    private void Awake()
    {
        IsBattleStarted = false;
        readyList = new List<Dueler_Mono>();
    }

    public void BattleStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(BattleStartRPC), RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void BattleStartRPC()
    {
        IsBattleStarted = true;
        deadList = new List<Dueler_Mono>();
        readyList.RemoveAll((e) => e == null);
        foreach(var elem in readyList)
        {
            elem.SetTeamID(elem.photonView.Owner.GetData<int>(GameScene.teamID_Key));
            elem.transform.position = spawnPos[elem.TeamID].position;
            elem.MyStart();
        }
    }


    public void AddReady(Dueler_Mono dueler)
    {
        if (!readyList.Contains(dueler))
        {
            readyList.Add(dueler);
        }

        dueler.HealthSystem.OnDeadEvent.AddListener(()=> Dead(dueler));

        if(readyList.Count == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            BattleStart();
        }
    }

    public void Dead(Dueler_Mono dueler)
    {
        if(dueler != null)
        {
            if (!deadList.Contains(dueler))
            {
                deadList.Add(dueler);
            }
        }
        JudgeFinish();
    }

    public void JudgeFinish()
    {
        if (PhotonNetwork.IsMasterClient && IsBattleStarted)
        {
            List<Dueler_Mono> remainList = new List<Dueler_Mono>(readyList);
            foreach(var elem in deadList)
            {
                remainList.Remove(elem);
            }

            for (int i = 1; i < remainList.Count; i++)
            {
                if (remainList[0].TeamID != remainList[i].TeamID)
                {
                    return;
                }
            }
            photonView.RPC(nameof(Won), RpcTarget.All, remainList[0].TeamID);
        }
        //readyList.Clear();
    }

    [PunRPC]
    public void Won(int teamID)
    {
        if (teamID == MyTeamID)
        {
            ShowResult("Victory");
        }
        else
        {
            ShowResult("Defeat");
        }
        readyList.Clear();
        IsBattleStarted = false;
    }

    void ShowResult(string resultText)
    {
        resultUI.gameObject.SetActive(true);
        resultUI.ResultText.text = resultText;

        ui_setting.enabled = false;
        ui_setting.HidePause();
        mouseCursorManager.UnlockCursor();
    }

    public void ReadyRPC()
    {
        //int teamID = readyList.Count;
        photonView.RPC(nameof(Ready), RpcTarget.All, MyDueler.ID);
        resultUI.gameObject.SetActive(false);

        ui_setting.enabled = true;
        mouseCursorManager.LockCursor();

        MyDueler.transform.position = spawnPos[MyDueler.TeamID % spawnPos.Length].position;
    }

    [PunRPC]
    public void Ready(int duelerID)
    {

        Dueler_Mono dueler = PhotonNetwork.GetPhotonView(duelerID).GetComponent<Dueler_Mono>();
        //dueler.SetTeamID(teamID);
        AddReady(dueler);

        //dueler.transform.position = spawnPos[dueler.TeamID % spawnPos.Length].position;
        dueler.MyStart();

    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        loadScene.LoadOnlineSelectScene();
    }
}