using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using ExitGames.Client.Photon;
using Photon.Realtime;


public class GameScene : MonoBehaviourPunCallbacks
{
    public const string teamID_Key = "TeamID";
    [SerializeField] Camera mainCamera;
    [SerializeField] DuelerSetUpper_Mono setUpper;
    [SerializeField] DuelerUI_Mono duelerUI;
    [SerializeField] BattleManager battleManager;


    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        SpawnPlayer();
    }

    int GetTeamID()
    {
        int[] teamCount = new int[2]; // roomCount
        foreach (var p in PhotonNetwork.PlayerListOthers)
        {
            teamCount[p.GetData<int>(teamID_Key)]++;
            //teamCount[p.TeamID]++;
        }
        Debug.Log("team0:" + teamCount[0]);
        Debug.Log("team1:" + teamCount[1]);
        return teamCount[0] < teamCount[1]? 0:1;
    }

    void SpawnPlayer()
    {
        float range = 3f;
        var pos = new Vector3(Random.Range(-range, range), 5f, Random.Range(-range, range));




        int teamID = GetTeamID();
        PhotonNetwork.LocalPlayer.SetData<int>(teamID_Key, teamID);
        var dueler = setUpper.SetUp(teamID, pos, SaveData.Instance.customizeData.body, SaveData.Instance.customizeData.skills, SaveData.Instance.keyConfig);



        dueler.SetCamera(mainCamera);

        duelerUI.Init(dueler);

        battleManager.MyDueler = dueler;
    }

}



public static class PlayerPropertiesExtensions
{
    private const string ScoreKey = "Score";
    private const string MessageKey = "Message";

    private static readonly Hashtable propsToSet = new Hashtable();

    // プレイヤーのスコアを取得する
    public static int GetScore(this Player player)
    {
        return (player.CustomProperties[ScoreKey] is int score) ? score : 0;
    }

    // プレイヤーのメッセージを取得する
    public static string GetMessage(this Player player)
    {
        return (player.CustomProperties[MessageKey] is string message) ? message : string.Empty;
    }

    // プレイヤーのスコアを設定する
    public static void SetScore(this Player player, int score)
    {
        propsToSet[ScoreKey] = score;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    // プレイヤーのメッセージを設定する
    public static void SetMessage(this Player player, string message)
    {
        propsToSet[MessageKey] = message;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }


    public static void SetData<DataType>(this Player player, string key, DataType data)
    {
        propsToSet[key] = data;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    public static DataType GetData<DataType>(this Player player, string key)
    {
        return (player.CustomProperties[key] is DataType data) ? data : default;
    }
}