using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelerUI_Mono : MonoBehaviour
{
    public static DuelerUI_Mono Instance => instance;
    static DuelerUI_Mono instance;

    [SerializeField] TargetUI_Mono targetUI;
    [SerializeField] UIStackableCooldown_Mono dashUI;
    [SerializeField] UIJumpCooldown_Mono jumpUI;
    [SerializeField] UI_SkillUISetUp_Mono skillUI;

    [SerializeField] UIHealthView_Mono healthView;

    [SerializeField] Camera camera;


    [SerializeField] Transform otherHealthViewParent;
    [SerializeField] BattleHealthView battleHealthViewPrefab;

    [SerializeField] bool usePhotonNickName;
    List<BattleHealthView> battleHealthViewList = new List<BattleHealthView>();

    Dueler_Mono owner;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Init(Dueler_Mono owner)
    {
        this.owner = owner;
        targetUI.owner = owner;
        dashUI.Init(owner.DashCooldown);
        jumpUI.Init(owner);
        skillUI.SetSkillUIs(owner);
        healthView.Init(owner.HealthSystem);

        InitOtherDuelerHealthView();
    }

    void InitOtherDuelerHealthView()
    {
        foreach(var d in DuelerManager_Mono.AllDuelers)
        {
            OnAddDueler(d);
        }
        DuelerManager_Mono.Instance.OnAddEvent.AddListener(OnAddDueler);
    }

    public void OnAddDueler(Dueler_Mono dueler)
    {
        //Debug.Log(owner);
        if(dueler != owner)
        {
            BattleHealthView healthView = Instantiate(battleHealthViewPrefab, otherHealthViewParent);
            healthView.Init(dueler, camera, usePhotonNickName);

            dueler.OnDestroyEvent.AddListener(() => DestroyBattleHealthView(healthView));
        }
    }

    void DestroyBattleHealthView(BattleHealthView healthView)
    {
        if (healthView != null)
        {
            Destroy(healthView.gameObject);
        }
    }

    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }
}
