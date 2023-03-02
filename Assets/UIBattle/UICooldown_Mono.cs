using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICooldown_Mono : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image useCoolDown;
    Cooldown cooldown;

    public void Init(Sprite icon, Cooldown target)
    {
        cooldown = target;
        this.icon.sprite = icon;
    }

    void Update()
    {
        useCoolDown.fillAmount = Mathf.Clamp01(1f - cooldown.CoolDownTimeNormalized);
    }
}
