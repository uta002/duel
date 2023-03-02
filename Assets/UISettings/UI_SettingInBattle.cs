using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_SettingInBattle : MonoBehaviour
{
    public KeyCode targetKey = KeyCode.Escape;
    [SerializeField] GameObject pause;
    [SerializeField] GameObject settings;
    [SerializeField] MouseCursorManager cursorManager;
    [SerializeField] BattleManager battleManager;
    [SerializeField] DuelerUI_Mono ui_Dueler;
    bool IsOpen => pause.gameObject.activeSelf ==false;
    bool IsSetting => settings.gameObject.activeSelf == true;

    void Update()
    {
        if (Input.GetKeyDown(targetKey))
        {
            if (IsSetting)
            {
                HideSettings();
            }
            else
            {
                if (IsOpen)
                {
                    ShowPause();
                }
                else
                {
                    HidePause();
                }
            }
        }
    }

    public void ShowPause()
    {
        pause.SetActive(true);
        cursorManager.UnlockCursor();
    }

    public void HidePause()
    {
        pause.SetActive(false);
        cursorManager.LockCursor();
    }

    public void ShowSettins()
    {
        HidePause();
        Debug.Log("change2Dummy");
        battleManager.MyDueler.SetInput(new InputDuelerDummy());
        settings.SetActive(true);
        cursorManager.UnlockCursor();
    }

    public void HideSettings()
    {
        battleManager.MyDueler.SetInput(SaveData.Instance.keyConfig);
        ui_Dueler.Init(battleManager.MyDueler);
        settings.SetActive(false);
        cursorManager.LockCursor();
        SaveData.Instance.Save();
    }
}

public class EventKey
{
    public KeyCode targetKey = KeyCode.Escape;
    public UnityEvent OnPressEscapeEvent;
    void Update()
    {
        if (Input.GetKeyDown(targetKey))
        {
            OnPressEscapeEvent?.Invoke();
        }
    }
}
