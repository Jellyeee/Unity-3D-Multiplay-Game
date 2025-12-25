using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

//게임 오브젝트의 뼈대
//체력, 데미지 받아들이기, 사망 기능, 사망 이벤트
public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startingHealth = 2f;
    public float health;
    public bool dead { get; protected set; }
    public event Action onDeath;

    //생명체가 활성화 될때 상태를 리셋
    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    // 데미지 처리
    // 호스트에서 먼저 단독 실행되고, 호스트를 통해 다른 클라이언트들에서 일괄 실행됨
    [PunRPC] 
    public void OnDamage(float damage)
    {
       // if (PhotonNetwork.IsMasterClient)
       // {
        health -= damage;


        if (health <= 0 && !dead)
        {
            Die();
        }

    }

    [PunRPC]
    public void RestoreHealth(float newHealth)
    {
        health += newHealth;
        if (dead)
        {

            return;

        }

    }

    [PunRPC]
    public void Die()
    {
        if (onDeath != null)
        {
            onDeath();
        }

        dead = true;
  
    }

}


