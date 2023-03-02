using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHealthView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] UIHealthView_Mono healthView;
    [SerializeField] WorldToCanvas_Mono worldToCanvas;

    public void Init(Dueler_Mono target, Camera camera, bool usePhotonNickName = true)
    {
        string playerName = usePhotonNickName ? target.photonView.Owner.NickName : target.PlayerName;
        int teamID = target.photonView.Owner.GetData<int>(GameScene.teamID_Key);
        nameText.text = teamID + ":" + playerName;
        healthView.Init(target.HealthSystem);
        worldToCanvas.Init(target.transform, camera);
    }
}
