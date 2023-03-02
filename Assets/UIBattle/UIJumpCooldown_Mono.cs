using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIJumpCooldown_Mono : MonoBehaviour
{
    [SerializeField] Image useCoolDown;
    [SerializeField] TextMeshProUGUI jumpCount;
    Dueler_Mono owner;
    Cooldown JumpCooldown => owner.JumpCooldown;

    public void Init(Dueler_Mono target)
    {
        owner = target;
    }

    void Update()
    {
        useCoolDown.fillAmount = Mathf.Clamp01(1f - JumpCooldown.CoolDownTimeNormalized);
        jumpCount.text = owner.CurrentJumpCount.ToString();
    }
}
