using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public class HealthSystem_Mono : MonoBehaviourPunCallbacks
{
    public UnityEvent OnDeadEvent = new UnityEvent();
    [SerializeField] float maxHealth;
    float currentHealth;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;


    public void SetHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public float HealthNormalized => currentHealth / maxHealth;

    private void Start()
    {
        InitHealth();
    }

    public void InitHealth() => currentHealth = maxHealth;

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (photonView.IsMine)
        {
            currentHealth = Mathf.Max(0f, currentHealth - damageInfo.damage);
            if(currentHealth == 0f)
            {
                photonView.RPC(nameof(Dead), RpcTarget.All);
            }
            else
            {
                photonView.RPC(nameof(ReduceHealth), RpcTarget.All, damageInfo.sourceID, damageInfo.damage, currentHealth);
            }
            
        }
    }

    [PunRPC]
    public void ReduceHealth(int source, float damage, float health, PhotonMessageInfo info)
    {
        currentHealth = health;
    }

    [PunRPC]
    public void Dead()
    {
        currentHealth = 0f;
        OnDeadEvent?.Invoke();
    }
}
