using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DuelerSelect_Mono : MonoBehaviour
{
    [SerializeField] Dueler_DataBase_SO dueler_DataBase;
    [SerializeField] Transform root;
    [SerializeField] Dueler_VFX_Mono currentPreview;
    [SerializeField] Image selectedDuelerIcon;
    [SerializeField] TextMeshProUGUI selectedDuelerName;


    [SerializeField] Transform content;

    [SerializeField] UI_IconButton_Mono duelerButtonPrefab;
    [SerializeField] Sprite defaultIcon;

    [SerializeField] GameObject duelerSelectWindow;

    List<UI_IconButton_Mono> duelerButtonList = new List<UI_IconButton_Mono>();

    public void ShowAllDueler()
    {
        foreach (Transform n in content)
        {
            GameObject.Destroy(n.gameObject);
        }
        duelerButtonList.Clear();

        for (int n = 0; n < dueler_DataBase.database.Length; ++n)
        {
            DuelerStatus_SO dueler = dueler_DataBase.GetDueler_SO(n);
            UI_IconButton_Mono b = Instantiate(duelerButtonPrefab, content);
            b.Init(dueler.icon != null ? dueler.icon : defaultIcon);
            int id = n;
            b.button.onClick.AddListener(()=>SelectDueler(id));
            b.button.onClick.AddListener(()=> duelerSelectWindow.SetActive(false));
        }
    }

    public void SelectDueler(int id)
    {
        if(currentPreview != null)
        {
            Destroy(currentPreview.gameObject);
        }

        DuelerStatus_SO dueler = dueler_DataBase.GetDueler_SO(id);
        currentPreview = Instantiate(dueler.VFX, root);
        currentPreview.transform.localPosition = Vector3.zero;
        currentPreview.transform.localRotation = Quaternion.identity;

        selectedDuelerIcon.sprite = dueler.icon != null ? dueler.icon : defaultIcon;
        selectedDuelerName.text = dueler.DuelerName;

        //DuelerDataSingleton.Instance.VFXId = id;
        SaveData.Instance.customizeData.body = id;
    }
}
