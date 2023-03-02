using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerNameSetter_Mono : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    public void Awake()
    {
        //inputField.onValueChanged.AddListener(OnStringChanged);
        inputField.text = SaveData.Instance.playerName;
        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    public void OnStringChanged(string str)
    {
        SaveData.Instance.playerName = str;
        //PhotonNetwork.LocalPlayer.NickName = str;
    }

    public void OnEndEdit(string str)
    {
        SaveData.Instance.playerName = str;
        PhotonNetwork.LocalPlayer.NickName = str;
    }
}
