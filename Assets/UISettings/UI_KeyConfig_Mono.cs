using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_KeyConfig_Mono : MonoBehaviour
{
    [SerializeField] UI_SettingInBattle ui_setting;

    [SerializeField] KeyEnum changeKey;
    [SerializeField] TextMeshProUGUI currentKeyText;
    [SerializeField] GameObject nowWaitingUI;
    [SerializeField] Button button;

    private void Start()
    {
        Init();
    }
    private void Init()
    {
        currentKeyText.text = GetKeybordButton(SaveData.Instance.keyConfig, changeKey).GetString();
        nowWaitingUI.SetActive(false);
    }
    public void StartChangeKey()
    {
        nowWaitingUI.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(ChangeKey());
    }

    public void CancelChangeKey()
    {
        StopAllCoroutines();
        nowWaitingUI.SetActive(false);

    }

    IEnumerator ChangeKey()
    {
        ui_setting.enabled = false;
        button.enabled = false;
        currentKeyText.text = "ƒL[‚ð“ü—Í";
        yield return new WaitUntil(() => Input.anyKeyDown);
        KeyCode pressed = KeyCode.None;
        foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                button.enabled = true;
                currentKeyText.text = GetKeybordButton(SaveData.Instance.keyConfig, changeKey).GetString();
                CancelChangeKey();
                ui_setting.enabled = true;
                yield break;
            }
            else if (Input.GetKeyDown(code))
            {
                pressed = code;
            }
        }
        var keybordButton = new KeybordButton(pressed);
        currentKeyText.text = keybordButton.GetString();
        SetKeybordButton(SaveData.Instance.keyConfig, keybordButton, changeKey);
        SaveData.Instance.Save();
        nowWaitingUI.SetActive(false);
        button.enabled = true;
        ui_setting.enabled = true;


    }

    KeyCode DownKeyCheck()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    return code;
                }
            }
        }
        return KeyCode.None;
    }

    public static void SetKeybordButton(InputDuelerKM target, IButtonState button, KeyEnum targetKey)
    {
        switch (targetKey)
        {
            case KeyEnum.Jump:
                target.jump = button;
                break;
            case KeyEnum.Dash:
                target.dash = button;
                break;
            case KeyEnum.Attack0:
                target.skillInputs[0] = button;
                break;
            case KeyEnum.Attack1:
                target.skillInputs[1] = button;
                break;
            case KeyEnum.Attack2:
                target.skillInputs[2] = button;
                break;
            case KeyEnum.Attack3:
                target.skillInputs[3] = button;
                break;
            case KeyEnum.Attack4:
                target.skillInputs[4] = button;
                break;
            case KeyEnum.Attack5:
                target.skillInputs[5] = button;
                break;
            default:
                break;
        }
    }

    public static KeybordButton GetKeybordButton(InputDuelerKM input, KeyEnum key)
    {
        switch (key)
        {
            case KeyEnum.Jump:
                return input.jump as KeybordButton;
            case KeyEnum.Dash:
                return input.dash as KeybordButton;
            case KeyEnum.Attack0:
                return input.GetSkillInput(0) as KeybordButton;
            case KeyEnum.Attack1:
                return input.GetSkillInput(1) as KeybordButton;
            case KeyEnum.Attack2:
                return input.GetSkillInput(2) as KeybordButton;
            case KeyEnum.Attack3:
                return input.GetSkillInput(3) as KeybordButton;
            case KeyEnum.Attack4:
                return input.GetSkillInput(4) as KeybordButton;
            case KeyEnum.Attack5:
                return input.GetSkillInput(5) as KeybordButton;
            default:
                break;
        }
        return default;
    }
}


public enum KeyEnum
{
    Jump,
    Dash,
    Attack0,
    Attack1,
    Attack2,
    Attack3,
    Attack4,
    Attack5,
}