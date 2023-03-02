using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MatchmakingView_Mono : MonoBehaviourPunCallbacks
{
    [SerializeField] string gameScene = "GameScene";
    [SerializeField] Transform roomButtonParent;
    RoomList roomList = new RoomList();
    List<RoomButton> roomButtonList = new List<RoomButton>();
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = PhotonNetwork.IsConnected;

        int roomId = 1;
        foreach (Transform child in roomButtonParent)
        {
            if (child.TryGetComponent<RoomButton>(out var roomButton))
            {
                roomButton.Init(this, roomId++);
                roomButtonList.Add(roomButton);
            }
        }
    }

    public override void OnJoinedLobby()
    {
        // ロビーに参加したら、ルーム参加ボタンを押せるようにする
        canvasGroup.interactable = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
        roomList.Update(changedRoomList);

        // 全てのルーム参加ボタンの表示を更新する
        foreach (var roomButton in roomButtonList)
        {
            if (roomList.TryGetRoomInfo(roomButton.RoomName, out var roomInfo))
            {
                roomButton.SetPlayerCount(roomInfo.PlayerCount);
            }
            else
            {
                roomButton.SetPlayerCount(0);
            }
        }
    }

    public void OnJoiningRoom()
    {
        // ルーム参加処理中は、全てのルーム参加ボタンを押せないようにする
        canvasGroup.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        // ルームへの参加が成功したら、UIを非表示にする
        gameObject.SetActive(false);

        PhotonNetwork.IsMessageQueueRunning = false;

        //PhotonNetwork.LoadLevel(gameScene);
        //LoadScene(gameScene, LoadSceneMode.Single);
        SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Single);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // ルームへの参加が失敗したら、再びルーム参加ボタンを押せるようにする
        canvasGroup.interactable = true;
    }

}

public class RoomList : IEnumerable<RoomInfo>
{
    private Dictionary<string, RoomInfo> dictionary = new Dictionary<string, RoomInfo>();

    public void Update(List<RoomInfo> changedRoomList)
    {
        foreach (var info in changedRoomList)
        {
            if (!info.RemovedFromList)
            {
                dictionary[info.Name] = info;
            }
            else
            {
                dictionary.Remove(info.Name);
            }
        }
    }

    public void Clear()
    {
        dictionary.Clear();
    }

    // 指定したルーム名のルーム情報があれば取得する
    public bool TryGetRoomInfo(string roomName, out RoomInfo roomInfo)
    {
        return dictionary.TryGetValue(roomName, out roomInfo);
    }

    public IEnumerator<RoomInfo> GetEnumerator()
    {
        foreach (var kvp in dictionary)
        {
            yield return kvp.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
