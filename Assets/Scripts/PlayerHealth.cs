using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;
    public float damage = 1f;
    private Animator playerAnimator;
    public PlayerMovement playerMovement;
    private float heal = 1f;
    static public int playerdeadcount = 0;
    public AudioSource HitSound;

    public Rigidbody RG;
    public CapsuleCollider CCol;
    public BoxCollider BCol;
    public PhotonView PV;
    public bool IsHit = false;
    public bool isDead = false;
    public UIManager ui;

    public int Myscore = 0;

    private void Awake()
    {
        Random_Spawn();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        ui = GetComponent<UIManager>();

    }


    protected override void OnEnable()
    {
        base.OnEnable();

        if (photonView.IsMine)
        {
            healthSlider.gameObject.SetActive(true);
            healthSlider.maxValue = startingHealth;
            playerMovement.enabled = true;
        }
    }

    [PunRPC]
    public void Player_Die()
    {
        if (!isDead)  // 이미 죽은 플레이어인지 확인
        {
            IsHit = true;
            CCol.enabled = false;
            BCol.enabled = false;
            RG.useGravity = false;
        

            healthSlider.gameObject.SetActive(false);
            playerMovement.enabled = false;
            isDead = true;
            playerAnimator.SetTrigger("Die");

            // 마스터 클라이언트에서만 playerdeadcount 증가
            if (PhotonNetwork.IsMasterClient)
            {
                playerdeadcount++;  // 중복 증가 방지
                UnityEngine.Debug.Log("Master Client - Player Dead Count Incremented: " + playerdeadcount);
                PV.RPC("IncreaseDeadCount", RpcTarget.All);
            }

            // 생존자 수를 업데이트
            UIManager.instance.OnPlayerDeath(); // 사망 이벤트 발생 시 UI 업데이트

        }
        else
        {
            UnityEngine.Debug.Log("Player is already dead");
        }
    }

    [PunRPC]
    public void IncreaseDeadCount()
    {
        // 모든 클라이언트에서 playerdeadcount를 동기화
        UnityEngine.Debug.Log("DeadCount updated to: " + playerdeadcount);
    }

    public void Restore_Health(float newHealth)
    {
        PV.RPC("RestoreHealth", RpcTarget.All, newHealth);

        PV.RPC("SetHealthSlider", RpcTarget.All);

    }

    public void TakeDamage(float takedamage)
    {
        playerAnimator.SetTrigger("Hit");
        PV.RPC("OnDamage", RpcTarget.All, takedamage);
        UIManager.instance.PlayerHit2(health, PV.Owner.ActorNumber);

        PV.RPC("SetHealthSlider", RpcTarget.All);
        HitSound.Play();
        StartCoroutine(HitDelay());

    }
    
    [PunRPC]
    public void SetHealthSlider()
    {
        healthSlider.value = health;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            var animator = other.transform.root.GetComponent<Animator>();

                if (!IsHit)
                {
                    IsHit = true;
                    TakeDamage(damage);
                    if (health <= 0)
                    {
                        PV.RPC("Player_Die", RpcTarget.All);
                    }

                }

        }

        if (other.tag == "Heal")
        {
            if (health < 2)
            {
                Restore_Health(heal);
            }
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(other.gameObject);
            }
        }

    }

    private void Random_Spawn()
    {
        if(photonView.IsMine)
        {
            Vector3 randomSpawnPos = Random.insideUnitSphere * 60f;
            randomSpawnPos.y = 8f;

            transform.position = randomSpawnPos;
        }
    }

    IEnumerator HitDelay()
    {
        yield return new WaitForSeconds(1.5f);
        IsHit = false;
    }

}

